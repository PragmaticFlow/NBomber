namespace NBomber.Extensions

open System
open System.Data
open System.Diagnostics
open System.Linq
open System.Runtime.CompilerServices

open Newtonsoft.Json

[<Extension>]
type StringExtensions() =

    [<Extension>]
    static member DeserializeJson<'T>(json: string) =
        JsonConvert.DeserializeObject<'T>(json)

    [<Extension>]
    static member DeserializeYaml<'T>(yaml: string) =
        let deserializer = YamlDotNet.Serialization.Deserializer()
        yaml |> deserializer.Deserialize<'T>

type CurrentTime() =

    let _timer = Stopwatch()
    let _initTime = DateTime.UtcNow

    do _timer.Start()

    member _.UtcNow = _initTime + _timer.Elapsed

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
    static member GetColumnCaptionOrName (column: DataColumn) =
        if String.IsNullOrEmpty(column.Caption)
        then column.ColumnName
        else column.Caption
