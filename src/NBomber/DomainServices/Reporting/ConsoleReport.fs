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
        [ AnsiConsole.addHeader("test info")
          AnsiConsole.addEmptyLine()
          AnsiConsole.addLine($"test suite: '{testInfo.TestSuite |> AnsiConsole.escapeMarkup |> AnsiConsole.highlight}'")
          AnsiConsole.addLine($"test name: '{testInfo.TestName |> AnsiConsole.escapeMarkup |> AnsiConsole.highlight}'")
          AnsiConsole.addEmptyLine() ]

module ConsoleErrorStats =

    let printErrorStatsHeader (scenarioName: string)  =
        AnsiConsole.addLine($"errors for scenario: {scenarioName |> AnsiConsole.highlight}")

    let private createErrorStatsTableRows (errorStats: ErrorStats[])=
        errorStats
        |> Seq.map(fun error ->
            [ error.ErrorCode.ToString()
              error.Count.ToString()
              error.Message |> AnsiConsole.escapeMarkup |> AnsiConsole.highlightError ]
        )
        |> List.ofSeq

    let printErrorStats (errorStats: ErrorStats[])  =
        let headers = ["code"; "count"; "message"]
        let rows = createErrorStatsTableRows(errorStats)

        AnsiConsole.addTable headers rows

module ConsoleNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) =
        [ AnsiConsole.addLine($"scenario: '{scnStats.ScenarioName |> AnsiConsole.escapeMarkup |> AnsiConsole.highlight}'")
          AnsiConsole.addLine($"duration: '{scnStats.Duration |> AnsiConsole.highlight}'" +
                              $", ok count: '{scnStats.OkCount |> AnsiConsole.highlight}'" +
                              $", fail count: '{scnStats.FailCount |> AnsiConsole.highlight}'" +
                              $", all data: '{scnStats.AllDataMB |> AnsiConsole.highlight}' MB") ]

    let private printLoadSimulation (simulation: LoadSimulation) =
        let simulationName = LoadTimeLine.getSimulationName(simulation)

        match simulation with
        | RampConstant (copies, during)     ->
            $"load simulation: '{simulationName |> AnsiConsole.highlight}'" +
            $", copies: '{copies |> AnsiConsole.highlight}'" +
            $", during: '{during |> AnsiConsole.highlight}'"

        | KeepConstant  (copies, during)    ->
            $"load simulation: '{simulationName |> AnsiConsole.highlight}'" +
            $", copies: '{copies |> AnsiConsole.highlight}'" +
            $", during: '{during |> AnsiConsole.highlight}'"

        | RampPerSec (rate, during)         ->
            $"load simulation: '{simulationName |> AnsiConsole.highlight}'" +
            $", rate: '{rate |> AnsiConsole.highlight}'" +
            $", during: '{during |> AnsiConsole.highlight}'"

        | InjectPerSec (rate, during)       ->
            $"load simulation: '{simulationName |> AnsiConsole.highlight}'" +
            $", rate: '{rate |> AnsiConsole.highlight}'" +
            $", during: '{during |> AnsiConsole.highlight}'"

        | InjectPerSecRandom  (minRate, maxRate, during) ->
            $"load simulation: '{simulationName |> AnsiConsole.highlight}'" +
            $", min rate: '{minRate |> AnsiConsole.highlight}'" +
            $", max rate: '{maxRate |> AnsiConsole.highlight}'" +
            $", during: '{during |> AnsiConsole.highlight}'"

        |> AnsiConsole.addLine

    let private printloadSimulations (simulations: LoadSimulation list) =
        simulations |> Seq.map printLoadSimulation

    let private createStepStatsRow (i) (s: StepStats) =
        let name = s.StepName |> AnsiConsole.highlight
        let okCount = s.Ok.Request.Count
        let failCount = s.Fail.Request.Count
        let reqCount = okCount + failCount
        let okRPS = s.Ok.Request.RPS
        let lt = s.Ok.Latency
        let dt = s.Ok.DataTransfer

        let count =
            $"all = {reqCount |> AnsiConsole.highlight}" +
            $", ok = {okCount |> AnsiConsole.highlight}" +
            $", failed = {failCount |> AnsiConsole.highlight}" +
            $", RPS = {okRPS |> AnsiConsole.highlight}"

        let times =
            $"min = {lt.MinMs |> AnsiConsole.highlight}" +
            $", mean = {lt.MeanMs |> AnsiConsole.highlight}" +
            $", max = {lt.MaxMs |> AnsiConsole.highlight}"

        let percentile =
            $"50%% = {lt.Percent50 |> AnsiConsole.highlight}" +
            $", 75%% = {lt.Percent75 |> AnsiConsole.highlight}" +
            $", 95%% = {lt.Percent95 |> AnsiConsole.highlight}" +
            $", 99%% = {lt.Percent99 |> AnsiConsole.highlight}" +
            $", StdDev = {lt.StdDev |> AnsiConsole.highlight}"

        let min = $"%.3f{dt.MinKb} KB" |> AnsiConsole.highlight
        let mean = $"%.3f{dt.MeanKb} KB" |> AnsiConsole.highlight
        let max = $"%.3f{dt.MaxKb} KB" |> AnsiConsole.highlight
        let all = $"%.3f{dt.AllMB} MB" |> AnsiConsole.highlight

        let dataTransfer =
            $"min = {min |> AnsiConsole.highlight}" +
            $", mean = {mean |> AnsiConsole.highlight}" +
            $", max = {max |> AnsiConsole.highlight}" +
            $", all = {all |> AnsiConsole.highlight}"

        [ ["name"; name]
          ["request count"; count]
          ["latency"; times]
          ["latency percentile"; percentile]
          if dt.AllMB > 0.0 then ["data transfer"; dataTransfer] ]

    let private createStepStatsTableRows (stepStats: StepStats[]) =
        stepStats
        |> Seq.mapi createStepStatsRow
        |> Seq.concat
        |> List.ofSeq

    let private printScenarioErrorStats (scnStats: ScenarioStats) =
        if scnStats.ErrorStats.Length > 0 then
            [ AnsiConsole.addEmptyLine()
              ConsoleErrorStats.printErrorStatsHeader(scnStats.ScenarioName)
              ConsoleErrorStats.printErrorStats(scnStats.ErrorStats) ]
        else List.Empty

    let private printScenarioStats (scnStats: ScenarioStats) (simulations: LoadSimulation list) =
        let headers = ["step"; "details"]
        let rows = createStepStatsTableRows(scnStats.StepStats)

        [ yield! printScenarioHeader(scnStats)
          yield! printloadSimulations(simulations)
          AnsiConsole.addTable headers rows
          yield! printScenarioErrorStats(scnStats) ]

    let printNodeStats (stats: NodeStats) (loadSimulations: IDictionary<string, LoadSimulation list>)=
        let scenarioStats =
            stats.ScenarioStats
            |> Seq.map(fun scnStats ->
                [ yield! printScenarioStats scnStats loadSimulations.[scnStats.ScenarioName]
                  AnsiConsole.addEmptyLine() ]
            )
            |> Seq.concat
            |> List.ofSeq

        [ AnsiConsole.addHeader("scenario stats")
          AnsiConsole.addEmptyLine()
          yield! scenarioStats ]

