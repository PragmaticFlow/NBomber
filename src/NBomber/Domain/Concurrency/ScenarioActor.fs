module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Step
open NBomber.Domain.Stats.ScenarioStatsActor

type ActorDep = {
    Logger: ILogger
    CancellationToken: CancellationToken
    ScenarioGlobalTimer: Stopwatch
    Scenario: Scenario
    ScenarioStatsActor: IScenarioStatsActor
    ExecStopCommand: StopCommand -> unit
}

type ScenarioActor(dep: ActorDep, scenarioInfo: ScenarioInfo) =

    let _logger = dep.Logger.ForContext<ScenarioActor>()
    let mutable _working = false

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

    let execSteps (runInfinite: bool) = task {
        try
            if not _working then
                let mutable shouldRun = true
                _working <- true
                do! Task.Yield()

                while shouldRun && _working && not dep.CancellationToken.IsCancellationRequested do

                    _stepDep.Data.Clear()

                    try
                        let stepsOrder = Scenario.getStepOrder dep.Scenario
                        do! RunningStep.execSteps _stepDep _steps stepsOrder
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
            _working <- false
    }

    member _.ScenarioStatsActor = dep.ScenarioStatsActor
    member _.ScenarioInfo = scenarioInfo
    member _.Working = _working

    member _.ExecSteps() = execSteps false
    member _.RunInfinite() = execSteps true

    member _.Stop() =
        _working <- false
