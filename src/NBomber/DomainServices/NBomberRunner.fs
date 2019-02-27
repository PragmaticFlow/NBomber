module internal NBomber.DomainServices.NBomberRunner

open Serilog

open NBomber.Contracts
open NBomber.Configuration
open NBomber.Domain
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Reporting
open NBomber.DomainServices.ScenariosHost
open NBomber.DomainServices.Cluster

let runSingleNode (scnHost: IScenariosHost, settings: ScenarioSetting[],
                   targetScenarios: string[]) = trial {
    do! scnHost.InitScenarios(settings, targetScenarios).Result
    scnHost.WarmUpScenarios().Wait()
    scnHost.RunBombing().Wait()
    scnHost.StopScenarios()
    return scnHost.GetStatistics()
}

let createAndSaveReport (dep: Dependency, context: NBomberContext,
                         nodeStats: NodeStats, 
                         assertResults: Result<Assertion,DomainError>[]) =

    let declaredReportFileName = context.ReportFileName
                                 |> Option.defaultValue("report_" + dep.SessionId)
    
    let reportFileName = NBomberContext.tryGetReportFileName(context)
                         |> Option.defaultValue(declaredReportFileName)
    
    let reportFormats = NBomberContext.tryGetReportFormats(context)
                        |> Option.defaultValue(context.ReportFormats)
    
    assertResults
    |> Array.filter(Result.isError)
    |> Array.map(Result.getError)
    |> Report.build(dep, nodeStats)
    |> Report.save(dep, "./", reportFileName, reportFormats)

let handleError (appType: ApplicationType, error: DomainError) =    
    if appType = ApplicationType.Test then TestFrameworkRunner.showErrors([|error|])
    else error |> Errors.toString |> Log.Error

let showAssertsResults (appType: ApplicationType, asrtResults: Result<Assertion,DomainError>[]) =     
    if asrtResults |> Array.exists(Result.isError) then
        let errors = asrtResults |> Array.filter(Result.isError) |> Array.map(Result.getError)
        if appType = ApplicationType.Test then            
            TestFrameworkRunner.showErrors(errors)
        else
            errors |> Array.iter(Errors.toString >> Log.Error)

let run (dep: Dependency, context: NBomberContext) =
    Dependency.Logger.initLogger(dep.ApplicationType)    
    Log.Information("NBomber started a new session: '{0}'", dep.SessionId)

    match Validation.validateRunnerContext(context) with
    | Ok context ->        
        let scnSettings     = NBomberContext.getScenariosSettings(context)
        let targetScenarios = NBomberContext.getTargetScenarios(context) 
        let clusterSettings = NBomberContext.tryGetClusterSettings(context)
        let registeredScenarios = ScenarioBuilder.build(context.Scenarios)        
        let assertions = context.Scenarios |> Array.collect(fun x -> x.Assertions) |> Assertion.create
        
        if clusterSettings.IsSome then
            match clusterSettings.Value with
            | Coordinator coordinatorSettings ->                 
                let cluster = ClusterCoordinator.create(dep, registeredScenarios, coordinatorSettings)
                cluster.Run(scnSettings, targetScenarios)
                |> Result.map(fun allNodeStats -> 

                    let clusterNodeStats = ClusterCoordinator.createStats(dep.SessionId, dep.NodeInfo.NodeName, registeredScenarios, scnSettings, allNodeStats)                    
                    let statistics = Statistics.create(clusterNodeStats)
                    let asrtResults = statistics |> Assertion.apply(assertions)                    
                    createAndSaveReport(dep, context, clusterNodeStats, asrtResults)
                    statistics |> NBomberContext.trySaveStatistics(context)                    
                    showAssertsResults(dep.ApplicationType, asrtResults)
                )
                |> Result.mapError(fun error -> handleError(dep.ApplicationType, error))
                |> ignore
            
            | Agent agentSettings ->
                ClusterAgent.create(dep, registeredScenarios)
                |> ClusterAgent.runAgentListener(agentSettings)
        else
            let scnHost = ScenariosHost(dep, registeredScenarios) 
            runSingleNode(scnHost, scnSettings, targetScenarios)
            |> Result.map(fun nodeStats ->
                let statistics = Statistics.create(nodeStats)
                let asrtResults = statistics |> Assertion.apply(assertions)
                createAndSaveReport(dep, context, nodeStats, asrtResults)
                statistics |> NBomberContext.trySaveStatistics(context)
                showAssertsResults(dep.ApplicationType, asrtResults)
            )
            |> Result.mapError(fun error -> handleError(dep.ApplicationType, error))
            |> ignore            
        
    | Error error ->
        handleError(dep.ApplicationType, error)