module internal NBomber.DomainServices.PluginStats

open System.Data

open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

let getStatsTables (pluginStats: DataSet[]) =
    pluginStats
    |> Seq.collect(fun dataSet -> dataSet.GetTables())
    |> Seq.filter(fun table -> table.Rows.Count > 0)
    |> Seq.filter(fun table -> not(table.TableName.StartsWith(".")))

let tryFindDataSetByName (pluginName) (pluginStats: DataSet[]) =
    pluginStats
    |> Array.tryFind(fun pluginStats -> pluginStats.DataSetName = pluginName)

let tryFindTableByName (tableName) (pluginStats: DataSet) =
    pluginStats.GetTables()
    |> Seq.filter(fun table -> table.Rows.Count > 0)
    |> Seq.tryFind(fun table -> table.TableName = tableName)
