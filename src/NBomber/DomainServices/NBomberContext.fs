module internal NBomber.DomainServices.NBomberContext

open NBomber.Contracts

let getScenariosSettings (context: NBomberContext) =
    let settings = 
        context.NBomberConfig
        |> Option.bind(fun x -> x.GlobalSettings)
        |> Option.bind(fun x -> Option.ofObj(x.ScenariosSettings))
                   
    match settings with
    | Some v -> v
    | None   -> Array.empty

let tryGetClusterSettings (context: NBomberContext) = maybe {
    let! config = context.NBomberConfig
    let! clusterSettings = config.ClusterSettings
    return clusterSettings
}

let getTargetScenarios (context: NBomberContext) =
    let targetScenarios = 
        context.NBomberConfig
        |> Option.bind(fun x -> x.GlobalSettings)
        |> Option.bind(fun x -> Option.ofObj(x.TargetScenarios))

    match targetScenarios with
    | Some v -> v
    | None   -> Array.empty

let tryGetReportFileName (context: NBomberContext) = maybe {
    let! config = context.NBomberConfig
    let! globalSettings = config.GlobalSettings
    return! Option.ofObj(globalSettings.ReportFileName)
}

let tryGetReportFormats (context: NBomberContext) = maybe {
    let! config = context.NBomberConfig
    let! globalSettings = config.GlobalSettings
    let reportFormats = globalSettings.ReportFormats |> Array.choose(Validation.isReportFormatSupported)              
    return! Option.ofObj(reportFormats)
}