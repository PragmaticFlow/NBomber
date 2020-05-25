module internal NBomber.DomainServices.Reporting.MdReport

open System
open System.Data

open NBomber.Contracts
open NBomber.Extensions

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
            [
                createTableHeader(header)
                createTableSeparator(header)
                createTableRows(rows)
            ]
            |> String.concatLines

        | []            -> String.Empty

module  MdUtilityNodeStats =

    let private headerScenario (scnStats: ScenarioStats) =
        [
            sprintf "# Scenario: `%s`" (scnStats.ScenarioName.Replace('_', ' '))
            ""
            sprintf "- Duration: `%A`" scnStats.Duration
            ""
        ]
        |> String.concatLines

    let private headerStepStats = [ "step"; "details" ]

    let private  rowsStepStats (s: StepStats) =
        let name = sprintf "`%s`" s.StepName
        let count = sprintf "all = `%i`, OK = `%i`, failed = `%i`" s.RequestCount s.OkCount s.FailCount
        let times = sprintf "RPS = `%i`, min = `%i`, mean = `%i`, max = `%i`" s.RPS s.Min s.Mean s.Max
        let percentile = sprintf "50%% = `%i`, 75%% = `%i`, 95%% = `%i`, StdDev = `%i`" s.Percent50 s.Percent75 s.Percent95 s.StdDev
        let dataTransfer = sprintf "min = `%.3f Kb`, mean = `%.3f Kb`, max = `%.3f Kb`, all = `%.3f MB`" s.MinDataKb s.MeanDataKb s.MaxDataKb s.AllDataMB

        [
            yield [ "name"; name ]
            yield [ "request count"; count ]
            yield [ "response time"; times ]
            yield [ "response time percentile"; percentile ]

            if s.AllDataMB > 0.0 then
                yield [ "data transfer"; dataTransfer ]
        ]

    let printNodeStats (stats: NodeStats) =
        stats.ScenarioStats
        |> Seq.collect(fun scnStats ->
            seq {
                headerScenario scnStats

                scnStats.StepStats
                |> List.ofSeq
                |> List.collect rowsStepStats
                |> List.append [ headerStepStats ]
                |> MdUtility.toMdTable
                |> String.appendNewLine
            })
        |> String.concatLines

module MdUtilityPluginStats =

    let inline private headerPluginStats (table: DataTable) =
        sprintf "# Statistics: `%s`" table.TableName

    let private headerTable (table: DataTable) =
        table.GetColumns()
        |> Seq.map(fun col -> col.GetColumnCaptionOrName())
        |> List.ofSeq

    let private rowsTable (table: DataTable) =
        let columns = table.GetColumns()

        table.GetRows()
        |> Seq.map(fun row -> columns |> Seq.map(fun col -> row.[col] |> string) |> List.ofSeq)
        |> List.ofSeq

    let printPluginStats (stats: NodeStats) =
        stats.PluginStats
        |> Seq.collect(fun pluginStat -> pluginStat.GetTables())
        |> Seq.collect(fun table ->
            seq {
                headerPluginStats table
                |> String.appendNewLine

                table
                |> rowsTable
                |> List.append ([headerTable(table)])
                |> MdUtility.toMdTable
                |> String.appendNewLine

            })
        |> String.concatLines

let print (stats: NodeStats) =
    [
        MdUtilityNodeStats.printNodeStats(stats)
        MdUtilityPluginStats.printPluginStats(stats)
    ]
    |> String.concatLines
