module internal NBomber.DomainServices.NBomberRunner

open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Reporting
open NBomber.DomainServices.Reporting.Report
open NBomber.DomainServices.TestHost
open NBomber.DomainServices.TestHost.Infra

let runSingleNode (dep: GlobalDependency, testInfo: TestInfo, context: NBomberContext) =
    taskResult {
        dep.Logger.Information("NBomber started as single node")

        let scnArgs = SessionArgs.getFromContext(testInfo, context)
        let! registeredScns = context.RegisteredScenarios |> Scenario.createScenarios

        use scnHost = new TestHost(dep, registeredScns)
        return! scnHost.RunSession(scnArgs)
    }

let saveReports (dep: GlobalDependency) (testInfo: TestInfo) (context: NBomberContext) (report: ReportsContent) =
    let fileName = NBomberContext.getReportFileName(testInfo.SessionId, context)
    let formats = NBomberContext.getReportFormats(context)
    Report.save("./", fileName, formats, report, dep.Logger)

let startPlugins (dep: GlobalDependency) (testInfo: TestInfo) =
    for plugin in dep.Plugins do
        try
            plugin.StartTest(testInfo).Wait()
        with
        | ex -> dep.Logger.Error(ex, "Plugin '{PluginName}' failed", plugin.PluginName)

let stopPlugins (dep: GlobalDependency) =
    for plugin in dep.Plugins do
        try
            plugin.StopTest().Wait()
            plugin.Dispose()
        with
        | ex -> dep.Logger.Error(ex, "Plugin '{PluginName}' failed", plugin.PluginName)

let startReportingSinks (dep: GlobalDependency) (testInfo: TestInfo) =
    for sink in dep.ReportingSinks do
        try
            sink.StartTest(testInfo) |> ignore
            sink.Dispose()
        with
        | ex -> dep.Logger.Error(ex, "ReportingSink '{SinkName}' failed", sink.SinkName)

let saveFinalStats (dep: GlobalDependency) (stats: NodeStats[]) (reportFiles: ReportFile[]) =
    for sink in dep.ReportingSinks do
        try
            sink.SaveFinalStats(stats, reportFiles).Wait()
        with
        | ex -> dep.Logger.Error(ex, "ReportingSink '{SinkName}' failed", sink.SinkName)

let stopReportingSink (dep: GlobalDependency) =
    for sink in dep.ReportingSinks do
        try
            sink.StopTest().Wait()
        with
        | ex -> dep.Logger.Error(ex, "ReportingSink '{SinkName}' failed", sink.SinkName)

let run (dep: GlobalDependency, testInfo: TestInfo, context: NBomberContext) =
    taskResult {
        dep.Logger.Information(Constants.NBomberWelcomeText, dep.NBomberVersion, testInfo.SessionId)

        let! ctx = Validation.validateContext(context)

        startPlugins dep testInfo
        startReportingSinks dep testInfo

        let! nodeStats = runSingleNode(dep, testInfo, ctx)

        nodeStats
        |> Report.build
        |> saveReports dep testInfo ctx
        |> saveFinalStats dep [|nodeStats|]

        stopPlugins dep
        stopReportingSink dep

        return nodeStats
    }
    |> TaskResult.mapError(fun error ->
        error |> AppError.toString |> dep.Logger.Error
        error
    )
    |> fun task -> task.Result

let runAs (appType: ApplicationType, context: NBomberContext) =

    let testInfo = {
        SessionId = Dependency.createSessionId()
        TestSuite = NBomberContext.getTestSuite(context)
        TestName = NBomberContext.getTestName(context)
    }

    let nodeType = NodeType.SingleNode

    let dep = Dependency.create(appType, nodeType, testInfo, context.InfraConfig)
    let dep = { dep with ReportingSinks = context.ReportingSinks; Plugins = context.Plugins }
    dep.Plugins |> List.iter(fun x -> x.Init(dep.Logger, context.InfraConfig))
    dep.ReportingSinks |> List.iter(fun x -> x.Init(dep.Logger, context.InfraConfig))

    run(dep, testInfo, context)
