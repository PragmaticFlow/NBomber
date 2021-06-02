module internal NBomber.DomainServices.Reporting.MdReport

open System
open System.Collections.Generic
open System.Data

open FuncyDown.Document
open Serilog

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain
open NBomber.Domain.Stats
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

module MdStatusCodeStats =

    let printScenarioHeader (scenarioName: string) (document: Document) =
        document
        |> Md.printHeader $"status codes for scenario: {scenarioName |> Md.printInlineCode}"

    let private createTableRows (scnStats: ScenarioStats) =
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
        allStatusCodes

    let printStatusCodeTable (scnStats: ScenarioStats) (document: Document) =
        let headers = ["status code"; "count"; "message"]
        let rows = createTableRows(scnStats)
        document |> addTable headers rows

module MdNodeStats =

    let private printDataKb (bytes: int) =
        $"{bytes |> Statistics.Converter.fromBytesToKb |> Md.printInlineCode} KB"

    let private printAllData (bytes: int64) =
        $"{bytes |> Statistics.Converter.fromBytesToMb |> Md.printInlineCode} MB"

    let private printScenarioHeader (scnStats: ScenarioStats) (document: Document) =
        let header =
            $"scenario: {scnStats.ScenarioName |> Md.printInlineCode}" +
            $", duration: {scnStats.Duration |> Md.printInlineCode}" +
            $", ok count: {scnStats.OkCount |> Md.printInlineCode}" +
            $", fail count: {scnStats.FailCount |> Md.printInlineCode}" +
            $", all data: {scnStats.AllBytes |> printAllData} MB"

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
            $"min = {okDataTransfer.MinBytes |> printDataKb}" +
            $", mean = {okDataTransfer.MeanBytes |> printDataKb}" +
            $", max = {okDataTransfer.MaxBytes |> printDataKb}" +
            $", all = {okDataTransfer.AllBytes |> printAllData}"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name |> Md.printInlineCode]
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
            $"min = {failDataTransfer.MinBytes |> printDataKb}" +
            $", mean = {failDataTransfer.MeanBytes |> printDataKb}" +
            $", max = {failDataTransfer.MaxBytes |> printDataKb}" +
            $", all = {failDataTransfer.AllBytes |> printAllData}"

        [ if i > 0 then [String.Empty; String.Empty]
          ["name"; name |> Md.printInlineCode]
          ["request count"; reqCount]
          ["latency"; failLatencies]
          ["latency percentile"; failPercentile]
          if failDataTransfer.AllBytes > 0L then ["data transfer"; failDt] ]

    let private printOkStepStatsTable (stepStats: StepStats[]) (document: Document) =
        stepStats
        |> Seq.mapi createOkStepStatsRow
        |> Seq.concat
        |> List.ofSeq
        |> fun rows -> document |> addTable ["step"; "ok stats"] rows

    let private failStepStatsExist (stepStats: StepStats[]) =
        stepStats |> Seq.exists(fun stats -> stats.Fail.Request.Count > 0)

    let private printFailStepStatsTable (stepStats: StepStats[]) (document: Document) =
        if failStepStatsExist(stepStats) then
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

    let private printScenarioStatusCodeStats (scnStats: ScenarioStats) (document: Document) =
        if scnStats.StatusCodes.Length > 0 then
            document
            |> MdStatusCodeStats.printScenarioHeader scnStats.ScenarioName
            |> MdStatusCodeStats.printStatusCodeTable scnStats
        else document

    let private printScenarioStats (scnStats: ScenarioStats) (simulations: LoadSimulation list) (document: Document) =
        document
        |> printScenarioHeader scnStats
        |> printLoadSimulations simulations
        |> printOkStepStatsTable scnStats.StepStats
        |> printFailStepStatsTable scnStats.StepStats
        |> printScenarioStatusCodeStats scnStats

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

    let private createHintsTableRows (hints: HintResult[]) =
        hints
        |> Seq.map(fun hint -> [hint.SourceType.ToString(); hint.SourceName; hint.Hint])
        |> List.ofSeq

    let printHints (hints: HintResult[]) (document: Document) =
        if hints.Length > 0 then
            let headers = ["source"; "name"; "hint"]
            let rows = createHintsTableRows(hints)

            document
            |> printHintsHeader
            |> addTable headers rows
        else
            document

let print (logger: ILogger) (sessionResult: NodeSessionResult) (simulations: IDictionary<string, LoadSimulation list>) =
    try
        logger.Verbose("MdReport.print")

        emptyDocument
        |> MdTestInfo.printTestInfo sessionResult.NodeStats.TestInfo
        |> MdNodeStats.printNodeStats sessionResult.NodeStats simulations
        |> MdPluginStats.printPluginStats sessionResult.NodeStats
        |> MdHints.printHints sessionResult.Hints
        |> asString
    with
    | ex ->
        logger.Error(ex, "MdReport.print failed")
        "Could not generate report"
