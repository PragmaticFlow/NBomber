module internal NBomber.DomainServices.Reporting.TxtReportExtStats

open System
open System.Data

open ConsoleTables

open NBomber.Extensions

let inline private concatLines s =
    String.concat Environment.NewLine s

let inline private printExtensionStatisticHeader (table: DataTable) =
    sprintf "statistics: '%s'" table.TableName

let private printExtensionStatisticTable (table: DataTable) =
    let columnNames = table.GetColumns() |> Array.map(fun x -> x.ColumnName)
    let columnCaptions = table.GetColumns() |> Array.map(fun x -> x.GetColumnCaptionOrName())
    let extensionStatisticTable = ConsoleTable(columnCaptions)

    table.GetRows()
    |> Array.map(fun x -> columnNames |> Array.map(fun columnName -> x.[columnName]))
    |> Array.iter(fun x -> extensionStatisticTable.AddRow(x) |> ignore)

    extensionStatisticTable.ToStringAlternative()

let print (table: DataTable) =
        [ table |> printExtensionStatisticHeader
          table |> printExtensionStatisticTable
        ] |> concatLines
