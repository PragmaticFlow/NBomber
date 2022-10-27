module internal NBomber.DomainServices.Reports.MdReport

open System.Collections.Generic
open System.Data

open FuncyDown
open Serilog

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain.Stats.Statistics
open NBomber.Extensions.Data

type Document = Document.Document

module Md =

    let addNewLine (document: Document) =
        document |> Document.addNewline |> Document.addNewline

    let printInlineCode (code: obj) =
       Document.emptyDocument
       |> Document.addInlineCode { Code = code.ToString(); Language = None }
       |> Document.asString

    let printHeader (header) (document: Document) =
        document
        |> Document.addBlockQuote header
        |> addNewLine

module MdTestInfo =

    let printTestInfo (testInfo: TestInfo) (document: Document) =
        document
        |> Md.printHeader "test info"
        |> Document.addText $"test suite: {Md.printInlineCode testInfo.TestSuite}" |> Md.addNewLine
        |> Document.addText $"test name: {Md.printInlineCode testInfo.TestName}"   |> Md.addNewLine
        |> Document.addText $"session id: {Md.printInlineCode testInfo.SessionId}" |> Md.addNewLine

module MdStatusCodeStats =

    let printScenarioHeader (scenarioName: string) (document: Document) =
        document
        |> Md.printHeader $"status codes for scenario: {Md.printInlineCode scenarioName}"

    let printStatusCodeTable (scnStats: ScenarioStats) (document: Document) =
        let createTableRows = ReportHelper.StatusCodesStats.createTableRows string string
        let headers = ["status code"; "count"; "message"]
        let rows = createTableRows scnStats
        document |> Document.addTable headers rows

module MdLoadSimulations =

    let print (simulations: LoadSimulation list) (document: Document) =
        let initState = document |> Document.addText "load simulations:" |> Md.addNewLine

        simulations
        |> List.fold(fun document simulation ->
            let row = ReportHelper.LoadSimulation.print Md.printInlineCode simulation
            document |> Document.addText row |> Md.addNewLine
        ) initState

module MdNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) (document: Document) =
        document
        |> Document.addText $"scenario: {Md.printInlineCode scnStats.ScenarioName}" |> Md.addNewLine
        |> Document.addText $"  - ok count: {Md.printInlineCode scnStats.Ok.Request.Count}" |> Md.addNewLine
        |> Document.addText $"  - fail count: {Md.printInlineCode scnStats.Fail.Request.Count}" |> Md.addNewLine
        |> Document.addText $"  - all data: {ReportHelper.printAllData Md.printInlineCode (ScenarioStats.calcAllBytes scnStats)}" |> Md.addNewLine
        |> Document.addText $"  - duration: {Md.printInlineCode scnStats.Duration}" |> Md.addNewLine

    let private printStepStatsHeader (stepStats: StepStats[]) (document: Document) =

        let print (document: Document) (stats: StepStats) =
            document
            |> Document.addText $"step: {Md.printInlineCode stats.StepName}" |> Md.addNewLine

        stepStats |> Seq.fold print document

    let private printStepStatsTable (isOkStats: bool) (scnStats: ScenarioStats) (document: Document) =
        let printStepStatsRow = ReportHelper.StepStats.printStepStatsRow isOkStats Md.printInlineCode Md.printInlineCode Md.printInlineCode
        let headers = if isOkStats then ["step"; "ok stats"] else ["step"; "fail stats"]

        scnStats.StepStats
        |> Seq.mapi printStepStatsRow
        |> List.concat
        |> fun rows -> document |> Document.addTable headers rows |> Md.addNewLine

    let private printScenarioStatusCodeStats (scnStats: ScenarioStats) (document: Document) =
        document
        |> MdStatusCodeStats.printScenarioHeader scnStats.ScenarioName
        |> MdStatusCodeStats.printStatusCodeTable scnStats

    let private printScenarioStats (scnStats: ScenarioStats) (simulations: LoadSimulation list) (document: Document) =
        document
        |> Md.printHeader "scenario stats"
        |> printScenarioHeader scnStats
        |> MdLoadSimulations.print simulations
        |> printStepStatsHeader scnStats.StepStats
        |> printStepStatsTable true scnStats
        |> fun doc ->
            if ScenarioStats.failStatsExist scnStats then
                doc |> printStepStatsTable false scnStats
            else
                doc
        |> printScenarioStatusCodeStats scnStats

    let printNodeStats (stats: NodeStats) (loadSimulations: IDictionary<string, LoadSimulation list>) (document: Document) =

        let print (document: Document) (scnStats: ScenarioStats) =
            document
            |> printScenarioStats scnStats loadSimulations[scnStats.ScenarioName]
            |> Md.addNewLine

        stats.ScenarioStats |> Seq.fold print document

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
        |> Seq.map(fun row -> columns |> Seq.map(fun col -> row[col] |> string) |> Seq.toList)
        |> Seq.toList

    let private printPluginStatsTable (table: DataTable) (document: Document) =
        let headers = createPluginStatsTableHeaders(table)
        let rows = createPluginStatsTableRows(table)

        document
        |> printPluginStatsHeader table
        |> Document.addTable headers rows

    let printPluginStats (stats: NodeStats) (document: Document) =
        stats.PluginStats
        |> Seq.collect(fun dataSet -> dataSet.GetTables())
        |> Seq.fold(fun document table -> document |> printPluginStatsTable table |> Md.addNewLine) document

module MdHints =

    let private printHintsHeader (document: Document) =
        document
        |> Md.printHeader "hints:"

    let private createHintsTableRows (hints: HintResult[]) =
        hints
        |> Seq.map(fun hint -> [hint.SourceType.ToString(); hint.SourceName; hint.Hint])
        |> Seq.toList

    let printHints (hints: HintResult[]) (document: Document) =
        if hints.Length > 0 then
            let headers = ["source"; "name"; "hint"]
            let rows = createHintsTableRows(hints)

            document
            |> printHintsHeader
            |> Document.addTable headers rows
        else
            document

let print (logger: ILogger) (sessionResult: NodeSessionResult) (simulations: IDictionary<string, LoadSimulation list>) =
    try
        logger.Verbose("MdReport.print")

        Document.emptyDocument
        |> MdTestInfo.printTestInfo sessionResult.FinalStats.TestInfo
        |> MdNodeStats.printNodeStats sessionResult.FinalStats simulations
        |> MdPluginStats.printPluginStats sessionResult.FinalStats
        |> MdHints.printHints sessionResult.Hints
        |> Document.asString
    with
    | ex ->
        logger.Error(ex, "MdReport.print failed")
        "Could not generate report"
