module internal NBomber.DomainServices.WorkerPlugin

open System.Data

open NBomber.Contracts
open NBomber.Extensions

[<Literal>]
let private PluginFullNameProp = "PluginFullName"

let getFullName (plugin: IWorkerPlugin) =
    $"{plugin.GetType().FullName}.{plugin.PluginName}"

let getStatsTables (pluginStats: DataSet[]) =
    pluginStats
    |> Seq.collect(fun dataSet -> dataSet.GetTables())
    |> Seq.filter(fun table -> table.Rows.Count > 0)

let tryFindPluginStats (pluginFullName: string) (pluginStats: DataSet[]) =
    pluginStats
    |> Array.tryFind(fun pluginStats ->
        let props = pluginStats.ExtendedProperties
        props.ContainsKey(PluginFullNameProp)
            && props.[PluginFullNameProp] :? string
            && props.[PluginFullNameProp].ToString() = pluginFullName
    )

let enrichPluginStats (pluginStats: DataSet, plugin: IWorkerPlugin) =
    let pluginFullName = getFullName(plugin)
    pluginStats.ExtendedProperties.Add(PluginFullNameProp, pluginFullName)
    pluginStats
