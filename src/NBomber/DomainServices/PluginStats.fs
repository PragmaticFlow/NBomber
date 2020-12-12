module internal NBomber.DomainServices.PluginStats

open System.Data

open NBomber.Contracts
open NBomber.Extensions

let getStatsTables (pluginStats: DataSet[]) =
    pluginStats
    |> Seq.collect(fun dataSet -> dataSet.GetTables())
    |> Seq.filter(fun table -> table.Rows.Count > 0)
    |> Seq.filter(fun table -> not(table.TableName.StartsWith(".")))

let tryFindPluginStatsByName (pluginName) (nodeStats: NodeStats) =
    nodeStats.PluginStats
    |> Array.tryFind(fun pluginStats -> pluginStats.DataSetName = pluginName)
