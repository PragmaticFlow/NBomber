module internal NBomber.DomainServices.NBomberRunner

open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.Step
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Reports
open NBomber.DomainServices.TestHost

let runSession (testInfo: TestInfo) (nodeInfo: NodeInfo) (context: NBomberContext) (dep: IGlobalDependency) =
    taskResult {
        Constants.Logo |> Console.addLogo |> Console.render

        dep.Logger.Information(Constants.NBomberWelcomeText, nodeInfo.NBomberVersion, testInfo.SessionId)
        dep.Logger.Information "NBomber started as single node"

        let! ctx = NBomberContext.EnterpriseValidation.validate dep context

        let! scenarios    = ctx |> NBomberContext.createScenarios
        let! sessionArgs  = ctx |> NBomberContext.createSessionArgs testInfo scenarios
        use testHost      = new TestHost(dep, scenarios, Scenario.getStepOrder, RunningStep.execSteps)
        let! result       = testHost.RunSession(sessionArgs)

        let! finalStats =
            Report.build dep.Logger result testHost.TargetScenarios
            |> Report.save dep ctx result.FinalStats

        return { result with FinalStats = finalStats }
    }

let run (context: NBomberContext) =

    let testInfo = {
        SessionId = Dependency.createSessionId()
        TestSuite = NBomberContext.getTestSuite context
        TestName = NBomberContext.getTestName context
    }

    let nodeInfo = NodeInfo.init None
    let appType = NodeInfo.getApplicationType()
    let reportFolder = NBomberContext.getReportFolderOrDefault testInfo.SessionId context

    Dependency.create reportFolder testInfo appType NodeType.SingleNode context
    |> runSession testInfo nodeInfo context
    |> TaskResult.mapError(fun error ->
        error |> AppError.toString |> Serilog.Log.Error
        error
    )
    |> fun task -> task.Result
