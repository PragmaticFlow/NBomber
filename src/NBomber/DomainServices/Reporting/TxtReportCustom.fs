module internal NBomber.DomainServices.Reporting.TxtReportCustom

open System
open System.Data

open ConsoleTables

open NBomber.Extensions

let inline private concatLines s =
    String.concat Environment.NewLine s

let inline private printCustomStatisticHeader (table: DataTable) =
    sprintf "statistics: '%s'" table.TableName

let private printCustomStatisticTable (table: DataTable) =
    let columnNames = table.GetColumns() |> Array.map(fun x -> x.ColumnName)
    let columnCaptions = table.GetColumns() |> Array.map(fun x -> x.GetColumnCaptionOrName())
    let customStatisticTable = ConsoleTable(columnCaptions)

    table.GetRows()
    |> Array.map(fun x -> columnNames |> Array.map(fun columnName -> x.[columnName]))
    |> Array.iter(fun x -> customStatisticTable.AddRow(x) |> ignore)

    customStatisticTable.ToStringAlternative()

let print (table: DataTable) =
        [ table |> printCustomStatisticHeader
          table |> printCustomStatisticTable
        ] |> concatLines
