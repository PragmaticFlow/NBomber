module internal NBomber.DomainServices.NBomberRunner

open System

open FsToolkit.ErrorHandling
open ShellProgressBar

open NBomber
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Errors
open NBomber.Extensions
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

let private printNBomberConfig (context: NBomberContext) (dep: IGlobalDependency) =

    let printGlobalSettings (globalSettings: GlobalSettings) (dep: IGlobalDependency) =
        let scenariosSettings = Option.toStringOrEmpty(globalSettings.ScenariosSettings)

        let connectionPoolSettings =
            globalSettings.ConnectionPoolSettings
            |> Option.bind(Seq.ofList >> Some)
            |> Option.toStringSeqOrEmpty
            |> String.concatWithCommaAndQuotes

        let reportFileName = Option.toStringOrEmpty(globalSettings.ReportFileName)

        let reportFormats =
            globalSettings.ReportFormats
            |> Option.bind(Seq.ofList >> Some)
            |> Option.toStringSeqOrEmpty
            |> String.concatWithCommaAndQuotes

        let sendStatsInterval = Option.toStringOrEmpty(globalSettings.SendStatsInterval)

        dep.Logger.Verbose("GlobalSettings.ScenariosSettings: {scenariosSettings}", scenariosSettings)
        dep.Logger.Verbose("GlobalSettings.ConnectionPoolSettings: {connectionPoolSettings}", connectionPoolSettings)
        dep.Logger.Verbose("GlobalSettings.ReportFileName: {reportFileName}", reportFileName)
        dep.Logger.Verbose("GlobalSettings.ReportFormats: {reportFormats}", reportFormats)
        dep.Logger.Verbose("GlobalSettings.SendStatsInterval: {sendStatsInterval}", sendStatsInterval)

    match context.NBomberConfig with
    | Some config ->
        let testSuite = Option.toStringOrEmpty(config.TestSuite)
        let testName = Option.toStringOrEmpty(config.TestName)

        let targetScenarios =
            config.TargetScenarios
            |> Option.bind(Seq.ofList >> Some)
            |> Option.toStringSeqOrEmpty
            |> String.concatWithCommaAndQuotes

        dep.Logger.Verbose("TestSuite: {TestSuite}", testSuite)
        dep.Logger.Verbose("TestName: {TestName}", testName)
        dep.Logger.Verbose("TargetScenarios: {TargetScenarios}", targetScenarios)

        match config.GlobalSettings with
        | Some gs -> printGlobalSettings gs dep
        | None    -> ()

    | None        -> ()

let runSession (testInfo: TestInfo) (context: NBomberContext) (dep: IGlobalDependency) =
    taskResult {
        dep.Logger.Information(Constants.NBomberWelcomeText, dep.NBomberVersion, testInfo.SessionId)
        dep.Logger.Information("NBomber started as single node")

        printNBomberConfig context dep

        let! sessionArgs = context |> NBomberContext.createSessionArgs(testInfo)
        let! scenarios   = context |> NBomberContext.createScenarios
        use testHost     = new TestHost(dep, scenarios, sessionArgs)
        let! nodeStats   = testHost.RunSession()
        let timeLineStats = testHost.GetTimeLineNodeStats()

        (nodeStats, timeLineStats) |> Report.build |> saveReports dep context
        return nodeStats
    }

let private getApplicationType () =
    try
        use pb = new ProgressBar(0, String.Empty)
        ApplicationType.Console
    with
    | _ -> ApplicationType.Process

let run (context: NBomberContext) =
    let testInfo = {
        SessionId = Dependency.createSessionId()
        TestSuite = NBomberContext.getTestSuite(context)
        TestName = NBomberContext.getTestName(context)
    }

    let applicationType =
        match context.ApplicationType with
        | Some apptype -> apptype
        | None         -> getApplicationType()

    Dependency.create applicationType NodeType.SingleNode context
    |> Dependency.init(testInfo)
    |> runSession testInfo context
    |> TaskResult.mapError(fun error ->
        error |> AppError.toString |> Serilog.Log.Error
        error
    )
    |> fun task -> task.Result
