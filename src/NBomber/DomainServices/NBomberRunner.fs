﻿module internal NBomber.DomainServices.NBomberRunner

open System
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices
open NBomber.DomainServices.Reporting
open NBomber.DomainServices.Reporting.Report
open NBomber.DomainServices.TestHost

let saveReports (dep: IGlobalDependency) (context: NBomberContext) (report: ReportsContent) =
    let fileName     = NBomberContext.getReportFileName(context)
    let fileNameDate = sprintf "%s_%s" fileName (DateTime.UtcNow.ToString "yyyy-dd-M--HH-mm-ss")
    let formats      = NBomberContext.getReportFormats(context)
    Report.save("./", fileNameDate, formats, report, dep.Logger) |> ignore

let runSession (testInfo: TestInfo) (context: NBomberContext) (dep: IGlobalDependency) =
    taskResult {
        dep.Logger.Information(Constants.NBomberWelcomeText, dep.NBomberVersion, testInfo.SessionId)
        dep.Logger.Information("NBomber started as single node")

        let! sessionArgs = NBomberContext.createSessionArgs testInfo context
        let! scenarios   = Scenario.createScenarios(context.RegisteredScenarios)
        use testHost     = new TestHost(dep, scenarios, sessionArgs)
        let! nodeStats   = testHost.RunSession()
        let timeLineStats = testHost.GetTimeLineNodeStats()

        (nodeStats, timeLineStats) |> Report.build |> saveReports dep context
        return nodeStats
    }

let run (appType: ApplicationType) (context: NBomberContext) =

    let testInfo = {
        SessionId = Dependency.createSessionId()
        TestSuite = NBomberContext.getTestSuite(context)
        TestName = NBomberContext.getTestName(context)
    }

    Dependency.create appType NodeType.SingleNode context
    |> Dependency.init(testInfo)
    |> runSession testInfo context
    |> TaskResult.mapError(fun error ->
        error |> AppError.toString |> Serilog.Log.Error
        error
    )
    |> fun task -> task.Result
