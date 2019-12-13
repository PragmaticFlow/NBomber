module internal NBomber.DomainServices.NBomberRunner

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
open NBomber.DomainServices.ScenariosHost
open NBomber.DomainServices.Cluster.Agent
open NBomber.DomainServices.Cluster.Coordinator

type ExecutionResult = {     
    RawNodeStats: RawNodeStats[]
    Statistics: Statistics[]
    FailedAsserts: DomainError[]
} with
  static member init (nodeStats: RawNodeStats[]) =
      let stats = nodeStats |> Array.collect(Statistics.create)
      { RawNodeStats = nodeStats; Statistics = stats; FailedAsserts = Array.empty }

let runClusterCoordinator (dep: Dependency, context: NBomberContext, crdSettings: CoordinatorSettings) =
    asyncResult {
        sprintf "NBomber started as cluster coordinator: %A" crdSettings
        |> dep.Logger.Information

        let scnArgs = ScenariosArgs.getFromContext(dep.SessionId, context)
        let registeredScns = context.Scenarios |> Array.map(Scenario.create)
        
        use coordinator = new ClusterCoordinator()
        let! clusterStats = coordinator.RunSession(dep, registeredScns, crdSettings, scnArgs)        
        return clusterStats
    }

let runClusterAgent (dep: Dependency, context: NBomberContext, agentSettings: AgentSettings) =
    asyncResult {
        sprintf "NBomber started as cluster agent: %A" agentSettings
        |> dep.Logger.Information
        
        let registeredScns = context.Scenarios |> Array.map(Scenario.create)
        
        use agent = new ClusterAgent()
        do! agent.StartListening(dep, registeredScns, agentSettings)
        return Array.empty<RawNodeStats>
    }

let runSingleNode (dep: Dependency, context: NBomberContext) =
    asyncResult {
        dep.Logger.Information("NBomber started as single node")

        let scnArgs = ScenariosArgs.getFromContext(dep.SessionId, context)
        let registeredScns = context.Scenarios |> Array.map(Scenario.create)

        use scnHost = new ScenariosHost(dep, registeredScns)
        let! rawNodeStats = scnHost.RunSession(scnArgs)
        return [|rawNodeStats|]
    }

let applyAsserts (context: NBomberContext) (result: ExecutionResult) = 
    let errors = 
        context.Scenarios 
        |> Array.collect(fun x -> x.Assertions)
        |> Assertion.cast
        |> Assertion.apply(result.Statistics)
    { result with FailedAsserts = errors }

let buildReport (dep: Dependency) (result: ExecutionResult) =    
    Report.build(dep, result.RawNodeStats, result.FailedAsserts)

let saveReport (dep: Dependency) (context: NBomberContext) (report: ReportResult) =
    let fileName = NBomberContext.getReportFileName(dep.SessionId, context)
    let formats = NBomberContext.getReportFormats(context)
    Report.save("./", fileName, formats, report, dep.Logger)

let showErrors (dep: Dependency) (errors: AppError[]) = 
    if dep.ApplicationType = ApplicationType.Test then
        TestFrameworkRunner.showErrors(errors)
    else
        errors |> Array.iter(AppError.toString >> dep.Logger.Error)

let showAsserts (dep: Dependency, result: ExecutionResult) =
    match result.FailedAsserts with
    | [||]          -> ()
    | failedAsserts -> failedAsserts
                       |> Array.map AppError.create
                       |> showErrors dep                           

let run (dep: Dependency) (context: NBomberContext) =
    asyncResult {        
        dep.Logger.Information("NBomber '{0}' started a new session: '{1}'", dep.NBomberVersion, dep.SessionId)

        let! ctx = Validation.validateContext(context)
        let! nodeStats =
            match NBomberContext.tryGetClusterSettings(ctx) with
            | Some (Coordinator c) -> runClusterCoordinator(dep, ctx, c)
            | Some (Agent a)       -> runClusterAgent(dep, ctx, a)
            | None                 -> runSingleNode(dep, ctx)

        let result = nodeStats
                     |> ExecutionResult.init
                     |> applyAsserts ctx

        result |> buildReport dep |> saveReport dep ctx
        return result
    }
    |> Async.RunSynchronously
    |> Result.map(fun result -> showAsserts(dep, result)
                                result)
    |> Result.mapError(fun error -> showErrors dep [|error|]
                                    error)

let runAs (appType: ApplicationType) (context: NBomberContext) =    
    let nodeType = NBomberContext.getNodeType(context)
    let dep = Dependency.create(appType, nodeType, context.InfraConfig)
    let dep = { dep with StatisticsSink = context.StatisticsSink }
    run dep context