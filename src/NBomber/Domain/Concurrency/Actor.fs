module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Diagnostics
open System.Threading

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open System.Threading.Tasks
open NBomber.Domain
open NBomber.Extensions
open NBomber.Domain.Statistics

type ActorDep = {
    Logger: ILogger
    FastCancelToken: FastCancellationToken
    CancellationToken: CancellationToken
    GlobalTimer: Stopwatch
    Scenario: Scenario
}

type ScenarioActor(dep: ActorDep, actorIndex: int,
                   correlationId: string, scenario: Scenario) =

    let _allScnResponses = ResizeArray<ResizeArray<StepResponse>>(scenario.Steps.Length)
    let _steps = scenario.Steps |> Array.map(Step.setStepContext dep.Logger correlationId actorIndex dep.CancellationToken)
    let mutable _working = false
    let mutable _reserved = false
    let mutable _schedulerName = ""

    do _steps |> Array.iter(fun _ -> _allScnResponses.Add(ResizeArray<StepResponse>()))

    member x.Working = _working
    member x.Reserved = _reserved
    member x.UseByScheduler = _schedulerName

    member x.ReserveForScheduler(schedulerName) =
        if _schedulerName = "" then
            _reserved <- true
            _schedulerName <- schedulerName

    member x.LeaveScheduler(schedulerName) =
        if _schedulerName = schedulerName then
            _reserved <- false
            _schedulerName <- ""

    member x.ExecSteps() = task {
        if _reserved then
            _working <- true
            do! Step.execSteps(dep.Logger, _steps, _allScnResponses, dep.FastCancelToken, dep.GlobalTimer)
            _working <- false
    }

    member x.GetStepResults(duration) =
        let filteredResponses =
            _allScnResponses
            |> ResizeArray.map(fun stepResponses -> Step.filterByDuration(stepResponses, duration))

        scenario.Steps
        |> Array.mapi(fun i step -> step, StepResults.create(step.StepName, filteredResponses.[i]))
        |> Array.choose(fun (step, results) -> if step.DoNotTrack then None else Some results)
