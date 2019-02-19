module internal NBomber.DomainServices.ScenarioRunner

open System
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Statistics

type ScenarioActor(actorIndex: int, correlationId: string, scenario: Scenario) =
    
    let stepsWithoutPause = scenario.Steps |> Array.filter(fun st -> not(Step.isPause st))
    let latencies = ResizeArray<ResizeArray<Response*Latency>>()    

    member x.Run(fastCancelToken, cancelToken) = 
        latencies.Clear()
        let steps = scenario.Steps |> Array.map(Step.setStepContext(correlationId, actorIndex, cancelToken))
        stepsWithoutPause |> Array.iter(fun _ -> latencies.Add(ResizeArray<Response*Latency>()))            
        Step.runSteps(steps, latencies, fastCancelToken)

    member x.GetResults() =
        stepsWithoutPause
        |> Array.mapi(fun i st -> StepResults.create(Step.getName(st), latencies.[i].ToArray()))

type ScenarioRunner(scenario: Scenario) = 
    
    let mutable cancelToken = new CancellationTokenSource()
    let mutable actorsTasks = Array.empty<Task<unit>>
    let fastCancelToken = { ShouldCancel = false }

    let createActors () =
        scenario.CorrelationIds |> Array.mapi(fun i id -> ScenarioActor(i, id, scenario))

    let actors = lazy createActors()    

    let waitOnAllFinish (actorsTasks: Task<unit>[]) = task {                
        let allFinish () = actorsTasks |> Array.forall(fun x -> x.IsCanceled || x.IsCompleted || x.IsFaulted)        
        while not (allFinish()) do            
            Log.Information("waiting all steps to finish.")
            do! Task.Delay(TimeSpan.FromSeconds(5.0))            
    }

    let stop () = task {
        if actorsTasks.Length > 0 && not cancelToken.IsCancellationRequested then
            fastCancelToken.ShouldCancel <- true
            cancelToken.Cancel()
            do! waitOnAllFinish(actorsTasks)
            actorsTasks <- Array.empty
    }

    let run (duration: TimeSpan) = task {
        do! stop()
        cancelToken <- new CancellationTokenSource()
        fastCancelToken.ShouldCancel <- false
        actorsTasks <- actors.Value |> Array.map(fun x -> x.Run(fastCancelToken, cancelToken.Token))
        do! Task.Delay(duration, cancelToken.Token)
        do! stop()
    }

    member x.Scenario = scenario
    member x.WarmUp() = run(scenario.WarmUpDuration)
    member x.Run() = run(scenario.Duration)
    member x.Stop() = stop()    
    
    member x.GetResult() =
        actors.Value
        |> Array.collect(fun actor -> actor.GetResults())
        |> ScenarioStats.create(scenario)