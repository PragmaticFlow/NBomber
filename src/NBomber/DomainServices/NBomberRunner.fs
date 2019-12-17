module internal NBomber.DomainServices.NBomberRunner

open System

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
open NBomber.DomainServices.TestHost
open NBomber.DomainServices.Cluster.Agent
open NBomber.DomainServices.Cluster.Coordinator

type ExecutionResult = {     
    RawNodeStats: RawNodeStats[]
    Statistics: Statistics[]
    FailedAsserts: DomainError[]
} with
  static member init (testInfo: TestInfo) (nodeStats: RawNodeStats[]) =
      let stats = nodeStats |> Array.collect(Statistics.create)
      { RawNodeStats = nodeStats; Statistics = stats; FailedAsserts = Array.empty }

let runClusterCoordinator (dep: Dependency, testInfo: TestInfo,
                           context: NBomberTestContext, crdSettings: CoordinatorSettings) =
    asyncResult {
        sprintf "NBomber started as cluster coordinator: %A" crdSettings
        |> dep.Logger.Information

        let testArgs = TestSessionArgs.getFromContext(testInfo, context)
        let registeredScns = context.Scenarios |> Array.map(Scenario.create)
        
        use coordinator = new ClusterCoordinator()
        let! clusterStats = coordinator.RunSession(dep, registeredScns, crdSettings, testArgs)        
        return clusterStats
    }

let runClusterAgent (dep: Dependency, context: NBomberTestContext, agentSettings: AgentSettings) =
    asyncResult {
        sprintf "NBomber started as cluster agent: %A" agentSettings
        |> dep.Logger.Information
        
        let registeredScns = context.Scenarios |> Array.map(Scenario.create)
        
        use agent = new ClusterAgent()
        do! agent.StartListening(dep, registeredScns, agentSettings)
        return Array.empty<RawNodeStats>
    }

let runSingleNode (dep: Dependency, testInfo: TestInfo, context: NBomberTestContext) =
    asyncResult {
        dep.Logger.Information("NBomber started as single node")

        let scnArgs = TestSessionArgs.getFromContext(testInfo, context)
        let registeredScns = context.Scenarios |> Array.map(Scenario.create)

        use scnHost = new TestHost(dep, registeredScns)
        let! rawNodeStats = scnHost.RunSession(scnArgs)
        return [|rawNodeStats|]
    }

let applyAsserts (context: NBomberTestContext) (result: ExecutionResult) = 
    let errors = 
        context.Scenarios 
        |> Array.collect(fun x -> x.Assertions)
        |> Assertion.cast
        |> Assertion.apply(result.Statistics)
    { result with FailedAsserts = errors }

let buildReport (dep: Dependency) (result: ExecutionResult) =    
    Report.build(dep, result.RawNodeStats, result.FailedAsserts)

let saveReport (dep: Dependency) (testInfo: TestInfo) (context: NBomberTestContext) (report: ReportsContent) =
    let fileName = NBomberTestContext.getReportFileName(testInfo.SessionId, context)
    let formats = NBomberTestContext.getReportFormats(context)
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

let sendStartTestToReportingSink (dep: Dependency, testInfo: TestInfo) =
    try
        if dep.ReportingSink.IsSome then
            dep.ReportingSink.Value.StartTest(testInfo).Wait()
    with
    | ex -> dep.Logger.Error(ex, "ReportingSink.StartTest failed")
        
let sendSaveReportsToReportingSink (dep: Dependency) (testInfo: TestInfo) (reportFiles: ReportFile[]) =
    try
        if dep.ReportingSink.IsSome then
            dep.ReportingSink.Value.SaveReports(testInfo, reportFiles).Wait()
    with
    | ex -> dep.Logger.Error(ex, "ReportingSink.SaveReports failed")            

let sendFinishTestToReportingSink (dep: Dependency) (testInfo: TestInfo) =
    try
        if dep.ReportingSink.IsSome then
            dep.ReportingSink.Value.FinishTest(testInfo).Wait()
    with
    | ex -> dep.Logger.Error(ex, "ReportingSink.FinishTest failed")

let run (dep: Dependency, testInfo: TestInfo, context: NBomberTestContext) =
    asyncResult {        
        dep.Logger.Information("NBomber '{0}' started a new session: '{1}'", dep.NBomberVersion, testInfo.SessionId)

        let! ctx = Validation.validateContext(context)
        
        sendStartTestToReportingSink(dep, testInfo)
        
        let! nodeStats =
            match NBomberTestContext.tryGetClusterSettings(ctx) with
            | Some (Coordinator c) -> runClusterCoordinator(dep, testInfo, ctx, c)
            | Some (Agent a)       -> runClusterAgent(dep, ctx, a)
            | None                 -> runSingleNode(dep, testInfo, ctx)

        let result = nodeStats
                     |> ExecutionResult.init(testInfo)
                     |> applyAsserts ctx

        result
        |> buildReport dep
        |> saveReport dep testInfo ctx
        |> sendSaveReportsToReportingSink dep testInfo
        |> fun () -> sendFinishTestToReportingSink dep testInfo
        
        return result
    }
    |> Async.RunSynchronously
    |> Result.map(fun result -> showAsserts(dep, result)
                                result)
    |> Result.mapError(fun error -> showErrors dep [|error|]
                                    error)

let runAs (appType: ApplicationType) (context: NBomberTestContext) =    
    let testInfo = {
        SessionId = Dependency.createSessionId()
        TestSuite = NBomberTestContext.getTestSuite(context)
        TestName = NBomberTestContext.getTestName(context)
    }
    
    let nodeType = NBomberTestContext.getNodeType(context)    
    
    let dep = Dependency.create(appType, nodeType, testInfo, context.InfraConfig)
    let dep = { dep with ReportingSink = context.ReportingSink }
    run(dep, testInfo, context)