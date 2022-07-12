namespace NBomber.Contracts.Internal.Serialization.JsonTransforms

open System
open System.Data
open FSharp.Json

type JsonColumn = {
    ColumnName: string
    Caption: string
}

type JsonDataTable = {
    TableName: string
    Columns: JsonColumn[]
    Rows: Map<string,obj>[]
}

type JsonDataSet = {
    Tables: Map<string,JsonDataTable>
}

type DateTableTransform() =
    interface ITypeTransform with
        member x.targetType() = typeof<string>
        member x.fromTargetType(value) = failwith "not implemented"

        member x.toTargetType(value) =
            let data =
                value :?> DataSet[]
                |> Array.map(fun dataSet ->
                    seq {
                        for table in dataSet.Tables do

                            let columns = seq {
                                for col in table.Columns do
                                    { ColumnName = col.ColumnName; Caption = col.Caption }
                            }

                            let rows = seq {
                                for row in table.Rows do
                                    columns
                                    |> Seq.map(fun x -> x.ColumnName, row[x.ColumnName])
                                    |> Map.ofSeq
                            }

                            { TableName = table.TableName
                              Columns = columns |> Seq.toArray
                              Rows = rows |> Seq.toArray }
                    }
                    |> Seq.map(fun x -> x.TableName, x)
                    |> Map.ofSeq
                    |> fun x -> { Tables = x }
                )

            data

type TimeSpanTransform() =
    interface ITypeTransform with
        member x.targetType() = typeof<string>
        member this.fromTargetType(value) = TimeSpan.Parse(value :?> string)
        member this.toTargetType(value) = string(value :?> TimeSpan)
