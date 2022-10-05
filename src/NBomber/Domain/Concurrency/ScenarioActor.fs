module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Collections.Generic
open System.Threading.Tasks

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Step
open NBomber.Domain.Stats.ScenarioStatsActor

type ActorDep = {
    ScenarioDep: ScenarioDep
    ExecSteps: StepDep -> RunningStep[] -> int[] -> Task<unit> // stepDep steps stepsOrder
}

type ScenarioActor(dep: ActorDep, scenarioInfo: ScenarioInfo) =

    let _logger = dep.ScenarioDep.Logger.ForContext<ScenarioActor>()
    let _scnDep = dep.ScenarioDep
    let _scenario = dep.ScenarioDep.Scenario
    let mutable _actorWorking = false

    let _stepDep = {
        ScenarioDep = _scnDep
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
                    && not _scnDep.ScenarioCancellationToken.IsCancellationRequested
                    && _scenario.PlanedDuration.TotalMilliseconds > _scnDep.ScenarioTimer.Elapsed.TotalMilliseconds do

                    _stepDep.Data.Clear()

                    try
                        let stepOrder = Scenario.getStepOrder _scenario
                        do! dep.ExecSteps _stepDep _steps stepOrder
                    with
                    | ex ->
                        _logger.Error(ex, $"Unhandled exception for Scenario: {_scenario.ScenarioName}")
                        let response = Response.fail(statusCode = Constants.StepInternalClientErrorCode, error = ex.Message)
                        let resp = { StepIndex = 0; ClientResponse = response; EndTimeMs = 0; LatencyMs = 0 }
                        _scnDep.ScenarioStatsActor.Publish(AddResponse resp)

                    shouldRun <- runInfinite
            else
                _logger.Error($"ExecSteps was invoked for already working actor with Scenario: {_scenario.ScenarioName}")
        finally
            _actorWorking <- false
    }

    member _.ScenarioStatsActor = _scnDep.ScenarioStatsActor
    member _.ScenarioInfo = scenarioInfo
    member _.Working = _actorWorking

    member _.ExecSteps() = execSteps false
    member _.RunInfinite() = execSteps true
    member _.Stop() = _actorWorking <- false