module ConsolePluginStats =

    let private printPluginStatsHeader (table: DataTable) =
        AnsiConsole.addLine $"plugin stats: {table.TableName |> AnsiConsole.highlight}"

    let private createPluginStatsRow (columns: DataColumn[]) (row: DataRow) =
        columns
        |> Seq.map(fun col ->
            [ col.GetColumnCaptionOrName() |> AnsiConsole.escapeMarkup
              row.[col].ToString() |> AnsiConsole.escapeMarkup ]
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

        AnsiConsole.addTable headers rows

    let printPluginStats (stats: NodeStats) =
        if stats.PluginStats.Length > 0 then
            let pluginStats =
                stats.PluginStats
                |> Seq.collect(fun dataSet -> dataSet.GetTables())
                |> Seq.map(fun table ->
                    [ printPluginStatsHeader(table)
                      AnsiConsole.addEmptyLine()
                      createPluginStatsTable(table)
                      AnsiConsole.addEmptyLine() ]
                )
                |> Seq.concat
                |> List.ofSeq

            [ AnsiConsole.addHeader("plugin stats")
              AnsiConsole.addEmptyLine()
              yield! pluginStats ]
        else
            List.empty

module ConsoleHints =

    let private createHintsList (hints: HintResult list) =
        hints
        |> Seq.map(fun hint ->
            seq {
                $"hint for {hint.SourceType} '{hint.SourceName |> AnsiConsole.escapeMarkup |> AnsiConsole.highlight}':"
                $"{hint.Hint |> AnsiConsole.escapeMarkup |> AnsiConsole.highlightWarning}"
            }
        )
        |> AnsiConsole.addList

    let printHints (hints: HintResult list) =
        if hints.Length > 0 then
            [ AnsiConsole.addHeader("hints")
              AnsiConsole.addEmptyLine()
              yield! createHintsList(hints) ]
        else
            List.Empty

let print (stats: NodeStats) (hints: HintResult list) (simulations: IDictionary<string, LoadSimulation list>) =
    [ AnsiConsole.addEmptyLine()
      yield! ConsoleTestInfo.printTestInfo stats.TestInfo
      yield! ConsoleNodeStats.printNodeStats stats simulations
      yield! ConsolePluginStats.printPluginStats stats
      yield! ConsoleHints.printHints hints
      AnsiConsole.addEmptyLine() ]
