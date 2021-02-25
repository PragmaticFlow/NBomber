module internal NBomber.DomainServices.Reporting.TxtReport

open System
open System.Collections.Generic
open System.Data

open ConsoleTables

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.HintsAnalyzer
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

module TxtTestInfo =

    let printTestInfo (testInfo: TestInfo) =
        [$"test suite: '{testInfo.TestSuite}'"; $"test name: '{testInfo.TestName}'"]
        |> String.concatLines
        |> String.appendNewLine

module TxtErrorStats =

    let printScenarioErrorStatsHeader (scnStats: ScenarioStats) =
        $"errors for scenario: '{scnStats.ScenarioName}'"

    let printErrorStatsTable (errorStats: ErrorStats[]) =
        let errorTable = ConsoleTable("error code", "count", "message")
        errorStats |> Seq.iter(fun error -> errorTable.AddRow(error.ErrorCode, error.Count, error.Message) |> ignore)
        errorTable.ToStringAlternative()

module TxtNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) =
        $"scenario: '{scnStats.ScenarioName}', duration: '{scnStats.Duration}'" +
        $", ok count: '{scnStats.OkCount}', fail count: '{scnStats.FailCount}', all data: '{scnStats.AllDataMB}' MB"

    let private printLoadSimulation (simulation: LoadSimulation) =
        let simulationName = LoadTimeLine.getSimulationName(simulation)

        match simulation with
        | RampConstant (copies, during)     ->
            $"load simulation: '{simulationName}'" +
            $", copies: '{copies}'" +
            $", during: '{during}'"

        | KeepConstant (copies, during)     ->
            $"load simulation: '{simulationName}'" +
            $", copies: '{copies}'" +
            $", during: '{during}'"

        | RampPerSec (rate, during)         ->
            $"load simulation: '{simulationName}'" +
            $", rate: '{rate}'" +
            $", during: '{during}'"

        | InjectPerSec (rate, during)       ->
            $"load simulation: '{simulationName}'" +
            $", rate: '{rate}'" +
            $", during: '{during}'"

        | InjectPerSecRandom (minRate, maxRate, during) ->
            $"load simulation: '{simulationName}'" +
            $", min rate: '{minRate}'" +
            $", max rate: '{maxRate}'" +
            $", during: '{during}'"

    let private printLoadSimulations (simulations: LoadSimulation list) =
        simulations |> Seq.map printLoadSimulation |> String.concatLines

    let private createOkStepStatsRow (i) (s: StepStats) =
        let name = s.StepName
        let okReqCount = s.Ok.Request.Count
        let failReqCount = s.Fail.Request.Count
        let allReqCount = okReqCount + failReqCount
        let okRPS = s.Ok.Request.RPS
        let okLatency = s.Ok.Latency
        let okDataTransfer = s.Ok.DataTransfer
        let okDtMin = $"%.3f{okDataTransfer.MinKb}"
        let okDtMean = $"%.3f{okDataTransfer.MeanKb}"
        let okDtMax = $"%.3f{okDataTransfer.MaxKb}"
        let okDtAll = $"%.3f{okDataTransfer.AllMB}"

        let reqCount =
            $"all = {allReqCount}" +
            $", ok = {okReqCount}" +
            $", fail = {failReqCount}" +
            $", RPS = {okRPS}"

        let okLatencies =
            $"min = {okLatency.MinMs}" +
            $", mean = {okLatency.MeanMs}" +
            $", max = {okLatency.MaxMs}" +
            $", StdDev = {okLatency.StdDev}"

        let okPercentile =
            $"50%% = {okLatency.Percent50}" +
            $", 75%% = {okLatency.Percent75}" +
            $", 95%% = {okLatency.Percent95}" +
            $", 99%% = {okLatency.Percent99}"

        let okDt =
            $"min = {okDtMin} KB" +
            $", mean = {okDtMean} KB" +
            $", max = {okDtMax} KB" +
            $", all = {okDtAll} MB"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name]
          ["request count"; reqCount]
          ["latency"; okLatencies]
          ["latency percentile"; okPercentile]
          if okDataTransfer.AllMB > 0.0 then ["data transfer"; okDt] ]

    let private createFailStepStatsRow (i) (s: StepStats) =
        let name = s.StepName
        let okReqCount = s.Ok.Request.Count
        let failReqCount = s.Fail.Request.Count
        let allReqCount = okReqCount + failReqCount
        let failRPS = s.Fail.Request.RPS
        let failLatency = s.Fail.Latency
        let failDataTransfer = s.Fail.DataTransfer
        let failDtMin = $"%.3f{failDataTransfer.MinKb}"
        let failDtMean = $"%.3f{failDataTransfer.MeanKb}"
        let failDtMax = $"%.3f{failDataTransfer.MaxKb}"
        let failDtAll = $"%.3f{failDataTransfer.AllMB}"

        let reqCount =
            $"all = {allReqCount}" +
            $", ok = {okReqCount}" +
            $", fail = {failReqCount}" +
            $", RPS = {failRPS}"

        let failLatencies =
            $"min = {failLatency.MinMs}" +
            $", mean = {failLatency.MeanMs}" +
            $", max = {failLatency.MaxMs}" +
            $", StdDev = {failLatency.StdDev}"

        let failPercentile =
            $"50%% = {failLatency.Percent50}" +
            $", 75%% = {failLatency.Percent75}" +
            $", 95%% = {failLatency.Percent95}" +
            $", 99%% = {failLatency.Percent99}"

        let failDt =
            $"min = {failDtMin} KB" +
            $", mean = {failDtMean} KB" +
            $", max = {failDtMax} KB" +
            $", all = {failDtAll} MB"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name]
          ["request count"; reqCount]
          ["latency"; failLatencies]
          ["latency percentile"; failPercentile]
          if failDataTransfer.AllMB > 0.0 then ["data transfer"; failDt] ]

    let private printOkStepStatsTable (stepStats: StepStats[]) =
        let table = ConsoleTable("step", "ok stats")

        stepStats
        |> Array.mapi createOkStepStatsRow
        |> Seq.concat
        |> Seq.iter(fun row -> table.AddRow(row.[0], row.[1]) |> ignore)

        table.ToStringAlternative()

    let private printFailStepStatsTable (stepStats: StepStats[]) =
        let table = ConsoleTable("step", "fail stats")

        stepStats
        |> Seq.filter(fun stats -> stats.Fail.Request.Count > 0)
        |> Seq.mapi createFailStepStatsRow
        |> Seq.concat
        |> Seq.iter(fun row -> table.AddRow(row.[0], row.[1]) |> ignore)

        table.ToStringAlternative()

    let private errorStepStatsExist (stepStats: StepStats[]) =
        stepStats |> Seq.exists(fun stats -> stats.Fail.Request.Count > 0)

    let private printScenarioStats (scnStats: ScenarioStats) (simulations: LoadSimulation list) =
        [ printScenarioHeader(scnStats)
          printLoadSimulations(simulations)
          printOkStepStatsTable(scnStats.StepStats)
          if errorStepStatsExist(scnStats.StepStats) then printFailStepStatsTable(scnStats.StepStats)
          if scnStats.ErrorStats.Length > 0 then
             TxtErrorStats.printScenarioErrorStatsHeader(scnStats)
             TxtErrorStats.printErrorStatsTable(scnStats.ErrorStats) ]

    let printNodeStats (stats: NodeStats) (loadSimulations: IDictionary<string, LoadSimulation list>) =
        stats.ScenarioStats
        |> Array.map(fun scnStats ->
            printScenarioStats scnStats loadSimulations.[scnStats.ScenarioName]
        )
        |> Seq.concat
        |> String.concatLines

