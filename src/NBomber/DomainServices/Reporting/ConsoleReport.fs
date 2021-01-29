module internal NBomber.DomainServices.Reporting.ConsoleReport

open System
open System.Collections.Generic
open System.Data

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.HintsAnalyzer
open NBomber.DomainServices
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

module ConsoleTestInfo =

    let printTestInfo (testInfo: TestInfo) =
        [ Console.addHeader("test info")
          Console.addEmptyLine()
          Console.addLine($"test suite: '{testInfo.TestSuite |> Console.escapeMarkup |> Console.highlight}'")
          Console.addLine($"test name: '{testInfo.TestName |> Console.escapeMarkup |> Console.highlight}'")
          Console.addEmptyLine() ]

module ConsoleErrorStats =

    let printErrorStatsHeader (scenarioName: string)  =
        Console.addLine($"errors for scenario: {scenarioName |> Console.highlight}")

    let private createErrorStatsTableRows (errorStats: ErrorStats[])=
        errorStats
        |> Seq.map(fun error ->
            [ error.ErrorCode.ToString()
              error.Count.ToString()
              error.ShortMessage |> Console.escapeMarkup |> Console.highlightError ]
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
                          $", fail count: '{scnStats.FailCount |> Console.highlight}'" +
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

    let private printloadSimulations (simulations: LoadSimulation list) =
        simulations |> Seq.map printLoadSimulation

    let private createStepStatsRow (i) (s: StepStats) =
        let name = s.StepName |> Console.highlight

        let count =
            $"all = {s.RequestCount |> Console.highlight}" +
            $", ok = {s.OkCount |> Console.highlight}" +
            $", failed = {s.FailCount |> Console.highlight}" +
            $", RPS = {s.RPS |> Console.highlight}"

        let times =
            $"min = {s.Min |> Console.highlight}" +
            $", mean = {s.Mean |> Console.highlight}" +
            $", max = {s.Max |> Console.highlight}"

        let percentile =
            $"50%% = {s.Percent50 |> Console.highlight}" +
            $", 75%% = {s.Percent75 |> Console.highlight}" +
            $", 95%% = {s.Percent95 |> Console.highlight}" +
            $", 99%% = {s.Percent99 |> Console.highlight}" +
            $", StdDev = {s.StdDev |> Console.highlight}"

        let min = $"%.3f{s.MinDataKb} KB" |> Console.highlight
        let mean = $"%.3f{s.MeanDataKb} KB" |> Console.highlight
        let max = $"%.3f{s.MaxDataKb} KB" |> Console.highlight
        let all = $"%.3f{s.AllDataMB} MB" |> Console.highlight

        let dataTransfer =
            $"min = {min |> Console.highlight}" +
            $", mean = {mean |> Console.highlight}" +
            $", max = {max |> Console.highlight}" +
            $", all = {all |> Console.highlight}"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name]
          ["request count"; count]
          ["latency"; times]
          ["latency percentile"; percentile]
          if s.AllDataMB > 0.0 then ["data transfer"; dataTransfer] ]

    let private createStepStatsTableRows (stepStats: StepStats[]) =
        stepStats
        |> Seq.mapi createStepStatsRow
        |> Seq.concat
        |> List.ofSeq

    let private printScenarioErrorStats (scnStats: ScenarioStats) =
        if scnStats.ErrorStats.Length > 0 then
            [ Console.addEmptyLine()
              ConsoleErrorStats.printErrorStatsHeader(scnStats.ScenarioName)
              ConsoleErrorStats.printErrorStats(scnStats.ErrorStats) ]
        else List.Empty

    let private printScenarioStats (scnStats: ScenarioStats) (simulations: LoadSimulation list) =
        let headers = ["step"; "details"]
        let rows = createStepStatsTableRows(scnStats.StepStats)

        [ yield! printScenarioHeader(scnStats)
          yield! printloadSimulations(simulations)
          Console.addTable headers rows
          yield! printScenarioErrorStats(scnStats) ]

    let printNodeStats (stats: NodeStats) (loadSimulations: IDictionary<string, LoadSimulation list>)=
        let scenarioStats =
            stats.ScenarioStats
            |> Seq.map(fun scnStats ->
                [ yield! printScenarioStats scnStats loadSimulations.[scnStats.ScenarioName]
                  Console.addEmptyLine() ]
            )
            |> Seq.concat
            |> List.ofSeq

        [ Console.addHeader("scenario stats")
          Console.addEmptyLine()
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
                      Console.addEmptyLine()
                      createPluginStatsTable(table)
                      Console.addEmptyLine() ]
                )
                |> Seq.concat
                |> List.ofSeq

            [ Console.addHeader("plugin stats")
              Console.addEmptyLine()
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
              Console.addEmptyLine()
              yield! createHintsList(hints) ]
        else
            List.Empty

let print (stats: NodeStats) (hints: HintResult list) (simulations: IDictionary<string, LoadSimulation list>) =
    [ Console.addEmptyLine()
      yield! ConsoleTestInfo.printTestInfo stats.TestInfo
      yield! ConsoleNodeStats.printNodeStats stats simulations
      yield! ConsolePluginStats.printPluginStats stats
      yield! ConsoleHints.printHints hints
      Console.addEmptyLine() ]
