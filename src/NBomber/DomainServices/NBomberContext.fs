module internal NBomber.DomainServices.NBomberContext

open NBomber.Configuration
open NBomber.Contracts

let getScenariosSettings (context: NBomberContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.GlobalSettings)
    |> Option.bind(fun x -> Option.ofObj(x.ScenariosSettings))
    |> Option.defaultValue Array.empty

let tryGetClusterSettings (context: NBomberContext) = maybe {
    let! config = context.NBomberConfig
    let! clusterSettings = config.ClusterSettings
    return clusterSettings
}

let getNodeType (context: NBomberContext) =
    let cluster = maybe {
        let! config = context.NBomberConfig
        match! config.ClusterSettings with
        | Coordinator _ -> return NodeType.Coordinator
        | Agent _       -> return NodeType.Agent
    }
    cluster |> Option.defaultValue NodeType.SingleNode

let getTargetScenarios (context: NBomberContext) =
    let targetScenarios =
        context.NBomberConfig
        |> Option.bind(fun x -> x.GlobalSettings)
        |> Option.bind(fun x -> Option.ofObj x.TargetScenarios)

    targetScenarios |> Option.defaultValue Array.empty

let tryGetReportFileName (context: NBomberContext) = maybe {
    let! config = context.NBomberConfig
    let! globalSettings = config.GlobalSettings
    return! Option.ofObj globalSettings.ReportFileName
}

let tryGetReportFormats (context: NBomberContext) = maybe {
    let! config = context.NBomberConfig
    let! globalSettings = config.GlobalSettings
    let reportFormats = globalSettings.ReportFormats |> Array.choose ReportFormat.tryParse
    return! Some reportFormats
}

let trySaveStatistics (context: NBomberContext) (stats: Statistics[]) =
    context.StatisticsSink
    |> Option.iter (fun sink -> sink.SaveStatistics(stats).Wait())
