module internal NBomber.DomainServices.NBomberContext

open NBomber.Extensions
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain

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
    let targetScn =
        context.NBomberConfig
        |> Option.bind(fun x -> x.GlobalSettings)
        |> Option.bind(fun x -> x.TargetScenarios)

    let allScns = context.Scenarios 
                  |> Array.map(fun x -> x.ScenarioName)
                  |> Array.toList

    defaultArg targetScn allScns
    |> List.toArray

let getReportFileName (sessionId: string, context: NBomberContext) = 
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ReportFileName    
    }
    context
    |> tryGetFromConfig
    |> Option.orElse(context.ReportFileName)
    |> Option.defaultValue("report_" + sessionId)

let getReportFormats (context: NBomberContext) = 
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ReportFormats    
    }
    context
    |> tryGetFromConfig
    |> Option.orElse(if List.isEmpty context.ReportFormats then None
                     else Some context.ReportFormats)
    |> Option.defaultValue Constants.AllReportFormats

let tryGetCustomSettings (context: NBomberContext) =
    maybe {
        let! config = context.NBomberConfig
        return! config.CustomSettings
    }
    |> function Some v -> v | None -> ""