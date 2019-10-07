module internal NBomber.DomainServices.ScenarioRunner

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Extensions
open NBomber.Domain
open NBomber.Domain.Statistics

type ScenarioActor(actorIndex: int, correlationId: string,
                   scenario: Scenario, globalTimer: Stopwatch,
                   fastCancelToken: FastCancellationToken, cancelToken: CancellationToken) =    
    
    let allResponses = ResizeArray<ResizeArray<StepResponse>>(scenario.Steps.Length)    
    let steps = scenario.Steps |> Array.map(Step.setStepContext(correlationId, actorIndex, cancelToken))
    let mutable working = false
    
    do steps |> Array.iter(fun _ -> allResponses.Add(ResizeArray<StepResponse>()))
    
    member x.Working = working

    member x.ExecSteps() = task {
        working <- true
        do! Step.execSteps(steps, allResponses, fastCancelToken, globalTimer)
        working <- false
    }

    member x.GetStepResults(duration) =
        let filteredResponses =
            allResponses |> Seq.map(fun x -> Step.filterLateResponses(x, duration)) |> Seq.toArray
        
        scenario.Steps
        |> Array.mapi(fun i step -> StepResults.create(step.StepName, filteredResponses.[i]))        

type ActorTaskId = int

type ActorTask = {
    Actor: ScenarioActor
    mutable Task: Task<unit>
}

type ScenarioScheduler(allActors: ScenarioActor[], fastCancelToken: FastCancellationToken) =
    
    let threadCount = Environment.ProcessorCount * 2
    
    let calcActorBulkSize (concurrencyCount, threadCount) =
        let result = concurrencyCount / threadCount
        if concurrencyCount % threadCount = 0 then result
        else result + 1
    
    let startActorsTasks (actorsBulk: ScenarioActor[]) =
        let actorsTasks = Dictionary<ActorTaskId, ActorTask>()
        
        actorsBulk
        |> Array.map(fun x -> { Actor = x; Task = x.ExecSteps() })
        |> Array.iter(fun x -> actorsTasks.[x.Task.Id] <- x)
        
        actorsTasks
    
    let startEventLoop (actorsBulk: ScenarioActor[]) =        
        
        let actorsTasks = startActorsTasks(actorsBulk)
        
        task {
            do! Task.Yield()

            while not fastCancelToken.ShouldCancel do
                let! finishedTask = Task.WhenAny(actorsTasks.Values |> Seq.map(fun x -> x.Task))
                
                let item = actorsTasks.[finishedTask.Id]
                item.Task <- item.Actor.ExecSteps()
                
                actorsTasks.Remove(finishedTask.Id) |> ignore
                actorsTasks.[item.Task.Id] <- item
                
            let allTasks = actorsTasks.Values |> Seq.map(fun x -> x.Task :> Task)
            do! Task.WhenAll(allTasks)
        }
    
    member x.Run() =        
        let actorBulkSize = calcActorBulkSize(allActors.Length, threadCount)            
        allActors
        |> Array.chunkBySize actorBulkSize
        |> Array.map(fun actorBulks -> startEventLoop(actorBulks) :> Task)
        |> Task.WhenAll

type ScenarioRunner(scenario: Scenario, logger: Serilog.ILogger) =
    
    let [<Literal>] TryCount = 20
    let mutable curCancelToken = new CancellationTokenSource()
    let curFastCancelToken = { ShouldCancel = false }
    let mutable curActors = Array.empty<ScenarioActor>    
    let mutable curJob: Task option = None
    
    let waitOnFinish (job: Task, actors: ScenarioActor[]) = task {

        let mutable count = 0        
        while count < TryCount do
            let! completedTask = Task.WhenAny(job, Task.Delay(TimeSpan.FromSeconds(2.0)))
            match completedTask.Equals(job) with
            | true -> count <- TryCount

            | false when count = TryCount ->
                logger.Information("hard stop of not finished steps.")
                count <- count + 1
                
            | false -> let workingSteps = actors |> Array.filter(fun x -> x.Working) |> Array.length
                       logger.Information(sprintf "waiting on '%i' working steps to finish..." workingSteps)
                       count <- count + 1
    }

    let stop (job: Task option, actors: ScenarioActor[]) = task {
        if not curCancelToken.IsCancellationRequested then            
           curFastCancelToken.ShouldCancel <- true
           curCancelToken.Cancel()
           
           if job.IsSome then
               do! waitOnFinish(job.Value, actors)

           curCancelToken <- new CancellationTokenSource()
           curFastCancelToken.ShouldCancel <- false
    }    

    let createActors (scenario, fastCancelToken: FastCancellationToken, cancelToken: CancellationTokenSource) = 

        let globalTimer = Stopwatch()                
        
        let actors = 
            scenario.CorrelationIds
            |> Array.mapi(fun actorIndex correlationId ->
                ScenarioActor(actorIndex, correlationId, scenario, globalTimer,
                              fastCancelToken, cancelToken.Token)
            )
        
        globalTimer, actors
        
    let run (duration: TimeSpan) = task {
        
        do! stop(curJob, curActors)
        
        let globalTimer, actors = createActors(scenario, curFastCancelToken, curCancelToken)        
        let scheduler = ScenarioScheduler(actors, curFastCancelToken)
        globalTimer.Start()
        let job = scheduler.Run()
        
        curActors <- actors
        curJob <- Some job
        
        // wait on finish
        do! Task.Delay(duration, curCancelToken.Token)
        
        // stop execution
        globalTimer.Stop()
        do! stop(curJob, actors)
    }

    member x.Scenario = scenario
    member x.WarmUp() = run(scenario.WarmUpDuration)
    member x.Run() = run(scenario.Duration)
    member x.Stop() = stop(curJob, curActors)
    
    member x.GetScenarioStats() =
        x.GetScenarioStats(scenario.Duration)        
        
    member x.GetScenarioStats(executionTime) =
        curActors
        |> Array.collect(fun x -> x.GetStepResults executionTime) 
        |> ScenarioStats.create scenario executionTime