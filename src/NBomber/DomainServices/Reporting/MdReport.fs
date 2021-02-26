module internal NBomber.DomainServices.Reporting.MdReport

open System
open System.Collections.Generic
open System.Data

open FuncyDown.Document

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.HintsAnalyzer
open NBomber.Extensions

module Md =

    let printInlineCode (code: obj) =
       emptyDocument
       |> addInlineCode { Code = code.ToString(); Language = None }
       |> asString

    let printHeader (header) (document: Document) =
        document
        |> addBlockQuote header
        |> addNewline
        |> addNewline

    let printBold (text) =
        emptyDocument
        |> addStrongEmphasis text
        |> asString

module MdTestInfo =

    let printTestInfo (testInfo: TestInfo) (document: Document) =
        document
        |> Md.printHeader $"test suite: {testInfo.TestSuite |> Md.printInlineCode}"
        |> Md.printHeader $"test name: {testInfo.TestName |> Md.printInlineCode}"

module MdErrorStats =

    let printErrorStatsHeader (scenarioName: string) (document: Document) =
        document
        |> Md.printHeader $"errors for scenario: {scenarioName |> Md.printInlineCode}"

    let private createErrorStatsTableRows (errorStats: ErrorStats[])=
        errorStats
        |> Seq.map(fun error -> [error.ErrorCode.ToString(); error.Count.ToString(); error.Message])
        |> List.ofSeq

    let printErrorStats (errorStats: ErrorStats[]) (document: Document) =
        let headers = ["error code"; "count"; "message"]
        let rows = createErrorStatsTableRows(errorStats)

        document |> addTable headers rows

module MdNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) (document: Document) =
        let header =
            $"scenario: {scnStats.ScenarioName |> Md.printInlineCode}" +
            $", duration: {scnStats.Duration |> Md.printInlineCode}" +
            $", ok count: {scnStats.OkCount |> Md.printInlineCode}" +
            $", fail count: {scnStats.FailCount |> Md.printInlineCode}" +
            $", all data: {scnStats.AllDataMB |> Md.printInlineCode} MB"

        document |> Md.printHeader header

    let private printLoadSimulation (simulation: LoadSimulation) (document: Document) =
        let simulationName = LoadTimeLine.getSimulationName(simulation)
        let loadSimulation =
            match simulation with
            | RampConstant (copies, during)     ->
                $"load simulation: {simulationName |> Md.printInlineCode}" +
                $", copies: {copies |> Md.printInlineCode}" +
                $", during: {during |> Md.printInlineCode}"

            | KeepConstant (copies, during)     ->
                $"load simulation: {simulationName |> Md.printInlineCode}" +
                $", copies: {copies |> Md.printInlineCode}" +
                $", during: {during |> Md.printInlineCode}"

            | RampPerSec (rate, during)         ->
                $"load simulation: {simulationName |> Md.printInlineCode}" +
                $", rate: {rate |> Md.printInlineCode}" +
                $", during: {during |> Md.printInlineCode}"

            | InjectPerSec (rate, during)       ->
                $"load simulation: {simulationName |> Md.printInlineCode}" +
                $", rate: {rate |> Md.printInlineCode}" +
                $", during: {during |> Md.printInlineCode}"

            | InjectPerSecRandom (minRate, maxRate, during) ->
                $"load simulation: {simulationName |> Md.printInlineCode}" +
                $", min rate: {minRate |> Md.printInlineCode}" +
                $", max rate: {maxRate |> Md.printInlineCode}" +
                $", during: {during |> Md.printInlineCode}"

        document |> addText loadSimulation

    let private printLoadSimulations (simulations: LoadSimulation list) (document: Document) =
        simulations
        |> Seq.fold(fun document simulation ->
            document |> printLoadSimulation simulation |> addNewline
        ) document

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
            $"all = {allReqCount |> Md.printInlineCode}" +
            $", ok = {okReqCount |> Md.printInlineCode}" +
            $", RPS = {okRPS |> Md.printInlineCode}"

        let okLatencies =
            $"min = {okLatency.MinMs |> Md.printInlineCode}" +
            $", mean = {okLatency.MeanMs |> Md.printInlineCode}" +
            $", max = {okLatency.MaxMs |> Md.printInlineCode}" +
            $", StdDev = {okLatency.StdDev |> Md.printInlineCode}"

        let okPercentile =
            $"50%% = {okLatency.Percent50 |> Md.printInlineCode}" +
            $", 75%% = {okLatency.Percent75 |> Md.printInlineCode}" +
            $", 95%% = {okLatency.Percent95 |> Md.printInlineCode}" +
            $", 99%% = {okLatency.Percent99 |> Md.printInlineCode}"

        let okDt =
            $"min = {okDtMin |> Md.printInlineCode} KB" +
            $", mean = {okDtMean |> Md.printInlineCode} KB" +
            $", max = {okDtMax |> Md.printInlineCode} KB" +
            $", all = {okDtAll |> Md.printInlineCode} MB"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name |> Md.printInlineCode]
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
            $"all = {allReqCount |> Md.printInlineCode}" +
            $", fail = {failReqCount |> Md.printInlineCode}" +
            $", RPS = {failRPS |> Md.printInlineCode}"

        let failLatencies =
            $"min = {failLatency.MinMs |> Md.printInlineCode}" +
            $", mean = {failLatency.MeanMs |> Md.printInlineCode}" +
            $", max = {failLatency.MaxMs |> Md.printInlineCode}" +
            $", StdDev = {failLatency.StdDev |> Md.printInlineCode}"

        let failPercentile =
            $"50%% = {failLatency.Percent50 |> Md.printInlineCode}" +
            $", 75%% = {failLatency.Percent75 |> Md.printInlineCode}" +
            $", 95%% = {failLatency.Percent95 |> Md.printInlineCode}" +
            $", 99%% = {failLatency.Percent99 |> Md.printInlineCode}"

        let failDt =
            $"min = {failDtMin |> Md.printInlineCode} KB" +
            $", mean = {failDtMean |> Md.printInlineCode} KB" +
            $", max = {failDtMax |> Md.printInlineCode} KB" +
            $", all = {failDtAll |> Md.printInlineCode} MB"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name |> Md.printInlineCode]
          ["request count"; reqCount]
          ["latency"; failLatencies]
          ["latency percentile"; failPercentile]
          if failDataTransfer.AllMB > 0.0 then ["data transfer"; failDt] ]

    let private printOkStepStatsTable (stepStats: StepStats[]) (document: Document) =
        stepStats
        |> Seq.mapi createOkStepStatsRow
        |> Seq.concat
        |> List.ofSeq
        |> fun rows -> document |> addTable ["step"; "ok stats"] rows

    let private errorStepStatsExist (stepStats: StepStats[]) =
        stepStats |> Seq.exists(fun stats -> stats.Fail.Request.Count > 0)

    let private printFailStepStatsTable (stepStats: StepStats[]) (document: Document) =
        if errorStepStatsExist(stepStats) then
            stepStats
            |> Seq.filter(fun stats -> stats.Fail.Request.Count > 0)
            |> Seq.mapi createFailStepStatsRow
            |> Seq.concat
            |> List.ofSeq
            |> fun rows ->
                document
                |> addNewline
                |> addTable ["step"; "fail stats"] rows
        else
            document

    let private printScenarioErrorStats (scnStats: ScenarioStats) (document: Document) =
        if scnStats.ErrorStats.Length > 0 then
            document
            |> MdErrorStats.printErrorStatsHeader scnStats.ScenarioName
            |> MdErrorStats.printErrorStats scnStats.ErrorStats
        else document

    let private printScenarioStats (scnStats: ScenarioStats) (simulations: LoadSimulation list) (document: Document) =
        document
        |> printScenarioHeader scnStats
        |> printLoadSimulations simulations
        |> printOkStepStatsTable scnStats.StepStats
        |> printFailStepStatsTable scnStats.StepStats
        |> printScenarioErrorStats scnStats

    let printNodeStats (stats: NodeStats) (loadSimulations: IDictionary<string, LoadSimulation list>) (document: Document) =
        stats.ScenarioStats
        |> Seq.fold(fun document scnStats ->
            document |> printScenarioStats scnStats loadSimulations.[scnStats.ScenarioName] |> addNewline
        ) document

