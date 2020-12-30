module internal NBomber.DomainServices.NBomberRunner

open System

open System.Threading.Tasks
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices
open NBomber.DomainServices.Reporting
open NBomber.DomainServices.Reporting.Report
open NBomber.DomainServices.TestHost

let getApplicationType () =
    try
        if Console.WindowHeight <= 0 then ApplicationType.Process
        else ApplicationType.Console
    with
    | _ -> ApplicationType.Process

let saveReports (dep: IGlobalDependency) (context: NBomberContext) (stats: NodeStats) (report: ReportsContent) =
    let fileName     = NBomberContext.getReportFileName context
    let folder       = NBomberContext.getReportFolder context
    let fileNameDate = sprintf "%s_%s" fileName (DateTime.UtcNow.ToString "yyyy-MM-dd--HH-mm-ss")
    let formats      = NBomberContext.getReportFormats context

    if formats.Length > 0 then
        let reportFiles = Report.save(folder, fileNameDate, formats, report, dep.Logger, stats.TestInfo)
        dep.ReportingSinks
        |> List.map(fun x -> x.SaveReports reportFiles)
        |> Task.WhenAll
        |> fun t -> t.Wait()

        { stats with ReportFiles = reportFiles }
    else
        stats

let getHints (context: NBomberContext) (stats: NodeStats) =
    if context.UseHintsAnalyzer then
        context.WorkerPlugins
        |> Seq.collect(fun x -> x.GetHints())
        |> Seq.append(HintsAnalyzer.analyze stats)
        |> Seq.toList

    else List.empty

let runSession (testInfo: TestInfo) (nodeInfo: NodeInfo) (context: NBomberContext) (dep: IGlobalDependency) =
    taskResult {
        dep.Logger.Information(Constants.NBomberWelcomeText, nodeInfo.NBomberVersion, testInfo.SessionId)
        dep.Logger.Information("NBomber started as single node")

        let! sessionArgs  = context |> NBomberContext.createSessionArgs(testInfo)
        let! scenarios    = context |> NBomberContext.createScenarios
        use testHost      = new TestHost(dep, scenarios, sessionArgs)
        let! nodeStats    = testHost.RunSession()
        let timeLineStats = testHost.GetTimeLineNodeStats()
        let hints         = getHints context nodeStats

        return Report.build nodeStats timeLineStats
               |> saveReports dep context nodeStats
    }

let run (context: NBomberContext) =

    let testInfo = {
        SessionId = Dependency.createSessionId()
        TestSuite = NBomberContext.getTestSuite(context)
        TestName = NBomberContext.getTestName(context)
    }

    let nodeInfo = NodeInfo.init()

    let appType =
        match context.ApplicationType with
        | Some appType -> appType
        | None         -> getApplicationType()

    Dependency.create appType NodeType.SingleNode context
    |> Dependency.init testInfo
    |> runSession testInfo nodeInfo context
    |> TaskResult.mapError(fun error ->
        error |> AppError.toString |> Serilog.Log.Error
        error
    )
    |> fun task -> task.Result
