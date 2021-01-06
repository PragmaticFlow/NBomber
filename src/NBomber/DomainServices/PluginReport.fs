module internal NBomber.DomainServices.PluginReport

open System
open System.Data
open System.Linq

open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

[<Literal>]
let PluginReportTableName = ".plugin-report"

[<Literal>]
let ReportElementTypeColName = "ReportElementType"

[<Literal>]
let ReportElementColName = "ReportElement"

type ReportElement =
    | TxtReportText of string
    | MdReportMarkup of string
    | HtmlReportHead of string
    | HtmlReportBody of string

let createTable () =
    let table = new DataTable(PluginReportTableName)

    [| new DataColumn(ReportElementTypeColName, Type.GetType("System.String"))
       new DataColumn(ReportElementColName, Type.GetType("System.String"))
    |]
    |> table.Columns.AddRange

    table

let addReportElement (table: DataTable) (reportElement: ReportElement) =
        let row = table.NewRow()

        match reportElement with
        | TxtReportText value ->
            row.[ReportElementTypeColName] <- nameof(TxtReportText)
            row.[ReportElementColName] <- value
        | MdReportMarkup value ->
            row.[ReportElementTypeColName] <- nameof(MdReportMarkup)
            row.[ReportElementColName] <- value
        | HtmlReportHead value ->
            row.[ReportElementTypeColName] <- nameof(HtmlReportHead)
            row.[ReportElementColName] <- value
        | HtmlReportBody value ->
            row.[ReportElementTypeColName] <- nameof(HtmlReportBody)
            row.[ReportElementColName] <- value

        table.Rows.Add(row)

        table

let getForAllPlugins (pluginStats: DataSet[]) (colName: string) =
    pluginStats
    |> Seq.map(fun ps ->
        ps
        |> PluginStats.tryFindTableByName PluginReportTableName
        |> Option.map(fun table -> table.Rows.Cast<DataRow>())
        |> Option.map(Seq.filter(fun row -> row.[ReportElementTypeColName].ToString() = colName))
        |> Option.map(Seq.map(fun row -> row.[ReportElementColName].ToString()))
        |> Option.map(String.concatLines)
        |> Option.defaultValue String.Empty
    )
    |> String.concatLines

let getTextReportText (pluginStats: DataSet[]) =
    nameof(TxtReportText) |> getForAllPlugins pluginStats

let getMdReportMarkup (pluginStats: DataSet[]) =
    nameof(MdReportMarkup) |> getForAllPlugins pluginStats

let getHtmlReportHead (pluginStats: DataSet[]) =
    nameof(HtmlReportHead) |> getForAllPlugins pluginStats

let getHtmlReportBody (pluginStats: DataSet[]) =
    nameof(HtmlReportBody) |> getForAllPlugins pluginStats
