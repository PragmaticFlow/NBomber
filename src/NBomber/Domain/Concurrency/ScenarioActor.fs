module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Domain.DomainTypes
open NBomber.Domain.Step
open NBomber.Domain.Stats.ScenarioStatsActor

type ActorDep = {
    Logger: ILogger
    CancellationToken: CancellationToken
    ScenarioGlobalTimer: Stopwatch
    Scenario: Scenario
    ScenarioStatsActor: ScenarioStatsActor
    ExecStopCommand: StopCommand -> unit
    GetStepOrder: Scenario -> int[]
    ExecSteps: StepDep -> RunningStep[] -> int[] -> Task<unit> // stepDep steps stepsOrder
}

type ScenarioActor(dep: ActorDep, scenarioInfo: ScenarioInfo) =

    let _logger = dep.Logger.ForContext<ScenarioActor>()
    let mutable _actorWorking = false

    let _stepDep = {
        Scenario = dep.Scenario
        ScenarioInfo = scenarioInfo
        Logger = dep.Logger
        CancellationToken = dep.CancellationToken
        ScenarioGlobalTimer = dep.ScenarioGlobalTimer
        ExecStopCommand = dep.ExecStopCommand
        ScenarioStatsActor = dep.ScenarioStatsActor
        Data = Dictionary<string,obj>()
    }

    let _steps =
        dep.Scenario.Steps
        |> List.map(fun step -> RunningStep.create _stepDep dep.Scenario.StepOrderIndex[step.StepName] step)
        |> List.toArray

    let execSteps (runInfinite: bool) = backgroundTask {
        try
            if not _actorWorking then
                let mutable shouldRun = true
                _actorWorking <- true

                while shouldRun && _actorWorking
                    && not dep.CancellationToken.IsCancellationRequested
                    && dep.Scenario.PlanedDuration.TotalMilliseconds > dep.ScenarioGlobalTimer.Elapsed.TotalMilliseconds do

                    _stepDep.Data.Clear()

                    try
                        let stepsOrder = dep.GetStepOrder dep.Scenario
                        do! dep.ExecSteps _stepDep _steps stepsOrder
                    with
                    | ex ->
                        _logger.Error(ex, $"Unhandled exception for Scenario: {dep.Scenario.ScenarioName}")
                        let response = Response.fail(statusCode = Constants.StepInternalClientErrorCode, error = ex.Message)
                        let resp = { StepIndex = 0; ClientResponse = response; EndTimeMs = 0; LatencyMs = 0 }
                        dep.ScenarioStatsActor.Publish(AddResponse resp)

                    shouldRun <- runInfinite
            else
                _logger.Error($"ExecSteps was invoked for already working actor with Scenario: {dep.Scenario.ScenarioName}")
        finally
            _actorWorking <- false
    }

    member _.ScenarioStatsActor = dep.ScenarioStatsActor
    member _.ScenarioInfo = scenarioInfo
    member _.Working = _actorWorking

    member _.ExecSteps() = execSteps false
    member _.RunInfinite() = execSteps true

    member _.Stop() =
        _actorWorking <- false
