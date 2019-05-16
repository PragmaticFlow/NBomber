module internal NBomber.DomainServices.ScenarioRunner

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Extensions
open NBomber.Domain
open NBomber.Domain.Statistics

type ScenarioActor(actorIndex: int, correlationId: string, 
                   scenario: Scenario, globalTimer: Stopwatch) =    
    
    let mutable stepResponses = Array.empty
    let mutable steps = Array.empty

    member x.Init(cancelToken) =
        stepResponses <- Array.empty
        steps <- scenario.Steps
                 |> Array.map(Step.setStepContext(correlationId, actorIndex, cancelToken))

    member x.Run(fastCancelToken, duration: TimeSpan) = task {                
        let! responses = Step.runSteps(steps, fastCancelToken, globalTimer)        
        stepResponses <- 
            responses
            |> Seq.map(fun x -> Step.filterInvalidResponses(x.ToArray(), duration))
            |> Seq.toArray
    }

    member x.GetResults() =        
        if Array.isEmpty stepResponses then Array.empty
        else            
            scenario.Steps
            |> Array.mapi(fun i st -> StepResults.create(st.StepName, stepResponses.[i]))

type ScenarioRunner(scenario: Scenario) = 
    
    let globalTimer = Stopwatch()
    let mutable cancelToken = new CancellationTokenSource()
    let mutable actorsTasks = Array.empty<Task<_>>
    let fastCancelToken = { ShouldCancel = false }    
    let actors = scenario.CorrelationIds |> Array.mapi(fun i id -> ScenarioActor(i, id, scenario, globalTimer))

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
        
        actors |> Array.iter(fun x -> x.Init(cancelToken.Token))

        globalTimer.Start()
        actorsTasks <- actors |> Array.map(fun x -> x.Run(fastCancelToken, duration))
        
        do! Task.Delay(duration, cancelToken.Token)
        globalTimer.Reset()
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