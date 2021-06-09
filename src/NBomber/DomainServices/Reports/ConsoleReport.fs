module internal NBomber.DomainServices.Reports.ConsoleReport

open System
open System.Collections.Generic
open System.Data

open Serilog

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain
open NBomber.Domain.Stats
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions
open NBomber.Infra

module ConsoleTestInfo =

    let printTestInfo (testInfo: TestInfo) =
        [ Console.addHeader("test info")
          Console.addLine(String.Empty)
          Console.addLine($"test suite: '{testInfo.TestSuite |> Console.escapeMarkup |> Console.highlightPrimary}'")
          Console.addLine($"test name: '{testInfo.TestName |> Console.escapeMarkup |> Console.highlightPrimary}'")
          Console.addLine(String.Empty) ]

module ConsoleStatusCodesStats =

    let printScenarioHeader (scenarioName: string)  =
        Console.addLine($"status codes for scenario: {scenarioName |> Console.highlightPrimary}")

    let private createTableRows (scnStats: ScenarioStats)=
        let okStatusCodes =
            scnStats.StatusCodes
            |> Seq.filter(fun x -> not x.IsError)
            |> Seq.map(fun x ->
                [ x.StatusCode.ToString() |> Console.escapeMarkup |> Console.highlightSuccess
                  x.Count.ToString()
                  x.Message |> Console.escapeMarkup |> Console.highlightDanger ]
            )
            |> List.ofSeq

        let failStatusCodes =
            scnStats.StatusCodes
            |> Seq.filter(fun x -> x.IsError)
            |> Seq.map(fun x ->
                [ x.StatusCode.ToString() |> Console.escapeMarkup |> Console.highlightDanger
                  x.Count.ToString()
                  x.Message |> Console.escapeMarkup |> Console.highlightDanger ]
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
                  "ok (no status)" |> Console.highlightSuccess
                  (scnStats.OkCount - okStatusCodesCount).ToString()
                  String.Empty
                ]
            else
                List.Empty

        let failNotAvailableStatusCodes =
            if failStatusCodesCount < scnStats.FailCount then
                List.singleton [
                  "fail (no status)" |> Console.highlightDanger
                  (scnStats.FailCount - failStatusCodesCount).ToString()
                  String.Empty
                ]
            else
                List.Empty

        let allStatusCodes = okNotAvailableStatusCodes @ okStatusCodes @ failNotAvailableStatusCodes @ failStatusCodes
        allStatusCodes

    let printStatusCodeTable (scnStats: ScenarioStats)  =
        let headers = ["status code"; "count"; "message"]
        let rows = scnStats |> createTableRows |> Seq.toList
        Console.addTable headers rows

