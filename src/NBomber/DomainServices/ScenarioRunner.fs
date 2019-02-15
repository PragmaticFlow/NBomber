module internal NBomber.DomainServices.ScenarioRunner

open System
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Statistics

type ScenarioActor(actorIndex: int, correlationId: string, scenario: Scenario) =
    
    let steps = scenario.Steps |> Array.map(Step.setStepContext(correlationId, actorIndex))
    let stepsWithoutPause = steps |> Array.filter(fun st -> not(Step.isPause st))
    let latencies = ResizeArray<ResizeArray<Response*Latency>>()    

    member x.Run(cancellToken) = 
        latencies.Clear()
        stepsWithoutPause |> Array.iter(fun _ -> latencies.Add(ResizeArray<Response*Latency>()))            
        Step.runSteps(steps, latencies, cancellToken)

    member x.GetResults() =
        stepsWithoutPause
        |> Array.mapi(fun i st -> StepResults.create(Step.getName(st), latencies.[i].ToArray()))

type ScenarioRunner(scenario: Scenario) = 
    
    let mutable cancellToken = new CancellationTokenSource()
    let mutable actorsTasks = Array.empty<Task<unit>>

    let createActors () =
        scenario.CorrelationIds |> Array.mapi(fun i id -> ScenarioActor(i, id, scenario))

    let actors = lazy createActors()    

    let waitOnAllFinish (actorsTasks: Task<unit>[]) = task {
        let mutable allFinish = actorsTasks |> Array.forall(fun x -> x.IsCompleted)
        while not allFinish do            
            allFinish <- actorsTasks |> Array.forall(fun x -> x.IsCompleted)
            do! Task.Delay(100)
    }

    let stop () = task {
        if not cancellToken.IsCancellationRequested then cancellToken.Cancel()
        do! waitOnAllFinish(actorsTasks)
    }

    let run (duration: TimeSpan) = task {
        do! stop()
        cancellToken <- new CancellationTokenSource()
        actorsTasks <- actors.Value |> Array.map(fun x -> x.Run(cancellToken.Token))
        do! Task.Delay(duration, cancellToken.Token)
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