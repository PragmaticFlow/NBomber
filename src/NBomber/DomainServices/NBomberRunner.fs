module internal NBomber.DomainServices.NBomberRunner

open Serilog
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Configuration
open NBomber.Domain
open NBomber.Errors
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
    let scnSettings = NBomberContext.getScenariosSettings context
    let targetScns = NBomberContext.getTargetScenarios context
    let registeredScns = context.Scenarios |> Array.map Scenario.create
    scnSettings, targetScns, registeredScns

let runClusterCoordinator (dep: Dependency, context: NBomberContext,
                           crdSettings: CoordinatorSettings) =
    sprintf "NBomber started as cluster coordinator: %A" crdSettings
    |> Log.Information

    let scnSettings, targetScns, registeredScns = getScenariosArgs context
    ClusterCoordinator.create(dep, registeredScns, crdSettings)
    |> ClusterCoordinator.run(scnSettings, targetScns)

let runClusterAgent (dep: Dependency, context: NBomberContext, agentSettings: AgentSettings) =
    sprintf "NBomber started as cluster agent: %A" agentSettings
    |> Log.Information

    let _, _, registeredScns = getScenariosArgs context
    ClusterAgent.create(dep, registeredScns)
    |> ClusterAgent.runAgentListener agentSettings
    Array.empty<NodeStats> |> Ok |> Async.singleton

let runSingleNode (dep: Dependency, context: NBomberContext) =
    Log.Information("NBomber started as single node")

    let scnSettings, targetScns, registeredScns = getScenariosArgs context
    ScenariosHost.create(dep, registeredScns)
    |> ScenariosHost.run(scnSettings, targetScns)
    |> AsyncResult.map(fun stats -> [|stats|])
    |> Some

let calcStatistics (dep: Dependency, context: NBomberContext) (allNodeStats: NodeStats[]) =
    let scnSettings, _, registeredScns = getScenariosArgs context
    match dep.NodeType with
    | NodeType.SingleNode  ->
        let statistics = allNodeStats |> Array.exactlyOne |> Statistics.create
        { AllNodeStats = allNodeStats
          Statistics = statistics
          FailedAsserts = Array.empty }

    | NodeType.Coordinator ->
        let clusterNodeStats =
            ClusterCoordinator.createStats(dep.SessionId, dep.NodeInfo.NodeName,
                                           registeredScns, scnSettings, allNodeStats)
        let statistics = Statistics.create clusterNodeStats
        { AllNodeStats = allNodeStats |> Array.append [|clusterNodeStats|]
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
        |> Assertion.create
        |> Assertion.apply result.Statistics
    { result with FailedAsserts = errors }

let buildReport (dep: Dependency) (result: ExecutionResult) =
    Report.build(dep, result.AllNodeStats, result.FailedAsserts)

let saveReport (dep: Dependency, context: NBomberContext) (report: ReportResult) =
    let defaultFileName = context.ReportFileName
                          |> Option.defaultValue("report_" + dep.SessionId)

    let fileName = NBomberContext.tryGetReportFileName context
                   |> Option.defaultValue defaultFileName

    let formats = NBomberContext.tryGetReportFormats context
                  |> Option.defaultValue context.ReportFormats

    Report.save(dep, "./", fileName, formats, report)

let showErrors (dep: Dependency) (errors: AppError[]) =
    if dep.ApplicationType = ApplicationType.Test then
        TestFrameworkRunner.showErrors errors
    else
        errors |> Array.iter(AppError.toString >> Log.Error)

let showAsserts (dep: Dependency) (result: ExecutionResult) =
    match result.FailedAsserts with
    | [||]          -> ()
    | failedAsserts -> failedAsserts |> Array.map AppError.create |> showErrors dep

let run (dep: Dependency, context: NBomberContext) =
    asyncResult {
        Dependency.Logger.initLogger dep.ApplicationType
        Log.Information("NBomber started a new session: '{0}'", dep.SessionId)

        let! ctx = Validation.validateContext context
        let! nodeStats =
            NBomberContext.tryGetClusterSettings ctx
            |> Option.map(function
                | Coordinator c        -> runClusterCoordinator(dep, ctx, c)
                | Agent a              -> runClusterAgent(dep, ctx, a))
            |> Option.orElseWith(fun _ -> runSingleNode(dep, ctx))
            |> Option.get

        let result = nodeStats |> calcStatistics(dep, ctx) |> saveStatistics ctx |> applyAsserts ctx
        result |> buildReport dep |> saveReport(dep, ctx)
        return result
    }
    |> AsyncResult.map(showAsserts dep)
    |> AsyncResult.mapError(fun error -> showErrors dep [|error|])
    |> Async.RunSynchronously
    |> ignore
