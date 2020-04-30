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

let saveReports (dep: GlobalDependency) (testInfo: TestInfo) (context: NBomberContext) (report: ReportsContent) =
    let fileName = NBomberContext.getReportFileName(testInfo.SessionId, context)
    let formats = NBomberContext.getReportFormats(context)
    Report.save("./", fileName, formats, report, dep.Logger) |> ignore

let run (dep: GlobalDependency, testInfo: TestInfo, context: NBomberContext) =
    taskResult {
        dep.Logger.Information(Constants.NBomberWelcomeText, dep.NBomberVersion, testInfo.SessionId)
        dep.Logger.Information("NBomber started as single node")

        let! ctx = Validation.validateContext(context)
        let scnArgs = context |> SessionArgs.buildFromContext(testInfo)
        let! scenarios = context.RegisteredScenarios |> Scenario.createScenarios
        use testHost = new TestHost(dep, scenarios)
        let! nodeStats = testHost.RunSession(scnArgs)
        let timeLineStats = testHost.GetTimeLineNodeStats()

        (nodeStats, timeLineStats) |> Report.build |> saveReports dep testInfo ctx
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
    let dep = Dependency.init(appType, nodeType, testInfo, context)
    run(dep, testInfo, context)
