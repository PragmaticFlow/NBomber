module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Threading.Tasks
open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.ScenarioContext

type ScenarioActor(scnCtx: ScenarioContextArgs, scenarioInfo: ScenarioInfo) =

    let _logger = scnCtx.Logger.ForContext<ScenarioActor>()
    let _scenario = scnCtx.Scenario
    let _cancelToken = scnCtx.ScenarioCancellationToken.Token
    let mutable _working = false

    let _scenarioCtx = ScenarioContext(scnCtx, scenarioInfo)

    let execSteps (runInfinite: bool) = backgroundTask {
        try
            if not _working then
                let mutable shouldRun = true
                _working <- true
                do! Task.Yield()

                while shouldRun && _working && not _cancelToken.IsCancellationRequested do

                    if _scenario.Run.IsSome then
                        _scenarioCtx.PrepareNextIteration()
                        do! Scenario.measure Constants.ScenarioGlobalInfo _scenarioCtx _scenario.Run.Value

                    shouldRun <- runInfinite
            else
                _logger.Fatal($"Unhandled exception: ExecSteps was invoked for already working actor with Scenario: {0}", _scenario.ScenarioName)
        finally
            _working <- false
    }

    member _.ScenarioStatsActor = scnCtx.ScenarioStatsActor
    member _.ScenarioInfo = scenarioInfo
    member _.Working = _working

    member _.ExecSteps() = execSteps false
    member _.RunInfinite() = execSteps true
    member _.Stop() = _working <- false
