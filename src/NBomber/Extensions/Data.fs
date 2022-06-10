namespace NBomber.Extensions.Data

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

module Converter =

    [<CompiledName("FromMicroSecToMs")>]
    let fromMicroSecToMs (microSec: float) = (microSec / 1000.0)

    [<CompiledName("FromMsToMicroSec")>]
    let fromMsToMicroSec (ms: float) = (ms * 1000.0) |> int

    [<CompiledName("FromBytesToKb")>]
    let inline fromBytesToKb (bytes) = Math.Round(float bytes / 1024.0, 3)

    [<CompiledName("FromBytesToMb")>]
    let inline fromBytesToMb (bytes) = Math.Round(decimal bytes / 1024.0M / 1024.0M, 4)

    [<CompiledName("Round")>]
    let inline round (digits: int) (value: float) = Math.Round(value, digits)
