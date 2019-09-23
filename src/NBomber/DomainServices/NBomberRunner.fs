module internal NBomber.DomainServices.NBomberRunner

open Serilog
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Configuration
open NBomber.Domain
open NBomber.Errors
open NBomber.Extensions
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Reporting
open NBomber.DomainServices.Reporting.Report
open NBomber.DomainServices.Cluster
open NBomber.DomainServices.ScenariosHost

type ExecutionResult = {     
    AllNodeStats: NodeStats[]
    Statistics: Statistics[]
    FailedAsserts: DomainError[]
} 

let runClusterCoordinator (dep: Dependency, context: NBomberContext, 
                           crdSettings: CoordinatorSettings) = asyncResult {
    sprintf "NBomber started as cluster coordinator: %A" crdSettings
    |> Log.Information

    let scnArgs = ScenariosArgs.getFromContext(dep.SessionId, context)
    let registeredScns = context.Scenarios |> Array.map(Scenario.create)
    
    let! state = Coordinator.init(dep, registeredScns, crdSettings, scnArgs)
    let! allStats = Coordinator.run(state)
    Coordinator.stop(state)
    return allStats
}

let runClusterAgent (dep: Dependency, context: NBomberContext, 
                     agentSettings: AgentSettings) = asyncResult {
    sprintf "NBomber started as cluster agent: %A" agentSettings
    |> Log.Information
    
    let registeredScns = context.Scenarios |> Array.map(Scenario.create)
    
    let! state = Agent.init(dep, registeredScns, agentSettings)    
    do! Agent.startListening(state)
    return Array.empty<NodeStats>
}

let runSingleNode (dep: Dependency, context: NBomberContext) =
    Log.Information("NBomber started as single node")

    let scnArgs = ScenariosArgs.getFromContext(dep.SessionId, context)
    let registeredScns = context.Scenarios |> Array.map(Scenario.create)
    
    let scnHost = ScenariosHost(dep, registeredScns)
    scnHost.RunSession(scnArgs)
    |> AsyncResult.map(fun stats -> [|stats|])

let calcStatistics (dep: Dependency, context: NBomberContext) (allNodeStats: NodeStats[]) =     
    let scnArgs = ScenariosArgs.getFromContext(dep.SessionId, context)
    let registeredScns = context.Scenarios |> Array.map(Scenario.create)
    
    match dep.NodeType with
    | NodeType.SingleNode  -> 
        let statistics = allNodeStats |> Array.exactlyOne |> Statistics.create
        { AllNodeStats = allNodeStats
          Statistics = statistics
          FailedAsserts = Array.empty }
    
    | NodeType.Coordinator -> 
        let mergedStats = Coordinator.mergeStats(dep.SessionId,dep.MachineInfo.MachineName, 
                                                 registeredScns, scnArgs.ScenariosSettings,
                                                 allNodeStats)
        let statistics = Statistics.create(mergedStats)        
        { AllNodeStats = allNodeStats |> Array.append [|mergedStats|]
          Statistics = statistics
          FailedAsserts = Array.empty }
    
    | _ -> { AllNodeStats = Array.empty
             Statistics = Array.empty
             FailedAsserts = Array.empty }

let saveStatistics (context: NBomberContext) (result: ExecutionResult) =    
    if context.StatisticsSink.IsSome then
        context.StatisticsSink.Value.SaveStatistics(result.Statistics).Wait()    
    result

let applyAsserts (context: NBomberContext) (result: ExecutionResult) = 
    let errors = 
        context.Scenarios 
        |> Array.collect(fun x -> x.Assertions)
        |> Assertion.cast
        |> Assertion.apply(result.Statistics)
    { result with FailedAsserts = errors }
    
let buildReport (dep: Dependency) (result: ExecutionResult) =    
    Report.build(dep, result.AllNodeStats, result.FailedAsserts)

let saveReport (dep: Dependency, context: NBomberContext) (report: ReportResult) =
    let fileName = NBomberContext.getReportFileName(dep.SessionId, context)
    let formats = NBomberContext.getReportFormats(context)
    Report.save(dep, "./", fileName, formats, report)
    
let showErrors (dep: Dependency) (errors: AppError[]) = 
    if dep.ApplicationType = ApplicationType.Test then
        TestFrameworkRunner.showErrors(errors)
    else
        errors |> Array.iter(AppError.toString >> Log.Error)

let showAsserts (dep: Dependency) (result: ExecutionResult) =
    match result.FailedAsserts with
    | [||]          -> ()
    | failedAsserts -> failedAsserts
                       |> Array.map AppError.create
                       |> showErrors dep

let run (dep: Dependency) (context: NBomberContext) =
    asyncResult {        
        Log.Information("NBomber '{0}' started a new session: '{1}'", dep.NBomberVersion, dep.SessionId)

        let! ctx = Validation.validateContext(context)
        let! nodeStats =
            match NBomberContext.tryGetClusterSettings(ctx) with
            | Some (Coordinator c) -> runClusterCoordinator(dep, ctx, c)
            | Some (Agent a)       -> runClusterAgent(dep, ctx, a)
            | None                 -> runSingleNode(dep, ctx)

        let result = nodeStats
                     |> calcStatistics(dep, ctx)
                     |> saveStatistics(ctx)
                     |> applyAsserts(ctx)

        result |> buildReport(dep) |> saveReport(dep, ctx)
        return result
    }
    |> AsyncResult.map(showAsserts(dep))
    |> AsyncResult.mapError(fun error -> showErrors dep [|error|]) 
    |> Async.RunSynchronously
    |> Result.toExitCode

let runAs (appType: ApplicationType) (context: NBomberContext) =
    let logSettings = NBomberContext.tryGetLogSettings(context)
    let nodeType = NBomberContext.getNodeType(context)
    let dep = Dependency.create(appType, nodeType, logSettings)
    run dep context