module internal NBomber.DomainServices.Reports.TxtReport

open System
open System.Collections.Generic
open System.Data

open ConsoleTables
open Serilog

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain
open NBomber.Domain.Stats
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

module TxtTestInfo =

    let printTestInfo (testInfo: TestInfo) =
        [$"test suite: '{testInfo.TestSuite}'"; $"test name: '{testInfo.TestName}'"]
        |> String.concatLines
        |> String.appendNewLine

module TxtStatusCodeStats =

    let printScenarioHeader (scenarioName: string) =
        $"status codes for scenario: '{scenarioName}'"

    let printStatusCodeTable (scnStats: ScenarioStats) =
        let okStatusCodes =
            scnStats.StatusCodes
            |> Seq.filter(fun x -> not x.IsError)
            |> Seq.map(fun x ->
                [ x.StatusCode.ToString()
                  x.Count.ToString()
                  x.Message ]
            )
            |> List.ofSeq

        let failStatusCodes =
            scnStats.StatusCodes
            |> Seq.filter(fun x -> x.IsError)
            |> Seq.map(fun x ->
                [ x.StatusCode.ToString()
                  x.Count.ToString()
                  x.Message ]
            )
            |> List.ofSeq

        let okStatusCodesCount =
            scnStats.StatusCodes
            |> Seq.filter(fun x -> not x.IsError)
            |> Seq.sumBy(fun x -> x.Count)

        let failStatusCodesCount =
            scnStats.StatusCodes
            |> Seq.filter(fun x -> x.IsError)
            |> Seq.sumBy(fun x -> x.Count)

        let okNotAvailableStatusCodes =
            if okStatusCodesCount < scnStats.OkCount then
                List.singleton [
                  "ok (no status)"
                  (scnStats.OkCount - okStatusCodesCount).ToString()
                  String.Empty
                ]
            else
                List.Empty

        let failNotAvailableStatusCodes =
            if failStatusCodesCount < scnStats.FailCount then
                List.singleton [
                  "fail (no status)"
                  (scnStats.FailCount - failStatusCodesCount).ToString()
                  String.Empty
                ]
            else
                List.Empty

        let allStatusCodes = okNotAvailableStatusCodes @ okStatusCodes @ failNotAvailableStatusCodes @ failStatusCodes

        let table = ConsoleTable("status code", "count", "message")
        allStatusCodes |> Seq.iter(fun x -> table.AddRow(x.[0], x.[1], x.[2]) |> ignore)
        table.ToStringAlternative()

module TxtNodeStats =

    let private printDataKb (bytes: int) =
        $"{bytes |> Statistics.Converter.fromBytesToKb} KB"

    let private printAllData (bytes: int64) =
        $"{bytes |> Statistics.Converter.fromBytesToMb} MB"

    let private printScenarioHeader (scnStats: ScenarioStats) =
        $"scenario: '{scnStats.ScenarioName}', duration: '{scnStats.Duration}'" +
        $", ok count: {scnStats.OkCount}, fail count: {scnStats.FailCount}, all data: {scnStats.AllBytes |> printAllData} MB"

    let private printLoadSimulation (simulation: LoadSimulation) =
        let simulationName = LoadTimeLine.getSimulationName(simulation)

        match simulation with
        | RampConstant (copies, during)     ->
            $"load simulation: '{simulationName}'" +
            $", copies: {copies}" +
            $", during: '{during}'"

        | KeepConstant (copies, during)     ->
            $"load simulation: '{simulationName}'" +
            $", copies: {copies}" +
            $", during: '{during}'"

        | RampPerSec (rate, during)         ->
            $"load simulation: '{simulationName}'" +
            $", rate: {rate}" +
            $", during: '{during}'"

        | InjectPerSec (rate, during)       ->
            $"load simulation: '{simulationName}'" +
            $", rate: {rate}" +
            $", during: '{during}'"

        | InjectPerSecRandom (minRate, maxRate, during) ->
            $"load simulation: '{simulationName}'" +
            $", min rate: {minRate}" +
            $", max rate: {maxRate}" +
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

        let reqCount =
            $"all = {allReqCount}" +
            $", ok = {okReqCount}" +
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
            $"min = {okDataTransfer.MinBytes |> printDataKb}" +
            $", mean = {okDataTransfer.MeanBytes |> printDataKb}" +
            $", max = {okDataTransfer.MaxBytes |> printDataKb}" +
            $", all = {okDataTransfer.AllBytes |> printAllData}"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name]
          ["request count"; reqCount]
          ["latency"; okLatencies]
          ["latency percentile"; okPercentile]
          if okDataTransfer.AllBytes > 0L then ["data transfer"; okDt] ]

    let private createFailStepStatsRow (i) (s: StepStats) =
        let name = s.StepName
        let okReqCount = s.Ok.Request.Count
        let failReqCount = s.Fail.Request.Count
        let allReqCount = okReqCount + failReqCount
        let failRPS = s.Fail.Request.RPS
        let failLatency = s.Fail.Latency
        let failDataTransfer = s.Fail.DataTransfer

        let reqCount =
            $"all = {allReqCount}" +
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
            $"min = {failDataTransfer.MinBytes |> printDataKb}" +
            $", mean = {failDataTransfer.MeanBytes |> printDataKb}" +
            $", max = {failDataTransfer.MaxBytes |> printDataKb}" +
            $", all = {failDataTransfer.AllBytes |> printAllData}"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name]
          ["request count"; reqCount]
          ["latency"; failLatencies]
          ["latency percentile"; failPercentile]
          if failDataTransfer.AllBytes > 0L then ["data transfer"; failDt] ]

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

    let private failStepStatsExist (stepStats: StepStats[]) =
        stepStats |> Seq.exists(fun stats -> stats.Fail.Request.Count > 0)

    let private printScenarioStats (scnStats: ScenarioStats) (simulations: LoadSimulation list) =
        [ printScenarioHeader(scnStats)
          printLoadSimulations(simulations)
          printOkStepStatsTable(scnStats.StepStats)

          if failStepStatsExist(scnStats.StepStats) then
              printFailStepStatsTable(scnStats.StepStats)

          if scnStats.StatusCodes.Length > 0 then
             TxtStatusCodeStats.printScenarioHeader(scnStats.ScenarioName)
             TxtStatusCodeStats.printStatusCodeTable(scnStats) ]

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

    let private printHintsTable (hints: HintResult[]) =
        let hintTable = ConsoleTable("source", "name", "hint")

        hints
        |> Seq.iter(fun hint -> hintTable.AddRow(hint.SourceType, hint.SourceName, hint.Hint) |> ignore)

        hintTable.ToStringAlternative()

    let printHints (hints: HintResult[]) =
        if hints.Length > 0 then
            seq {
                yield printHintsHeader()
                yield printHintsTable(hints)
            }
            |> String.concatLines
        else
            String.Empty

let print (logger: ILogger) (sessionResult: NodeSessionResult) (simulations: IDictionary<string, LoadSimulation list>) =
    try
        logger.Verbose("TxtReport.print")

        [ TxtTestInfo.printTestInfo sessionResult.NodeStats.TestInfo
          TxtNodeStats.printNodeStats sessionResult.NodeStats simulations
          TxtPluginStats.printPluginStats sessionResult.NodeStats
          TxtHints.printHints sessionResult.Hints ]
        |> String.concatLines
    with
    | ex ->
        logger.Error(ex, "TxtReport.print failed")
        "Could not generate report"