module ConsoleNodeStats =

    let private printDataKb (bytes: int) =
        $"{bytes |> Statistics.Converter.fromBytesToKb |> Console.highlightPrimary} KB"

    let private printAllData (bytes: int64) =
        $"{bytes |> Statistics.Converter.fromBytesToMb |> Console.highlightPrimary} MB"

    let private printScenarioHeader (scnStats: ScenarioStats) =
        [ Console.addLine($"scenario: '{scnStats.ScenarioName |> Console.escapeMarkup |> Console.highlightPrimary}'")
          Console.addLine($"duration: '{scnStats.Duration |> Console.highlightPrimary}'" +
                          $", ok count: {scnStats.OkCount |> Console.highlightPrimary}" +
                          $", fail count: {scnStats.FailCount |> Console.highlightDanger}" +
                          $", all data: {scnStats.AllBytes |> printAllData}") ]

    let private printLoadSimulation (simulation: LoadSimulation) =
        let simulationName = LoadTimeLine.getSimulationName(simulation)

        match simulation with
        | RampConstant (copies, during)     ->
            $"load simulation: '{simulationName |> Console.highlightPrimary}'" +
            $", copies: {copies |> Console.highlightPrimary}" +
            $", during: '{during |> Console.highlightPrimary}'"

        | KeepConstant (copies, during)     ->
            $"load simulation: '{simulationName |> Console.highlightPrimary}'" +
            $", copies: {copies |> Console.highlightPrimary}" +
            $", during: '{during |> Console.highlightPrimary}'"

        | RampPerSec (rate, during)         ->
            $"load simulation: '{simulationName |> Console.highlightPrimary}'" +
            $", rate: {rate |> Console.highlightPrimary}" +
            $", during: '{during |> Console.highlightPrimary}'"

        | InjectPerSec (rate, during)       ->
            $"load simulation: '{simulationName |> Console.highlightPrimary}'" +
            $", rate: {rate |> Console.highlightPrimary}" +
            $", during: '{during |> Console.highlightPrimary}'"

        | InjectPerSecRandom (minRate, maxRate, during) ->
            $"load simulation: '{simulationName |> Console.highlightPrimary}'" +
            $", min rate: {minRate |> Console.highlightPrimary}" +
            $", max rate: {maxRate |> Console.highlightPrimary}" +
            $", during: '{during |> Console.highlightPrimary}'"

        |> Console.addLine

    let private printLoadSimulations (simulations: LoadSimulation list) =
        simulations |> Seq.map printLoadSimulation

    let private createOkStepStatsRow (i) (s: StepStats) =
        let name = s.StepName
        let okReqCount = s.Ok.Request.Count
        let failReqCount = s.Fail.Request.Count
        let allReqCount = okReqCount + failReqCount
        let okRPS = s.Ok.Request.RPS
        let okLatency = s.Ok.Latency
        let okDataTransfer = s.Ok.DataTransfer

        let reqCount =
            $"all = {allReqCount |> Console.highlightSuccess}" +
            $", ok = {okReqCount |> Console.highlightSuccess}" +
            $", RPS = {okRPS |> Console.highlightSuccess}"

        let okLatencies =
            $"min = {okLatency.MinMs |> Console.highlightSuccess}" +
            $", mean = {okLatency.MeanMs |> Console.highlightSuccess}" +
            $", max = {okLatency.MaxMs |> Console.highlightSuccess}" +
            $", StdDev = {okLatency.StdDev |> Console.highlightSuccess}"

        let okPercentile =
            $"50%% = {okLatency.Percent50 |> Console.highlightSuccess}" +
            $", 75%% = {okLatency.Percent75 |> Console.highlightSuccess}" +
            $", 95%% = {okLatency.Percent95 |> Console.highlightSuccess}" +
            $", 99%% = {okLatency.Percent99 |> Console.highlightSuccess}"

        let okDt =
            $"min = {okDataTransfer.MinBytes |> printDataKb}" +
            $", mean = {okDataTransfer.MeanBytes |> printDataKb}" +
            $", max = {okDataTransfer.MaxBytes |> printDataKb}" +
            $", all = {okDataTransfer.AllBytes |> printAllData}"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name |> Console.highlightSecondary]
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
            $"all = {allReqCount |> Console.highlightSuccess}" +
            $", fail = {failReqCount |> Console.highlightDanger}" +
            $", RPS = {failRPS |> Console.highlightDanger}"

        let failLatencies =
            $"min = {failLatency.MinMs |> Console.highlightDanger}" +
            $", mean = {failLatency.MeanMs |> Console.highlightDanger}" +
            $", max = {failLatency.MaxMs |> Console.highlightDanger}" +
            $", StdDev = {failLatency.StdDev |> Console.highlightDanger}"

        let failPercentile =
            $"50%% = {failLatency.Percent50 |> Console.highlightDanger}" +
            $", 75%% = {failLatency.Percent75 |> Console.highlightDanger}" +
            $", 95%% = {failLatency.Percent95 |> Console.highlightDanger}" +
            $", 99%% = {failLatency.Percent99 |> Console.highlightDanger}"

        let failDt =
            $"min = {failDataTransfer.MinBytes |> printDataKb}" +
            $", mean = {failDataTransfer.MeanBytes |> printDataKb}" +
            $", max = {failDataTransfer.MaxBytes |> printDataKb}" +
            $", all = {failDataTransfer.AllBytes |> printAllData}"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name |> Console.highlightSecondary]
          ["request count"; reqCount]
          ["latency"; failLatencies]
          ["latency percentile"; failPercentile]
          if failDataTransfer.AllBytes > 0L then ["data transfer"; failDt] ]

    let private printOkStepStatsTable (stepStats: StepStats[]) =
        stepStats
        |> Seq.mapi createOkStepStatsRow
        |> Seq.concat
        |> List.ofSeq
        |> Console.addTable ["step"; "ok stats"]

    let private printFailStepStatsTable (stepStats: StepStats[]) =
        stepStats
        |> Seq.filter(fun stats -> stats.Fail.Request.Count > 0)
        |> Seq.mapi createFailStepStatsRow
        |> Seq.concat
        |> List.ofSeq
        |> Console.addTable ["step"; "fail stats"]

    let private printScenarioStatusCodes (scnStats: ScenarioStats) =
        if scnStats.StatusCodes.Length > 0 then
            [ Console.addLine(String.Empty)
              ConsoleStatusCodesStats.printScenarioHeader(scnStats.ScenarioName)
              ConsoleStatusCodesStats.printStatusCodeTable(scnStats) ]
        else List.Empty

    let private errorStepStatsExist (stepStats: StepStats[]) =
        stepStats |> Seq.exists(fun stats -> stats.Fail.Request.Count > 0)

    let private printScenarioStats (scnStats: ScenarioStats) (simulations: LoadSimulation list) =
        [ yield! printScenarioHeader(scnStats)
          yield! printLoadSimulations(simulations)
          printOkStepStatsTable(scnStats.StepStats)
          if errorStepStatsExist(scnStats.StepStats) then
              printFailStepStatsTable(scnStats.StepStats)
          yield! printScenarioStatusCodes(scnStats) ]

    let printNodeStats (stats: NodeStats) (loadSimulations: IDictionary<string, LoadSimulation list>) =
        let scenarioStats =
            stats.ScenarioStats
            |> Seq.map(fun scnStats ->
                [ yield! printScenarioStats scnStats loadSimulations.[scnStats.ScenarioName]
                  Console.addLine(String.Empty) ]
            )
            |> Seq.concat
            |> List.ofSeq

        [ Console.addHeader("scenario stats")
          Console.addLine(String.Empty)
          yield! scenarioStats ]

