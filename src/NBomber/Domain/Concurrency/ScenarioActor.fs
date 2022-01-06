module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.NonAffine

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Step
open NBomber.Domain.Stats.GlobalScenarioStatsActor
open NBomber.Domain.Stats.LocalScenarioStatsActor

type ActorDep = {
    Logger: ILogger
    CancellationToken: CancellationToken
    ScenarioGlobalTimer: Stopwatch
    Scenario: Scenario
    GlobalScenarioStatsActor: GlobalScenarioStatsActor
    ExecStopCommand: StopCommand -> unit
}

type ScenarioActor(dep: ActorDep, scenarioInfo: ScenarioInfo) =

    let _logger = dep.Logger.ForContext<ScenarioActor>()
    let _isAllExecSync = Step.isAllExecSync dep.Scenario.Steps

    let _stepDep = {
        ScenarioInfo = scenarioInfo
        Logger = dep.Logger
        CancellationToken = dep.CancellationToken
        ScenarioGlobalTimer = dep.ScenarioGlobalTimer
        ExecStopCommand = dep.ExecStopCommand
        LocalScenarioStatsActor = LocalScenarioStatsActor(dep.GlobalScenarioStatsActor)
        Data = Dictionary<string,obj>()
    }

    let _steps = dep.Scenario.Steps |> List.map(RunningStep.create _stepDep) |> List.toArray
    let mutable _working = false

    let execSteps () = task {
        try
            if not _working then
                _working <- true
                do! Task.Yield()

                _stepDep.Data.Clear()

                if _isAllExecSync then
                    Step.execSteps(_stepDep, _steps, dep.Scenario.GetStepsOrder())
                else
                    do! Step.execStepsAsync(_stepDep, _steps, dep.Scenario.GetStepsOrder())
            else
                _logger.Error($"ExecSteps was invoked for already working actor with scenario '{dep.Scenario.ScenarioName}'.")
        finally
            _working <- false
    }

    let runInfinite () = task {
        try
            if not _working then
                _working <- true

                do! Task.Yield()
                while _working && not dep.CancellationToken.IsCancellationRequested do

                    _stepDep.Data.Clear()

                    if _isAllExecSync then
                        Step.execSteps(_stepDep, _steps, dep.Scenario.GetStepsOrder())
                    else
                        do! Step.execStepsAsync(_stepDep, _steps, dep.Scenario.GetStepsOrder())
            else
                _logger.Error($"RunInfinite was invoked for already working actor with scenario '{dep.Scenario.ScenarioName}'.")
        finally
            _working <- false
    }

    let flushStats () =
        _stepDep.LocalScenarioStatsActor.Publish(ActorMessage.FlushBuffer)

    member _.ScenarioInfo = scenarioInfo
    member _.Working = _working

    member _.ExecSteps() = execSteps()
    member _.RunInfinite() = runInfinite()
    member _.FlushStats() = flushStats()

    member _.Stop() =
        _working <- false
        flushStats()
