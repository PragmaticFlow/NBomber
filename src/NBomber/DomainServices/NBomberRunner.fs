module internal NBomber.DomainServices.NBomberRunner

open System.Threading.Tasks

open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain
open NBomber.Errors
open NBomber.Extensions
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Reporting
open NBomber.DomainServices.Reporting.Report
open NBomber.DomainServices.TestHost
open NBomber.DomainServices.TestHost.Infra

type ExecutionResult = {
    RawNodeStats: RawNodeStats[]
    Statistics: Statistics[]
} with
  static member init (nodeStats: RawNodeStats list) =
      { RawNodeStats = nodeStats |> List.toArray
        Statistics = nodeStats |> Seq.collect(Statistics.create) |> Seq.toArray }

let runSingleNode (dep: GlobalDependency, testInfo: TestInfo, context: TestContext) =
    asyncResult {
        dep.Logger.Information("NBomber started as single node")

        let scnArgs = TestSessionArgs.getFromContext(testInfo, context)
        let! registeredScns = context.RegisteredScenarios |> Scenario.createScenarios

        use scnHost = new TestHost(dep, registeredScns)
        return! scnHost.RunSession(scnArgs)
    }

let buildReport (dep: GlobalDependency) (result: ExecutionResult) =
    Report.build(dep, result.RawNodeStats)

let saveReport (dep: GlobalDependency) (testInfo: TestInfo) (context: TestContext) (report: ReportsContent) =
    let fileName = TestContext.getReportFileName(testInfo.SessionId, context)
    let formats = TestContext.getReportFormats(context)
    Report.save("./", fileName, formats, report, dep.Logger)

let sendStartTestToReportingSink (dep: GlobalDependency, testInfo: TestInfo) =
    try
        for sink in dep.ReportingSinks do
            sink.StartTest(testInfo) |> ignore
    with
    | ex -> dep.Logger.Error(ex, "ReportingSink.StartTest failed")

let sendSaveReportsToReportingSink (dep: GlobalDependency) (testInfo: TestInfo) (stats: Statistics[]) (reportFiles: ReportFile[]) =
    try
        dep.ReportingSinks
        |> List.map(fun sink -> sink.SaveFinalStats(testInfo, stats, reportFiles))
        |> Task.WhenAll
        |> Async.AwaitTask
        |> Async.RunSynchronously
    with
    | ex -> dep.Logger.Error(ex, "ReportingSink.SaveReports failed")

let sendFinishTestToReportingSink (dep: GlobalDependency) (testInfo: TestInfo) =
    try
        dep.ReportingSinks
        |> List.map(fun sink -> sink.FinishTest testInfo)
        |> Task.WhenAll
        |> Async.AwaitTask
        |> Async.RunSynchronously
    with
    | ex -> dep.Logger.Error(ex, "ReportingSink.FinishTest failed")

let run (dep: GlobalDependency, testInfo: TestInfo, context: TestContext) =
    asyncResult {
        dep.Logger.Information("NBomber '{0}' started a new session: '{1}'", dep.NBomberVersion, testInfo.SessionId)

        let! ctx = Validation.validateContext(context)

        sendStartTestToReportingSink(dep, testInfo)

        let! nodeStats = runSingleNode(dep, testInfo, ctx)

        let result = ExecutionResult.init(nodeStats)

        result
        |> buildReport dep
        |> saveReport dep testInfo ctx
        |> sendSaveReportsToReportingSink dep testInfo result.Statistics
        |> fun () -> sendFinishTestToReportingSink dep testInfo

        return result
    }
    |> Async.RunSynchronously
    |> Result.mapError(fun error ->
        error |> AppError.toString |> dep.Logger.Error
        error
    )

let runAs (appType: ApplicationType, context: TestContext) =

    let testInfo = {
        SessionId = Dependency.createSessionId()
        TestSuite = TestContext.getTestSuite(context)
        TestName = TestContext.getTestName(context)
    }

    let nodeType = NodeType.SingleNode

    let dep = Dependency.create(appType, nodeType, testInfo, context.InfraConfig)
    let dep = { dep with ReportingSinks = context.ReportingSinks }
    dep.ReportingSinks |> List.iter(fun x -> x.Init(dep.Logger, context.InfraConfig))

    run(dep, testInfo, context)