module ConsolePluginStats =

    let private printPluginStatsHeader (table: DataTable) =
        Console.addLine $"plugin stats: {table.TableName |> Console.highlightPrimary}"

    let private createPluginStatsRow (columns: DataColumn[]) (row: DataRow) =
        columns
        |> Seq.map(fun col ->
            [ col.GetColumnCaptionOrName() |> Console.escapeMarkup
              row.[col].ToString() |> Console.escapeMarkup ]
        )

    let private createPluginStatsTable (table: DataTable)=
        let headers = ["key"; "value"]
        let createPluginStatsRow = table.GetColumns() |> createPluginStatsRow
        let rows =
            table.GetRows()
            |> Seq.mapi(fun i row ->
                [ if i > 0 then [String.Empty; String.Empty]
                  yield! createPluginStatsRow row ]
            )
            |> Seq.concat
            |> Seq.toList

        Console.addTable headers rows

    let printPluginStats (stats: NodeStats) =
        if stats.PluginStats.Length > 0 then
            let pluginStats =
                stats.PluginStats
                |> Seq.collect(fun dataSet -> dataSet.GetTables())
                |> Seq.map(fun table ->
                    [ printPluginStatsHeader(table)
                      Console.addLine(String.Empty)
                      createPluginStatsTable(table)
                      Console.addLine(String.Empty) ]
                )
                |> Seq.concat
                |> List.ofSeq

            [ Console.addHeader("plugin stats")
              Console.addLine(String.Empty)
              yield! pluginStats ]
        else
            List.empty

module ConsoleHints =

    let private createHintsList (hints: HintResult[]) =
        hints
        |> Seq.map(fun hint ->
            seq {
                $"hint for {hint.SourceType} '{hint.SourceName |> Console.escapeMarkup |> Console.highlightPrimary}':"
                $"{hint.Hint |> Console.escapeMarkup |> Console.highlightWarning}"
            }
        )
        |> Console.addList

    let printHints (hints: HintResult[]) =
        if hints.Length > 0 then
            [ Console.addHeader("hints")
              Console.addLine(String.Empty)
              yield! createHintsList(hints) ]
        else
            List.Empty

let print (logger: ILogger) (sessionResult: NodeSessionResult) (simulations: IDictionary<string, LoadSimulation list>) =
    try
        logger.Verbose("ConsoleReport.print")

        [ Console.addLine(String.Empty)
          yield! ConsoleTestInfo.printTestInfo sessionResult.FinalStats.TestInfo
          yield! ConsoleNodeStats.printNodeStats sessionResult.FinalStats simulations
          yield! ConsolePluginStats.printPluginStats sessionResult.FinalStats
          yield! ConsoleHints.printHints sessionResult.Hints
          Console.addLine(String.Empty) ]
    with
    | ex ->
        logger.Error(ex, "ConsoleReport.print failed")
        [ "Could not generate report" |> Console.addLine ]
