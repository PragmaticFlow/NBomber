module internal NBomber.DomainServices.NBomberTestContext

open NBomber.Extensions
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain

let getTestSuite (context: NBomberTestContext) =
    context.NBomberConfig
    |> Option.map(fun x -> x.TestSuite)
    |> Option.defaultValue context.TestSuite

let getTestName (context: NBomberTestContext) =
    context.NBomberConfig
    |> Option.map(fun x -> x.TestName)
    |> Option.defaultValue context.TestName

let getScenariosSettings (context: NBomberTestContext) =    
    context.NBomberConfig
    |> Option.bind(fun x -> x.GlobalSettings)
    |> Option.bind(fun x -> x.ScenariosSettings)
    |> Option.defaultValue List.empty
    |> List.toArray

let tryGetClusterSettings (context: NBomberTestContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.ClusterSettings)

let getNodeType (context: NBomberTestContext) = 
    context.NBomberConfig
    |> Option.bind(fun x -> x.ClusterSettings)
    |> Option.map(function 
        | ClusterSettings.Coordinator _ -> NodeType.Coordinator
        | ClusterSettings.Agent _       -> NodeType.Agent)
    |> Option.defaultValue NodeType.SingleNode        

let getTargetScenarios (context: NBomberTestContext) =
    let targetScn =
        context.NBomberConfig
        |> Option.bind(fun x -> x.GlobalSettings)
        |> Option.bind(fun x -> x.TargetScenarios)

    let allScns = context.Scenarios 
                  |> Array.map(fun x -> x.ScenarioName)
                  |> Array.toList

    defaultArg targetScn allScns
    |> List.toArray

let getReportFileName (sessionId: string, context: NBomberTestContext) = 
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ReportFileName    
    }
    context
    |> tryGetFromConfig
    |> Option.orElse(context.ReportFileName)
    |> Option.defaultValue("report_" + sessionId)

let getReportFormats (context: NBomberTestContext) = 
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        let! formats = settings.ReportFormats
        return formats |> List.toArray
    }
    context
    |> tryGetFromConfig
    |> Option.orElse(if Array.isEmpty context.ReportFormats then None
                     else Some context.ReportFormats)
    |> Option.defaultValue Constants.AllReportFormats

let tryGetCustomSettings (context: NBomberTestContext) =
    maybe {
        let! config = context.NBomberConfig
        return! config.CustomSettings
    }
    |> function Some v -> v | None -> ""