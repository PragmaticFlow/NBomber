module internal NBomber.DomainServices.NBomberContext

open NBomber.Extensions
open NBomber.Configuration
open NBomber.Contracts

let getScenariosSettings (context: NBomberContext) =    
    context.NBomberConfig
    |> Option.bind(fun x -> x.GlobalSettings)
    |> Option.map(fun x -> x.ScenariosSettings)
    |> Option.defaultValue List.empty
    |> List.toArray

let tryGetClusterSettings (context: NBomberContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.ClusterSettings)

let getNodeType (context: NBomberContext) = 
    context.NBomberConfig
    |> Option.bind(fun x -> x.ClusterSettings)
    |> Option.map(function 
        | ClusterSettings.Coordinator _ -> NodeType.Coordinator
        | ClusterSettings.Agent _       -> NodeType.Agent)
    |> Option.defaultValue NodeType.SingleNode        

let getTargetScenarios (context: NBomberContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.GlobalSettings)
    |> Option.map(fun x -> x.TargetScenarios)
    |> Option.defaultValue List.empty
    |> List.toArray

let tryGetReportFileName (context: NBomberContext) = maybe {
    let! config = context.NBomberConfig
    let! settings = config.GlobalSettings
    return! settings.ReportFileName
}   

let tryGetReportFormats (context: NBomberContext) = maybe {
    let! config = context.NBomberConfig
    let! settings = config.GlobalSettings
    return! settings.ReportFormats    
}

let tryGetLogSettings (context: NBomberContext) = maybe {
    let! config = context.NBomberConfig
    return! config.LogSetting
}