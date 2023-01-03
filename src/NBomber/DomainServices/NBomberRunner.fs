module internal NBomber.DomainServices.NBomberRunner

open FsToolkit.ErrorHandling
open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Logger
open NBomber.DomainServices.Reports
open NBomber.DomainServices.TestHost

let runSession (testInfo: TestInfo) (nodeInfo: NodeInfo) (context: NBomberContext) (dep: IGlobalDependency) =
    taskResult {
        Constants.Logo |> Console.addLogo |> Console.render

        dep.LogInfo(Constants.NBomberWelcomeText, nodeInfo.NBomberVersion, testInfo.SessionId)
        dep.LogInfo "NBomber started as single node"

        let! scenarios = context |> NBomberContext.createScenarios
        let! sessionArgs = context |> NBomberContext.createSessionArgs testInfo scenarios
        use testHost = new TestHost(dep, scenarios)
        let! result = testHost.RunSession(sessionArgs)

        let finalStats =
            Report.build dep.Logger result testHost.TargetScenarios
            |> Report.save dep context result.FinalStats

        do! ReportingSinks.saveFinalStats dep finalStats

        return { result with FinalStats = finalStats }
    }

let run (context: NBomberContext) =

    let testInfo = {
        SessionId = Dependency.createSessionId()
        TestSuite = NBomberContext.getTestSuite context
        TestName = NBomberContext.getTestName context
        ClusterId = ""
    }

    let nodeInfo = NodeInfo.init None
    let appType = NodeInfo.getApplicationType()
    let reportFolder = NBomberContext.getReportFolderOrDefault testInfo.SessionId context

    let logSettings = {
        Folder = reportFolder
        TestInfo = testInfo
        NodeType = NodeType.SingleNode
        AgentGroup = ""
    }

    let dep = Dependency.create appType logSettings context

    runSession testInfo nodeInfo context dep
    |> TaskResult.mapError(fun error ->
        error |> AppError.toString |> dep.LogError
        error
    )
    |> fun task -> task.GetAwaiter().GetResult()
