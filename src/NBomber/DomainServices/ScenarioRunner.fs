module internal NBomber.DomainServices.ScenarioRunner

open System
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Extensions
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Statistics

type ScenarioActor(actorIndex: int, correlationId: string, scenario: Scenario) =

    let actorLatencies = ResizeArray<ResizeArray<Response*Latency>>()

    member x.Run(fastCancelToken, cancelToken) = task {
        do! Task.Delay(1)
        actorLatencies.Clear()
        let steps = scenario.Steps |> Array.map(Step.setStepContext(correlationId, actorIndex, cancelToken))
        let! latencies = Step.runSteps(steps, fastCancelToken)
        actorLatencies.AddRange(latencies)
    }

    member x.GetResults() =
        if actorLatencies.Count > 0 then
            scenario.Steps
            |> Array.mapi(fun i st -> StepResults.create st.StepName (actorLatencies.[i].ToArray()) )
        else
            Array.empty

type ScenarioRunner(scenario: Scenario) =

    let mutable cancelToken = new CancellationTokenSource()
    let mutable actorsTasks = Array.empty<Task<_>>
    let fastCancelToken = { ShouldCancel = false }
    let actors = scenario.CorrelationIds |> Array.mapi(fun i id -> ScenarioActor(i, id, scenario))

    let waitOnAllFinish (actorsTasks: Task<unit>[]) = task {
        let allFinish () = actorsTasks |> Array.forall(fun x -> x.IsCanceled || x.IsCompleted || x.IsFaulted)
        while not (allFinish()) do
            Log.Information("waiting all steps to finish.")
            do! Task.Delay(TimeSpan.FromSeconds(1.0))
    }

    let stop () = task {
        if actorsTasks.Length > 0 && not cancelToken.IsCancellationRequested then
            fastCancelToken.ShouldCancel <- true
            cancelToken.Cancel()
            do! Task.Delay(TimeSpan.FromSeconds(1.5))
            do! waitOnAllFinish(actorsTasks)
            actorsTasks <- Array.empty
    }

    let run (duration: TimeSpan) = task {
        do! stop()
        cancelToken <- new CancellationTokenSource()
        fastCancelToken.ShouldCancel <- false
        actorsTasks <- actors |> Array.map(fun x -> x.Run(fastCancelToken, cancelToken.Token))
        do! Task.Delay(duration, cancelToken.Token)
        do! stop()
    }

    member x.Scenario = scenario
    member x.WarmUp() = run(scenario.WarmUpDuration)
    member x.Run() = run(scenario.Duration)
    member x.Stop() = stop()

    member x.GetResult() =
        actors
        |> Array.collect(fun actor -> actor.GetResults())
        |> ScenarioStats.create scenario
