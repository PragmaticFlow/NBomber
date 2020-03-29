module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Extensions
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

    let _allScnResponses = Array.init<StepResponse list> dep.Scenario.Steps.Length (fun _ -> List.empty)

    let _stepDep = { Logger = dep.Logger; CancellationToken = dep.CancellationToken
                     GlobalTimer = dep.GlobalTimer; CorrelationId = correlationId
                     ExecStopCommand = dep.ExecStopCommand }

    let mutable _working = false
    let mutable _reserved = false
    let mutable _currentTask = Unchecked.defaultof<Task>

    member x.CorrelationId = correlationId
    member x.Working = _working
    member x.Reserved = _reserved
    member x.CurrentTask = _currentTask

    member x.ReserveForScheduler() =
        _reserved <- true

    member x.LeaveScheduler() =
        _reserved <- false

    member x.ExecSteps() = task {
        if _reserved then
            _working <- true
            _currentTask <- Step.execSteps(_stepDep, dep.Scenario.Steps, _allScnResponses )
            do! _currentTask
            _working <- false
    }

    member x.GetStepResults(duration) =
        let filteredResponses =
            _allScnResponses
            |> Array.map(fun stepResponses -> Step.filterByDuration(stepResponses, duration))

        dep.Scenario.Steps
        |> List.mapi(fun i step -> step, StepResults.create(step.StepName, filteredResponses.[i]))
        |> List.choose(fun (step, results) -> if step.DoNotTrack then None else Some results)
