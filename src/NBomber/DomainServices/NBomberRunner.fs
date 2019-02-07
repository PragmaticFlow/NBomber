module internal NBomber.DomainServices.NBomberRunner

open Serilog

open NBomber.Contracts
open NBomber.Configuration
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes
open NBomber.Domain.Errors
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

let applyAsserts (assertions: Assertion[]) (globalStats: GlobalStats) = 
    assertions
    |> Assertion.apply(globalStats)
    |> fun assrtResult -> (globalStats, assrtResult)

let createAndSaveReport (dep: Dependency, context: NBomberContext) 
                        (globalStats: GlobalStats, assertResults: Result<Assertion,DomainError>[]) =

    let declaredReportFileName = context.ReportFileName
                                 |> Option.defaultValue("report_" + dep.SessionId)
    
    let reportFileName = NBomberContext.tryGetReportFileName(context)
                         |> Option.defaultValue(declaredReportFileName)
    
    let reportFormats = NBomberContext.tryGetReportFormats(context)
                        |> Option.defaultValue(context.ReportFormats)
    
    assertResults
    |> Array.filter(Result.isError)
    |> Array.map(Result.getError)
    |> Report.build(dep, globalStats)
    |> Report.save(dep, "./", reportFileName, reportFormats)

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
                |> Result.map(assertions |> applyAsserts >> createAndSaveReport(dep, context))
                |> ignore // todo: use custom operator >=>
            
            | Agent agentSettings ->
                ClusterAgent.create(dep, registeredScenarios)
                |> ClusterAgent.runAgentListener(agentSettings)
        else
            let scnHost = ScenariosHost(dep, registeredScenarios) 
            runSingleNode(scnHost, scnSettings, targetScenarios)
            |> Result.map(assertions |> applyAsserts >> createAndSaveReport(dep, context))
            |> ignore
        
    | Error ex ->
        let errorMessage = toString(ex)
        if dep.ApplicationType = ApplicationType.Test then TestFrameworkRunner.showValidationErrors(errorMessage)
        else Log.Error(errorMessage)