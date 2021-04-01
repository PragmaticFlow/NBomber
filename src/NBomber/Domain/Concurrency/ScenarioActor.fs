module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open Nessos.Streams
open FSharp.UMX
open FSharp.Control.Tasks.NonAffine

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Step

type ActorDep = {
    Logger: ILogger
    CancellationToken: CancellationToken
    GlobalTimer: Stopwatch
    Scenario: Scenario
    ExecStopCommand: StopCommand -> unit
}

type ScenarioActor(dep: ActorDep, scenarioId: ScenarioId) =

    let _isAllExecSync = Step.isAllExecSync dep.Scenario.Steps

    let _stepDep = { ScenarioName = dep.Scenario.ScenarioName
                     ScenarioMaxDuration = % dep.Scenario.PlanedDuration.Ticks
                     Logger = dep.Logger; CancellationToken = dep.CancellationToken
                     GlobalTimer = dep.GlobalTimer; ScenarioId = scenarioId
                     ExecStopCommand = dep.ExecStopCommand }

    let _steps = dep.Scenario.Steps
                 |> List.map(RunningStep.create _stepDep)
                 |> List.toArray

    let mutable _working = false

    member _.ScenarioId = scenarioId
    member _.Working = _working

    member _.ExecSteps() = task {
        try
            if not _working then
                _working <- true
                do! Task.Yield()
                if _isAllExecSync then
                    Step.execSteps _stepDep _steps (dep.Scenario.GetStepsOrder())
                else
                    do! Step.execStepsAsync _stepDep _steps (dep.Scenario.GetStepsOrder())
            else
                dep.Logger.Fatal("ExecSteps was invoked for already working actor with scenario '{ScenarioName}'.", dep.Scenario.ScenarioName)
        finally
            _working <- false
    }

    member _.RunInfinite() = task {
        try
            if not _working then
                _working <- true
                do! Task.Yield()
                while _working && not dep.CancellationToken.IsCancellationRequested do
                    if _isAllExecSync then
                        Step.execSteps _stepDep _steps (dep.Scenario.GetStepsOrder())
                    else
                        do! Step.execStepsAsync _stepDep _steps (dep.Scenario.GetStepsOrder())
            else
                dep.Logger.Fatal("RunInfinite was invoked for already working actor with scenario '{ScenarioName}'.", dep.Scenario.ScenarioName)
        finally
            _working <- false
    }

    member _.Stop() = _working <- false

    member _.GetStepStats(duration) =
        _steps
        |> Stream.ofArray
        |> Stream.choose(fun x -> if x.Value.DoNotTrack then None else Some x)
        |> Stream.map(fun x -> Statistics.StepStats.create x.Value.StepName x.ExecutionData duration)
