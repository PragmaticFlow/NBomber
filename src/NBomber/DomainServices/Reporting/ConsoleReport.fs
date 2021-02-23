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
          Console.addLine($"test suite: '{testInfo.TestSuite |> Console.escapeMarkup |> Console.highlight}'")
          Console.addLine($"test name: '{testInfo.TestName |> Console.escapeMarkup |> Console.highlight}'")
          Console.addLine(String.Empty) ]

module ConsoleErrorStats =

    let printErrorStatsHeader (scenarioName: string)  =
        Console.addLine($"errors for scenario: {scenarioName |> Console.highlight}")

    let private createErrorStatsTableRows (errorStats: ErrorStats[])=
        errorStats
        |> Seq.map(fun error ->
            [ error.ErrorCode.ToString()
              error.Count.ToString()
              error.Message |> Console.escapeMarkup |> Console.highlightError ]
        )
        |> List.ofSeq

    let printErrorStats (errorStats: ErrorStats[])  =
        let headers = ["code"; "count"; "message"]
        let rows = createErrorStatsTableRows(errorStats)

        Console.addTable headers rows

module ConsoleNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) =
        [ Console.addLine($"scenario: '{scnStats.ScenarioName |> Console.escapeMarkup |> Console.highlight}'")
          Console.addLine($"duration: '{scnStats.Duration |> Console.highlight}'" +
                          $", ok count: '{scnStats.OkCount |> Console.highlight}'" +
                          $", fail count: '{scnStats.FailCount |> Console.highlightError}'" +
                          $", all data: '{scnStats.AllDataMB |> Console.highlight}' MB") ]

    let private printLoadSimulation (simulation: LoadSimulation) =
        let simulationName = LoadTimeLine.getSimulationName(simulation)

        match simulation with
        | RampConstant (copies, during)     ->
            $"load simulation: '{simulationName |> Console.highlight}'" +
            $", copies: '{copies |> Console.highlight}'" +
            $", during: '{during |> Console.highlight}'"

        | KeepConstant  (copies, during)    ->
            $"load simulation: '{simulationName |> Console.highlight}'" +
            $", copies: '{copies |> Console.highlight}'" +
            $", during: '{during |> Console.highlight}'"

        | RampPerSec (rate, during)         ->
            $"load simulation: '{simulationName |> Console.highlight}'" +
            $", rate: '{rate |> Console.highlight}'" +
            $", during: '{during |> Console.highlight}'"

        | InjectPerSec (rate, during)       ->
            $"load simulation: '{simulationName |> Console.highlight}'" +
            $", rate: '{rate |> Console.highlight}'" +
            $", during: '{during |> Console.highlight}'"

        | InjectPerSecRandom  (minRate, maxRate, during) ->
            $"load simulation: '{simulationName |> Console.highlight}'" +
            $", min rate: '{minRate |> Console.highlight}'" +
            $", max rate: '{maxRate |> Console.highlight}'" +
            $", during: '{during |> Console.highlight}'"

        |> Console.addLine

    let private printLoadSimulations (simulations: LoadSimulation list) =
        simulations |> Seq.map printLoadSimulation

    let private createStepStatsRow (i) (s: StepStats) =
        let name = s.StepName |> Console.highlight
        let okReqCount = s.Ok.Request.Count
        let failReqCount = s.Fail.Request.Count
        let allReqCount = okReqCount + failReqCount
        let okRPS = s.Ok.Request.RPS
        let failRPS = s.Fail.Request.RPS
        let okLatency = s.Ok.Latency
        let failLatency = s.Fail.Latency
        let okDataTransfer = s.Ok.DataTransfer
        let failDataTransfer = s.Fail.DataTransfer

        let allCount = $"all = {allReqCount |> Console.highlight}"

        let okCount =
            $"ok = {okReqCount |> Console.highlight}" +
            $", RPS = {okRPS |> Console.highlight}"

        let failCount =
            $"fail = {failReqCount |> Console.highlightError}" +
            $", RPS = {failRPS |> Console.highlightError}"

        let okTimes =
            $"min = {okLatency.MinMs |> Console.highlight}" +
            $", mean = {okLatency.MeanMs |> Console.highlight}" +
            $", max = {okLatency.MaxMs |> Console.highlight}"

        let failTimes =
            $"min = {failLatency.MinMs |> Console.highlightError}" +
            $", mean = {failLatency.MeanMs |> Console.highlightError}" +
            $", max = {failLatency.MaxMs |> Console.highlightError}"

        let okPercentile =
            $"50%% = {okLatency.Percent50 |> Console.highlight}" +
            $", 75%% = {okLatency.Percent75 |> Console.highlight}" +
            $", 95%% = {okLatency.Percent95 |> Console.highlight}" +
            $", 99%% = {okLatency.Percent99 |> Console.highlight}" +
            $", StdDev = {okLatency.StdDev |> Console.highlight}"

        let failPercentile =
            $"50%% = {failLatency.Percent50 |> Console.highlightError}" +
            $", 75%% = {failLatency.Percent75 |> Console.highlightError}" +
            $", 95%% = {failLatency.Percent95 |> Console.highlightError}" +
            $", 99%% = {failLatency.Percent99 |> Console.highlightError}" +
            $", StdDev = {failLatency.StdDev |> Console.highlightError}"

        let okDtMin = $"%.3f{okDataTransfer.MinKb} KB" |> Console.highlight
        let okDtMean = $"%.3f{okDataTransfer.MeanKb} KB" |> Console.highlight
        let okDtMax = $"%.3f{okDataTransfer.MaxKb} KB" |> Console.highlight
        let okDtAll = $"%.3f{okDataTransfer.AllMB} MB" |> Console.highlight

        let okDt =
            $"min = {okDtMin |> Console.highlight}" +
            $", mean = {okDtMean |> Console.highlight}" +
            $", max = {okDtMax |> Console.highlight}" +
            $", all = {okDtAll |> Console.highlight}"

        let failDtMin = $"%.3f{failDataTransfer.MinKb} KB" |> Console.highlightError
        let failDtMean = $"%.3f{failDataTransfer.MeanKb} KB" |> Console.highlightError
        let failDtMax = $"%.3f{failDataTransfer.MaxKb} KB" |> Console.highlightError
        let failDtAll = $"%.3f{failDataTransfer.AllMB} MB" |> Console.highlightError

        let failDt =
            $"min = {failDtMin |> Console.highlightError}" +
            $", mean = {failDtMean |> Console.highlightError}" +
            $", max = {failDtMax |> Console.highlightError}" +
            $", all = {failDtAll |> Console.highlightError}"

        [ if i > 0 then [" "; " "]
          ["name"; name]
          ["all request count"; allCount]
          ["ok stats"; String.Empty]
          ["ok request count"; okCount]
          ["ok latency"; okTimes]
          ["ok latency percentile"; okPercentile]
          if okDataTransfer.AllMB > 0.0 then ["ok data transfer"; okDt]
          ["fail stats"; String.Empty]
          ["fail request count"; failCount]
          ["fail latency"; failTimes]
          ["fail latency percentile"; failPercentile]
          if failDataTransfer.AllMB > 0.0 then ["fail data transfer"; failDt] ]

    let private createStepStatsTableRows (stepStats: StepStats[]) =
        stepStats
        |> Seq.mapi createStepStatsRow
        |> Seq.concat
        |> List.ofSeq

    let private printScenarioErrorStats (scnStats: ScenarioStats) =
        if scnStats.ErrorStats.Length > 0 then
            [ Console.addLine(String.Empty)
              ConsoleErrorStats.printErrorStatsHeader(scnStats.ScenarioName)
              ConsoleErrorStats.printErrorStats(scnStats.ErrorStats) ]
        else List.Empty

    let private printScenarioStats (scnStats: ScenarioStats) (simulations: LoadSimulation list) =
        let headers = ["step"; "details"]
        let rows = createStepStatsTableRows(scnStats.StepStats)

        [ yield! printScenarioHeader(scnStats)
          yield! printLoadSimulations(simulations)
          Console.addTable headers rows
          yield! printScenarioErrorStats(scnStats) ]

    let printNodeStats (stats: NodeStats) (loadSimulations: IDictionary<string, LoadSimulation list>)=
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
        Console.addLine $"plugin stats: {table.TableName |> Console.highlight}"

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
                $"hint for {hint.SourceType} '{hint.SourceName |> Console.escapeMarkup |> Console.highlight}':"
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
