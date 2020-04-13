module internal NBomber.DomainServices.Reporting.TxtReportPluginStats

open System
open System.Data

open ConsoleTables

open NBomber.Extensions

let inline private concatLines s =
    String.concat Environment.NewLine s

let inline private printPluginStatisticHeader (table: DataTable) =
    sprintf "statistics: '%s'" table.TableName

let private printPluginStatisticTable (table: DataTable) =
    let columnNames = table.GetColumns() |> Array.map(fun x -> x.ColumnName)
    let columnCaptions = table.GetColumns() |> Array.map(fun x -> x.GetColumnCaptionOrName())
    let consoleTable = ConsoleTable(columnCaptions)

    table.GetRows()
    |> Array.map(fun x -> columnNames |> Array.map(fun columnName -> x.[columnName]))
    |> Array.iter(fun x -> consoleTable.AddRow(x) |> ignore)

    consoleTable.ToStringAlternative()

let print (table: DataTable) =
    [table |> printPluginStatisticHeader
     table |> printPluginStatisticTable]
    |> concatLines
