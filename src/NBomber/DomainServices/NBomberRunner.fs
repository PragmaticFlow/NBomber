module internal NBomber.DomainServices.NBomberRunner

open System.Threading.Tasks

open FsToolkit.ErrorHandling

open NBomber.Extensions
open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.StatisticsTypes
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Reporting
open NBomber.DomainServices.Reporting.Report
open NBomber.DomainServices.TestHost
open NBomber.DomainServices.TestHost.Infra

type ExecutionResult = {
    RawNodeStats: RawNodeStats[]
    Statistics: Statistics[]
    ExtensionStatistics: ExtensionStatistics[]
} with
  static member init (nodeStats: RawNodeStats list, extensionStats: ExtensionStatistics list) =
      { RawNodeStats = nodeStats |> List.toArray
        Statistics = nodeStats |> Seq.collect(Statistics.create) |> Seq.toArray
        ExtensionStatistics = extensionStats |> List.toArray }

let runSingleNode (dep: GlobalDependency, testInfo: TestInfo, context: TestContext) =
    asyncResult {
        dep.Logger.Information("NBomber started as single node")

        let scnArgs = TestSessionArgs.getFromContext(testInfo, context)
        let! registeredScns = context.RegisteredScenarios |> Scenario.createScenarios

        use scnHost = new TestHost(dep, registeredScns)
        return! scnHost.RunSession(scnArgs)
    }

let buildReport (dep: GlobalDependency) (result: ExecutionResult) =
    Report.build(dep, result.RawNodeStats, result.ExtensionStatistics)

let saveReport (dep: GlobalDependency) (testInfo: TestInfo) (context: TestContext) (report: ReportsContent) =
    let fileName = TestContext.getReportFileName(testInfo.SessionId, context)
    let formats = TestContext.getReportFormats(context)
    Report.save("./", fileName, formats, report, dep.Logger)

let sendStartTestToExtension (dep: GlobalDependency, testInfo: TestInfo, context: TestContext) =
    try
        for extension in dep.Extensions do
            extension.StartTest(testInfo) |> ignore
    with
    | ex -> dep.Logger.Error(ex, "Extension.StartTest failed")

let sendGetStatsToExtension (dep: GlobalDependency, testInfo: TestInfo) =
    try
        dep.Extensions
        |> List.map(fun ext -> ext.GetStats(testInfo))
        |> Task.WhenAll
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> List.ofSeq
    with
    | ex -> dep.Logger.Error(ex, "Extension.GetStats failed")
            List.empty

let sendFinishTestToExtension (dep: GlobalDependency, testInfo: TestInfo) =
    try
        dep.Extensions
        |> List.map(fun ext -> ext.FinishTest(testInfo))
        |> Task.WhenAll
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore
    with
    | ex -> dep.Logger.Error(ex, "Extension.FinishTest failed")

let sendStartTestToReportingSink (dep: GlobalDependency, testInfo: TestInfo) =
    try
        for sink in dep.ReportingSinks do
            sink.StartTest(testInfo) |> ignore
    with
    | ex -> dep.Logger.Error(ex, "ReportingSink.StartTest failed")

let sendSaveReportsToReportingSink (dep: GlobalDependency) (testInfo: TestInfo)
                                   (stats: Statistics[]) (extensionStats: ExtensionStatistics[])
                                   (reportFiles: ReportFile[]) =
    try
        dep.ReportingSinks
        |> List.map(fun sink -> sink.SaveFinalStats(testInfo, stats, extensionStats, reportFiles))
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
        dep.Logger.Information(ResourceManager.Constants.NBomberWelcomeText, dep.NBomberVersion, testInfo.SessionId)

        let! ctx = Validation.validateContext(context)

        sendStartTestToExtension(dep, testInfo, context)
        sendStartTestToReportingSink(dep, testInfo)

        let! nodeStats = runSingleNode(dep, testInfo, ctx)
        let extensionStats = sendGetStatsToExtension(dep, testInfo)
        let result = ExecutionResult.init(nodeStats, extensionStats)

        result
        |> buildReport dep
        |> saveReport dep testInfo ctx
        |> sendSaveReportsToReportingSink dep testInfo result.Statistics result.ExtensionStatistics
        |> fun () -> sendFinishTestToReportingSink dep testInfo

        sendFinishTestToExtension(dep, testInfo)

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
    let dep = { dep with ReportingSinks = context.ReportingSinks; Extensions = context.Extensions }
    dep.Extensions |> List.iter(fun x -> x.Init(dep.Logger, context.InfraConfig))
    dep.ReportingSinks |> List.iter(fun x -> x.Init(dep.Logger, context.InfraConfig))

    run(dep, testInfo, context)
