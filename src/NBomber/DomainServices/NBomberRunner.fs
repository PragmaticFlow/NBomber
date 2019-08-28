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

type ExecutionResult = {     
    AllNodeStats: NodeStats[]
    Statistics: Statistics[]
    FailedAsserts: DomainError[]
}

let getScenariosArgs (context: NBomberContext) =
    let scnSettings = NBomberContext.getScenariosSettings(context)
    let targetScns = NBomberContext.getTargetScenarios(context)
    let registeredScns = context.Scenarios |> Array.map(Scenario.create)    
    scnSettings, targetScns, registeredScns

let runClusterCoordinator (dep: Dependency, context: NBomberContext, 
                           crdSettings: CoordinatorSettings) = asyncResult {
    sprintf "NBomber started as cluster coordinator: %A" crdSettings
    |> Log.Information

    let scnSettings, _, registeredScns = getScenariosArgs(context)
    
    let! state = Coordinator.init(dep, registeredScns, crdSettings, scnSettings)
    let! stats = Coordinator.run state
    Coordinator.stop state
    return stats
}

let runClusterAgent (dep: Dependency, context: NBomberContext, 
                     agentSettings: AgentSettings) = asyncResult {
    sprintf "NBomber started as cluster agent: %A" agentSettings
    |> Log.Information

    let _, _, registeredScns = getScenariosArgs(context)
    let! state = Agent.init(dep, registeredScns, agentSettings)    
    do! Agent.run(state)
    return Array.empty<NodeStats>
}

let runSingleNode (dep: Dependency, context: NBomberContext) =
    Log.Information("NBomber started as single node")

    let scnSettings, targetScns, registeredScns = getScenariosArgs context
    ScenariosHost.create(dep, registeredScns)
    |> ScenariosHost.run dep.SessionId scnSettings targetScns
    |> AsyncResult.map(fun stats -> [|stats|])

let calcStatistics (dep: Dependency, context: NBomberContext) (allNodeStats: NodeStats[]) =     
    let scnSettings, _, registeredScns = getScenariosArgs(context)
    match dep.NodeType with
    | NodeType.SingleNode  -> 
        let statistics = allNodeStats |> Array.exactlyOne |> Statistics.create
        { AllNodeStats = allNodeStats
          Statistics = statistics
          FailedAsserts = Array.empty }
    
    | NodeType.Coordinator -> 
        let mergedStats = Coordinator.mergeStats(dep.SessionId, dep.MachineInfo.MachineName, 
                                                 registeredScns, scnSettings, allNodeStats)
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
        let logSettings = NBomberContext.tryGetLogSettings(context)
        Dependency.Logger.initLogger(dep.ApplicationType, logSettings)
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
    let nodeType = NBomberContext.getNodeType context
    let dep = Dependency.create(appType, nodeType)
    run dep context