module internal NBomber.DomainServices.NBomberRunner

open System

open System.Threading.Tasks
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.HintsAnalyzer
open NBomber.Domain.Stats
open NBomber.DomainServices.Reporting
open NBomber.DomainServices.TestHost
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Dependency

let getApplicationType () =
    try
        if Console.WindowHeight <= 0 then ApplicationType.Process
        else ApplicationType.Console
    with
    | _ -> ApplicationType.Process

let saveReports (dep: IGlobalDependency) (context: NBomberContext) (stats: NodeStats) (report: Report.ReportsContent) =
    let fileName     = NBomberContext.getReportFileName context
    let currentTime  = DateTime.UtcNow.ToString "yyyy-MM-dd--HH-mm-ss"
    let fileNameDate = $"{fileName}_{currentTime}"
    let folder       = NBomberContext.getReportFolder context
    let formats      = NBomberContext.getReportFormats context

    if formats.Length > 0 then
        let reportFiles = Report.save(folder, fileNameDate, formats, report, dep.Logger, stats.TestInfo)
        let finalStats = { stats with ReportFiles = reportFiles }
        dep.ReportingSinks
        |> List.map(fun x -> x.SaveFinalStats [| finalStats |])
        |> Task.WhenAll
        |> fun t -> t.Wait()

        finalStats
    else
        stats

let getLoadSimulations (context: NBomberContext) =
    context.RegisteredScenarios
    |> Seq.map(fun scn -> scn.ScenarioName, scn.LoadSimulations)
    |> dict

let runSession (testInfo: TestInfo) (nodeInfo: NodeInfo) (context: NBomberContext) (dep: IGlobalDependency) =
    taskResult {
        Constants.Logo |> Console.addLogo |> Console.render

        dep.Logger.Information(Constants.NBomberWelcomeText, nodeInfo.NBomberVersion, testInfo.SessionId)
        dep.Logger.Information("NBomber started as single node.")

        let! sessionArgs  = context |> NBomberContext.createSessionArgs(testInfo)
        let! scenarios    = context |> NBomberContext.createScenarios
        use testHost      = new TestHost(dep, scenarios, sessionArgs, ScenarioStatsActor.create)
        let! result       = testHost.RunSession()
        let simulations   = context |> getLoadSimulations
        let reports       = Report.build result simulations

        if dep.ApplicationType = ApplicationType.Console then
            reports.ConsoleReport |> Seq.iter Console.render

        return reports |> saveReports dep context result.NodeStats
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

    let reportFolder = NBomberContext.getReportFolder(context)

    Dependency.create reportFolder testInfo appType NodeType.SingleNode context
    |> runSession testInfo nodeInfo context
    |> TaskResult.mapError(fun error ->
        error |> AppError.toString |> Serilog.Log.Error
        error
    )
    |> fun task -> task.Result
