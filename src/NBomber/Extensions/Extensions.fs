namespace NBomber.Extensions

open System
open System.Data
open System.Linq
open System.Runtime.CompilerServices

[<Extension>]
type DataSetExtensions() =

    [<Extension>]
    static member GetTables(dataSet: DataSet) =
        dataSet.Tables.Cast<DataTable>() |> Array.ofSeq

    [<Extension>]
    static member GetColumns(dataTable: DataTable) =
        dataTable.Columns.Cast<DataColumn>() |> Array.ofSeq

    [<Extension>]
    static member GetRows(dataTable: DataTable) =
        dataTable.Rows.Cast<DataRow>() |> Array.ofSeq

    [<Extension>]
    static member GetColumnCaptionOrName(column: DataColumn) =
        if String.IsNullOrEmpty(column.Caption)
        then column.ColumnName
        else column.Caption

module DataGenerator =

    [<CompiledName("GenerateRandomBytes")>]
    let generateRandomBytes (sizeInBytes: int) =
        let buffer = Array.zeroCreate<byte> sizeInBytes;
        Random().NextBytes(buffer)
        buffer
