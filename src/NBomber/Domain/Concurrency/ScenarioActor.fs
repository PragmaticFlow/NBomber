module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Nessos.Streams
open Serilog
open FSharp.Control.Tasks.NonAffine

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Step
open NBomber.Domain.Statistics

type ActorDep = {
    Logger: ILogger
    CancellationToken: CancellationToken
    GlobalTimer: Stopwatch
    Scenario: Scenario
    ExecStopCommand: StopCommand -> unit
}

type ScenarioActor(dep: ActorDep, correlationId: CorrelationId) =

    let _allScnResponses = Array.init<ResizeArray<StepResponse>> dep.Scenario.Steps.Length (fun _ -> ResizeArray(capacity = 1_000_000))
    let _isAllExecSync = Step.isAllExecSync dep.Scenario.Steps

    let _stepDep = { ScenarioName = dep.Scenario.ScenarioName
                     Logger = dep.Logger; CancellationToken = dep.CancellationToken
                     GlobalTimer = dep.GlobalTimer; CorrelationId = correlationId
                     ExecStopCommand = dep.ExecStopCommand }

    let _steps = dep.Scenario.Steps
                 |> List.map(RunningStep.create _stepDep)
                 |> List.toArray

    let mutable _working = false

    member _.CorrelationId = correlationId
    member _.Working = _working

    member _.ExecSteps() = task {
        try
            if not _working then
                _working <- true
                do! Task.Yield()
                if _isAllExecSync then
                    Step.execSteps _stepDep _steps (dep.Scenario.GetStepsOrder()) _allScnResponses
                else
                    do! Step.execStepsAsync _stepDep _steps (dep.Scenario.GetStepsOrder()) _allScnResponses
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
                        Step.execSteps _stepDep _steps (dep.Scenario.GetStepsOrder()) _allScnResponses
                    else
                        do! Step.execStepsAsync _stepDep _steps (dep.Scenario.GetStepsOrder()) _allScnResponses
            else
                dep.Logger.Fatal("RunInfinite was invoked for already working actor with scenario '{ScenarioName}'.", dep.Scenario.ScenarioName)
        finally
            _working <- false
    }

    member _.Stop() = _working <- false

    member _.GetStepResults(duration) =
        let filteredResponses = _allScnResponses |> Array.map(Stream.ofResizeArray >> Step.filterByDuration duration)

        dep.Scenario.Steps
        |> Stream.ofList
        |> Stream.mapi(fun i step -> step, StepResults.create(step.StepName, filteredResponses.[i]))
        |> Stream.choose(fun (step, results) -> if step.DoNotTrack then None else Some results)