module TxtPluginStats =

    let private printPluginStatsHeader (table: DataTable) =
        $"plugin stats: '{table.TableName}'"

    let private printPluginStatsTable (table: DataTable) =
        let columnNames = table.GetColumns() |> Array.map(fun x -> x.ColumnName)
        let columnCaptions = table.GetColumns() |> Array.map(fun x -> x.GetColumnCaptionOrName())
        let consoleTable = ConsoleTable(columnCaptions)

        table.GetRows()
        |> Array.map(fun x -> columnNames |> Array.map(fun columnName -> x.[columnName]))
        |> Array.iter(fun x -> consoleTable.AddRow(x) |> ignore)

        consoleTable.ToStringAlternative()

    let printPluginStats (stats: NodeStats) =
        stats.PluginStats
        |> Seq.collect(fun dataSet -> dataSet.GetTables())
        |> Seq.collect(fun table ->
            seq {
                printPluginStatsHeader(table)
                printPluginStatsTable(table)
            })
        |> String.concatLines

module TxtHints =
    let private printHintsHeader () =
        "hints:"

    let private printHintsTable (hints: HintResult list) =
        let hintTable = ConsoleTable("source", "name", "hint")

        hints
        |> Seq.iter(fun hint -> hintTable.AddRow(hint.SourceType, hint.SourceName, hint.Hint) |> ignore)

        hintTable.ToStringAlternative()

    let printHints (hints: HintResult list) =
        if hints.Length > 0 then
            seq {
                yield printHintsHeader()
                yield printHintsTable(hints)
            }
            |> String.concatLines
        else
            String.Empty

let print (stats) (hints) (simulations: IDictionary<string, LoadSimulation list>) =
    [TxtTestInfo.printTestInfo stats.TestInfo
     TxtNodeStats.printNodeStats stats simulations
     TxtPluginStats.printPluginStats stats
     TxtHints.printHints hints]
    |> String.concatLines