module MdPluginStats =

    let private printPluginStatsHeader (table: DataTable) (document: Document) =
        document
        |> Md.printHeader $"plugin stats: {table.TableName |> Md.printInlineCode}"

    let private createPluginStatsTableHeaders (table: DataTable) =
        table.GetColumns()
        |> Seq.map(fun col -> col.GetColumnCaptionOrName())
        |> Seq.toList

    let private createPluginStatsTableRows (table: DataTable) =
        let columns = table.GetColumns()

        table.GetRows()
        |> Seq.map(fun row -> columns |> Seq.map(fun col -> row.[col] |> string) |> List.ofSeq)
        |> List.ofSeq

    let private printPluginStatsTable (table: DataTable) (document: Document) =
        let headers = createPluginStatsTableHeaders(table)
        let rows = createPluginStatsTableRows(table)

        document
        |> printPluginStatsHeader table
        |> addTable headers rows

    let printPluginStats (stats: NodeStats) (document: Document) =
        stats.PluginStats
        |> Seq.collect(fun dataSet -> dataSet.GetTables())
        |> Seq.fold(fun document table ->
            document |> printPluginStatsTable table |> addNewline
        ) document

module MdHints =

    let private printHintsHeader (document: Document) =
        document
        |> Md.printHeader "hints:"

    let private createHintsTableRows (hints: HintResult list) =
        hints
        |> Seq.map(fun hint -> [hint.SourceType.ToString(); hint.SourceName; hint.Hint])
        |> List.ofSeq

    let printHints (hints: HintResult list) (document: Document) =
        if hints.Length > 0 then
            let headers = ["source"; "name"; "hint"]
            let rows = createHintsTableRows(hints)

            document
            |> printHintsHeader
            |> addTable headers rows
        else
            document

let print (stats: NodeStats) (hints: HintResult list) (simulations: IDictionary<string, LoadSimulation list>) =
    emptyDocument
    |> MdTestInfo.printTestInfo stats.TestInfo
    |> MdNodeStats.printNodeStats stats simulations
    |> MdPluginStats.printPluginStats stats
    |> MdHints.printHints hints
    |> asString
