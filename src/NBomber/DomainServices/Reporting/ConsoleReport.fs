module internal NBomber.DomainServices.Reporting.ConsoleReport

open System
open System.Collections.Generic
open System.Data

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.HintsAnalyzer
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

    let private createTableRows (stats: StatusCodeStats[])=
        stats
        |> Seq.map(fun x ->
            [ x.StatusCode.ToString()
              x.Count.ToString()
              x.Message |> Console.escapeMarkup |> Console.highlightDanger ]
        )

    let print (stats: StatusCodeStats[])  =
        let headers = ["status code"; "count"; "message"]
        let rows = stats |> createTableRows |> Seq.toList
        Console.addTable headers rows

module ConsoleNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) =
        [ Console.addLine($"scenario: '{scnStats.ScenarioName |> Console.escapeMarkup |> Console.highlightPrimary}'")
          Console.addLine($"duration: '{scnStats.Duration |> Console.highlightPrimary}'" +
                          $", ok count: '{scnStats.OkCount |> Console.highlightPrimary}'" +
                          $", fail count: '{scnStats.FailCount |> Console.highlightDanger}'" +
                          $", all data: '{scnStats.AllDataMB |> Console.highlightPrimary}' MB") ]

    let private printLoadSimulation (simulation: LoadSimulation) =
        let simulationName = LoadTimeLine.getSimulationName(simulation)

        match simulation with
        | RampConstant (copies, during)     ->
            $"load simulation: '{simulationName |> Console.highlightPrimary}'" +
            $", copies: '{copies |> Console.highlightPrimary}'" +
            $", during: '{during |> Console.highlightPrimary}'"

        | KeepConstant (copies, during)     ->
            $"load simulation: '{simulationName |> Console.highlightPrimary}'" +
            $", copies: '{copies |> Console.highlightPrimary}'" +
            $", during: '{during |> Console.highlightPrimary}'"

        | RampPerSec (rate, during)         ->
            $"load simulation: '{simulationName |> Console.highlightPrimary}'" +
            $", rate: '{rate |> Console.highlightPrimary}'" +
            $", during: '{during |> Console.highlightPrimary}'"

        | InjectPerSec (rate, during)       ->
            $"load simulation: '{simulationName |> Console.highlightPrimary}'" +
            $", rate: '{rate |> Console.highlightPrimary}'" +
            $", during: '{during |> Console.highlightPrimary}'"

        | InjectPerSecRandom (minRate, maxRate, during) ->
            $"load simulation: '{simulationName |> Console.highlightPrimary}'" +
            $", min rate: '{minRate |> Console.highlightPrimary}'" +
            $", max rate: '{maxRate |> Console.highlightPrimary}'" +
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
        let okDtMin = $"%.3f{okDataTransfer.MinKb}"
        let okDtMean = $"%.3f{okDataTransfer.MeanKb}"
        let okDtMax = $"%.3f{okDataTransfer.MaxKb}"
        let okDtAll = $"%.3f{okDataTransfer.AllMB}"

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
            $"min = {okDtMin |> Console.highlightSuccess} KB" +
            $", mean = {okDtMean |> Console.highlightSuccess} KB" +
            $", max = {okDtMax |> Console.highlightSuccess} KB" +
            $", all = {okDtAll |> Console.highlightSuccess} MB"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name |> Console.highlightSecondary]
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
            $"min = {failDtMin |> Console.highlightDanger} KB" +
            $", mean = {failDtMean |> Console.highlightDanger} KB" +
            $", max = {failDtMax |> Console.highlightDanger} KB" +
            $", all = {failDtAll |> Console.highlightDanger} MB"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name |> Console.highlightSecondary]
          ["request count"; reqCount]
          ["latency"; failLatencies]
          ["latency percentile"; failPercentile]
          if failDataTransfer.AllMB > 0.0 then ["data transfer"; failDt] ]

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
              ConsoleStatusCodesStats.print(scnStats.StatusCodes) ]
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

    let private createHintsList (hints: HintResult list) =
        hints
        |> Seq.map(fun hint ->
            seq {
                $"hint for {hint.SourceType} '{hint.SourceName |> Console.escapeMarkup |> Console.highlightPrimary}':"
                $"{hint.Hint |> Console.escapeMarkup |> Console.highlightWarning}"
            }
        )
        |> Console.addList

    let printHints (hints: HintResult list) =
        if hints.Length > 0 then
            [ Console.addHeader("hints")
              Console.addLine(String.Empty)
              yield! createHintsList(hints) ]
        else
            List.Empty

let print (stats: NodeStats) (hints: HintResult list) (simulations: IDictionary<string, LoadSimulation list>) =
    [ Console.addLine(String.Empty)
      yield! ConsoleTestInfo.printTestInfo stats.TestInfo
      yield! ConsoleNodeStats.printNodeStats stats simulations
      yield! ConsolePluginStats.printPluginStats stats
      yield! ConsoleHints.printHints hints
      Console.addLine(String.Empty) ]
