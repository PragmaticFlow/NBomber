module internal NBomber.DomainServices.PluginStats

open System
open System.Data

open NBomber.Contracts
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

[<Literal>]
let CustomPluginDataTableName = ".custom-html-report"

let getStatsTables (pluginStats: DataSet[]) =
    pluginStats
    |> Seq.collect(fun dataSet -> dataSet.GetTables())
    |> Seq.filter(fun table -> table.Rows.Count > 0)
    |> Seq.filter(fun table -> not(table.TableName.StartsWith(".")))

let tryFindPluginStatsByName (pluginName) (pluginStats: DataSet[]) =
    pluginStats
    |> Array.tryFind(fun pluginStats -> pluginStats.DataSetName = pluginName)

let tryFindTableByName (tableName) (pluginStats: DataSet) =
    pluginStats.GetTables()
    |> Seq.filter(fun table -> table.Rows.Count > 0)
    |> Seq.tryFind(fun table -> table.TableName = tableName)

let createCustomPluginDataTable (context: CustomHtmlReportContext) =
    let table = new DataTable(CustomPluginDataTableName)

    [| new DataColumn("Title", Type.GetType("System.String"))
       new DataColumn("Header", Type.GetType("System.String"))
       new DataColumn("Js", Type.GetType("System.String"))
       new DataColumn("HtmlTemplate", Type.GetType("System.String"))
       new DataColumn("ViewModel", Type.GetType("System.String"))
    |]
    |> table.Columns.AddRange

    let row = table.NewRow()
    row.["Title"] <- context.Title
    row.["Header"] <- context.Header
    row.["Js"] <- context.Js
    row.["HtmlTemplate"] <- context.HtmlTemplate
    row.["ViewModel"] <- context.ViewModel
    table.Rows.Add(row)

    table

let tryGetCustomPluginData (pluginStats: DataSet) =
    pluginStats
    |> tryFindTableByName CustomPluginDataTableName
    |> Option.map(fun table ->
        let row = table.Rows.[0]
        {
            Title = row.["Title"].ToString()
            Header = row.["Header"].ToString()
            Js = row.["Js"].ToString()
            HtmlTemplate = row.["HtmlTemplate"].ToString()
            ViewModel = row.["ViewModel"].ToString()
        }
    )

let private joinForAllPlugins (colName: string) (pluginStats: DataSet[]) =
    pluginStats
    |> Seq.map(fun ps ->
        ps
        |> tryFindTableByName CustomPluginDataTableName
        |> Option.map(fun table -> table.Rows.[0].[colName].ToString())
        |> Option.defaultValue String.Empty
    )
    |> String.concatLines

let getCustomHeaders (pluginStats: DataSet[]) =
    pluginStats |> joinForAllPlugins "Header"

let getCustomJs (pluginStats: DataSet[]) =
    pluginStats |> joinForAllPlugins "Js"

let getCustomHtmlTemplates (pluginStats: DataSet[]) =
    pluginStats
    |> Seq.map(fun ps ->
        ps
        |> tryFindTableByName CustomPluginDataTableName
        |> Option.map(fun table -> table.Rows.[0].["HtmlTemplate"].ToString())
        |> Option.defaultValue String.Empty
        |> fun htmlTemplate -> htmlTemplate, ps.DataSetName
    )
    |> Seq.filter(fun (htmlTemplate, _) -> htmlTemplate <> String.Empty)

