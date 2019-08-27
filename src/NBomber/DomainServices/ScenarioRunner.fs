module internal NBomber.DomainServices.ScenarioRunner

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

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
        x

    member x.Run(fastCancelToken, duration: TimeSpan) = task {                
        let! responses = Step.runSteps(steps, fastCancelToken, globalTimer)        
        stepResponses <- 
            responses
            |> Seq.map(fun x -> Step.filterLateResponses(x.ToArray(), duration))
            |> Seq.toArray
    }

    member x.GetResults() =        
        if Array.isEmpty stepResponses then Array.empty
        else            
            scenario.Steps
            |> Array.mapi(fun i st -> StepResults.create(st.StepName, stepResponses.[i]))

type ScenarioRunner(scenario: Scenario) = 
    
    let [<Literal>] TryCount = 20
    let mutable curntCancToken = new CancellationTokenSource()
    let curntFastCancelToken = { ShouldCancel = false }
    let mutable curntActors = Array.empty<ScenarioActor>
    let mutable curntActorsEnv: Option<ActionBlock<ScenarioActor>> = None

    let waitOnFinish (actorsEnv: ActionBlock<ScenarioActor>) = task {

        let mutable count = 0        
        while count < TryCount do
            let! completedTask = Task.WhenAny(actorsEnv.Completion, Task.Delay(TimeSpan.FromSeconds(2.0)))
            match completedTask.Equals(actorsEnv.Completion) with
            | true -> count <- TryCount

            | false when count = TryCount ->
                Log.Information("hard stop of not finished steps.")
                count <- count + 1
                
            | false -> Log.Information("waiting all steps to finish...")
                       count <- count + 1
    }

    let stop (actorsEnv: ActionBlock<ScenarioActor> option) = task {
        if not curntCancToken.IsCancellationRequested then            
           curntFastCancelToken.ShouldCancel <- true
           curntCancToken.Cancel()
           
           if actorsEnv.IsSome then 
              do! waitOnFinish(actorsEnv.Value)

           curntCancToken <- new CancellationTokenSource()
           curntFastCancelToken.ShouldCancel <- false
    }

    let createActors (scn: Scenario, fastToken: FastCancellationToken, cancToken: CancellationToken) = 

        let globalTimer = Stopwatch()
        let actors = 
            scn.CorrelationIds
            |> Array.mapi(fun i id -> ScenarioActor(i, id, scn, globalTimer))        
            |> Array.map(fun x -> x.Init(cancToken))
        
        let envOptions = ExecutionDataflowBlockOptions(MaxDegreeOfParallelism = 8)
        let actorsEnv = ActionBlock<ScenarioActor>((fun actor -> 
            actor.Run(fastToken, scn.Duration) :> Task), envOptions)
        
        globalTimer, actors, actorsEnv

    let run (scn: Scenario, duration: TimeSpan) = task {
        
        do! stop curntActorsEnv

        let globalTimer, actors, actorsEnv = createActors(scn, curntFastCancelToken, curntCancToken.Token)
        curntActors <- actors
        curntActorsEnv <- Some actorsEnv

        globalTimer.Start()
        actors |> Array.iter(actorsEnv.Post >> ignore)
        actorsEnv.Complete()
        
        // wait on finish
        do! Task.Delay(duration, curntCancToken.Token)        
        
        // stop execution
        globalTimer.Stop()
        do! stop curntActorsEnv
    }

    member x.Scenario = scenario
    member x.WarmUp() = run(scenario, scenario.WarmUpDuration)
    member x.Run() = run(scenario, scenario.Duration)
    member x.Stop() = stop curntActorsEnv
    
    member x.GetResult() =        
        curntActors
        |> Array.collect(fun actor -> actor.GetResults())
        |> ScenarioStats.create scenario