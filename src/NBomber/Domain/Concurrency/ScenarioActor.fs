module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Collections.Generic

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.ScenarioContext
open NBomber.Domain.Step
open NBomber.Domain.Stats.ScenarioStatsActor

type ScenarioActor(scnCtx: ScenarioExecContext, scenarioInfo: ScenarioInfo) =

    let _logger = scnCtx.Logger.ForContext<ScenarioActor>()
    let _scenario = scnCtx.Scenario
    let mutable _actorWorking = false

    let _scenarioCtx = ScenarioContext(scenarioInfo, scnCtx)

    let _stepDep = {
        ScenarioExecContext = scnCtx
        ScenarioInfo = scenarioInfo
        Data = Dictionary<string,obj>()
    }

    let _steps =
        _scenario.Steps
        |> List.map(fun step -> RunningStep.create _stepDep _scenario.StepOrderIndex[step.StepName] step)
        |> List.toArray

    let execSteps (runInfinite: bool) = backgroundTask {
        try
            if not _actorWorking then
                let mutable shouldRun = true
                _actorWorking <- true

                while shouldRun && _actorWorking
                    && not scnCtx.ScenarioCancellationToken.IsCancellationRequested
                    && _scenario.PlanedDuration.TotalMilliseconds > scnCtx.ScenarioTimer.Elapsed.TotalMilliseconds do

                    // _stepDep.Data.Clear()

                    if _scenario.Run.IsSome then
                        do! Scenario.measure Constants.ScenarioGlobalInfo _scenarioCtx _scenario.Run.Value

                        // let stepOrder = Scenario.getStepOrder _scenario
                        // do! RunningStep.execSteps _stepDep _steps stepOrder
                    // with
                    // | ex ->
                    //     _logger.Error(ex, "Unhandled exception for Scenario: {0}", _scenario.ScenarioName)
                        // let response = Response.fail(statusCode = Constants.StepInternalClientErrorCode, error = ex.Message)
                        // let resp = { StepIndex = 0; ClientResponse = response; EndTimeMs = 0; LatencyMs = 0 }
                        // scnCtx.ScenarioStatsActor.Publish(AddResponse resp)

                    shouldRun <- runInfinite
            else
                _logger.Error($"ExecSteps was invoked for already working actor with Scenario: {_scenario.ScenarioName}")
        finally
            _actorWorking <- false
    }

    member _.ScenarioStatsActor = scnCtx.ScenarioStatsActor
    member _.ScenarioInfo = scenarioInfo
    member _.Working = _actorWorking

    member _.ExecSteps() = execSteps false
    member _.RunInfinite() = execSteps true
    member _.Stop() = _actorWorking <- false
