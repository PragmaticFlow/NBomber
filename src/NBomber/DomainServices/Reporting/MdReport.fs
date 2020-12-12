module internal NBomber.DomainServices.Reporting.MdReport

open System
open System.Data

open NBomber.Contracts
open NBomber.DomainServices
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

module MdUtility =

    let inline private createTableHeader (header) =
        String.Join("|", header |> Seq.map(fun x -> sprintf "__%s__" x))

    let inline private createTableSeparator (header: string list) =
        String.Join("|", Array.create header.Length "---")

    let private createTableRows (rows: string list list) =
        rows |> Seq.map(fun row -> String.Join("|", row)) |> String.concatLines

    let toMdTable (headerWithRows: string list list) =
        match headerWithRows with
        | header :: rows ->
            [createTableHeader(header); createTableSeparator(header); createTableRows(rows)]
            |> String.concatLines

        | [] -> String.Empty

module MdTestInfo =
    let printTestInfo (testInfo: TestInfo) =
        [sprintf "> test suite: `%s`" testInfo.TestSuite
         ">"
         sprintf "> test name: `%s`" testInfo.TestName
         ""]
        |> String.concatLines

module MdErrorStats =

    let headerScenarioErrorStats (scnStats: ScenarioStats) =
        sprintf "> errors for scenario: `%s`" scnStats.ScenarioName

    let printScenarioErrorStats (errorStats: ErrorStats[]) =
        errorStats
        |> Seq.map(fun error -> [error.ErrorCode.ToString(); error.Count.ToString(); error.Message])
        |> Seq.append [["error code"; "count"; "message"]]
        |> Seq.toList

module MdNodeStats =

    let private headerScenario (scnStats: ScenarioStats) =
        let scnName = scnStats.ScenarioName |> String.replace("_", " ")
        [sprintf "> scenario: `%s`, duration: `%A`, ok count: `%i`, fail count: `%i`, all data: %f MB" scnName scnStats.Duration scnStats.OkCount scnStats.FailCount scnStats.AllDataMB
         ""]
        |> String.concatLines

    let private headerStepStats = ["step"; "details"]

    let private  rowsStepStats (s: StepStats) =
        let name = sprintf "`%s`" s.StepName
        let count = sprintf "all = `%i`, ok = `%i`, failed = `%i`" s.RequestCount s.OkCount s.FailCount
        let times = sprintf "RPS = `%i`, min = `%i`, mean = `%i`, max = `%i`" s.RPS s.Min s.Mean s.Max
        let percentile = sprintf "50%% = `%i`, 75%% = `%i`, 95%% = `%i`, 99%% = `%i`, StdDev = `%i`" s.Percent50 s.Percent75 s.Percent95 s.Percent99 s.StdDev
        let dataTransfer = sprintf "min = `%.3f KB`, mean = `%.3f KB`, max = `%.3f KB`, all = `%.3f MB`" s.MinDataKb s.MeanDataKb s.MaxDataKb s.AllDataMB

        [ ["name"; name]
          ["request count"; count]
          ["latency"; times]
          ["latency percentile"; percentile]
          if s.AllDataMB > 0.0 then ["data transfer"; dataTransfer] ]

    let printNodeStats (stats: NodeStats) =
        stats.ScenarioStats
        |> Seq.collect(fun scnStats ->
            seq {
                    headerScenario(scnStats)

                    scnStats.StepStats
                    |> Seq.collect rowsStepStats
                    |> Seq.append [headerStepStats]
                    |> Seq.toList
                    |> MdUtility.toMdTable
                    |> String.appendNewLine

                    if scnStats.ErrorStats.Length > 0 then
                        MdErrorStats.headerScenarioErrorStats(scnStats)
                        |> String.appendNewLine

                        MdErrorStats.printScenarioErrorStats(scnStats.ErrorStats)
                        |> MdUtility.toMdTable
                        |> String.appendNewLine
            })
        |> String.concatLines

module MdPluginStats =

    let inline private headerPluginStats (table: DataTable) =
        sprintf "> plugin stats: `%s`" table.TableName

    let private headerTable (table: DataTable) =
        table.GetColumns()
        |> Seq.map(fun col -> col.GetColumnCaptionOrName())
        |> Seq.toList

    let private rowsTable (table: DataTable) =
        let columns = table.GetColumns()

        table.GetRows()
        |> Seq.map(fun row -> columns |> Seq.map(fun col -> row.[col] |> string) |> List.ofSeq)
        |> List.ofSeq

    let printPluginStats (stats: NodeStats) =
        stats.PluginStats
        |> PluginStats.getStatsTables
        |> Seq.collect(fun table ->
            seq {
                yield table |> headerPluginStats |> String.appendNewLine

                yield table
                      |> rowsTable
                      |> List.append [headerTable(table)]
                      |> MdUtility.toMdTable
                      |> String.appendNewLine
            })
        |> String.concatLines

let print (stats: NodeStats) =
    [MdTestInfo.printTestInfo stats.TestInfo
     MdNodeStats.printNodeStats stats
     MdPluginStats.printPluginStats stats]
    |> String.concatLines
