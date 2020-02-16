module internal NBomber.DomainServices.ScenarioRunner

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Extensions
open NBomber.Domain
open NBomber.Domain.Statistics

type ScenarioActor(logger: ILogger,
                   actorIndex: int,
                   correlationId: string,
                   scenario: Scenario,
                   scenarioTimer: Stopwatch,
                   fastCancelToken: FastCancellationToken,
                   cancelToken: CancellationToken) =

    let allScnResponses = ResizeArray<ResizeArray<StepResponse>>(scenario.Steps.Length)
    let mutable working = false
    do scenario.Steps |> Array.iter(fun _ -> allScnResponses.Add(ResizeArray<StepResponse>()))

    member x.Working = working

    member x.ExecSteps() = task {
        working <- true
        let feedData = scenario.Feed.GetNext()
        let steps =
            scenario.Steps
            |> Array.map(Step.setStepContext correlationId feedData actorIndex cancelToken logger)
        do! Step.execSteps(logger, steps, allScnResponses, fastCancelToken, scenarioTimer)
        working <- false
    }

    member x.GetStepResults(duration) =
        let filteredResponses =
            allScnResponses
            |> Seq.map(fun stepResponses -> Step.filterByDuration(stepResponses, duration))
            |> Seq.toArray

        scenario.Steps
        |> Array.mapi(fun i step -> step, StepResults.create(step.StepName, filteredResponses.[i]))
        |> Array.choose(fun (step, results) -> if step.DoNotTrack then None else Some results)

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

    let createActorsEnv (scenario, fastCancelToken: FastCancellationToken, cancelToken: CancellationTokenSource) =

        let scenarioTimer = Stopwatch()

        let actors =
            scenario.CorrelationIds
            |> Array.mapi(fun actorIndex correlationId ->
                ScenarioActor(logger, actorIndex, correlationId, scenario, scenarioTimer,
                              fastCancelToken, cancelToken.Token)
            )

        {| ScenarioTimer = scenarioTimer; Actors = actors |}

    let run (duration: TimeSpan) = task {

        do! stop(curJob, curActors)

        let env = createActorsEnv(scenario, curFastCancelToken, curCancelToken)
        let scheduler = ScenarioScheduler(env.Actors, curFastCancelToken)
        env.ScenarioTimer.Start()
        let job = scheduler.Run()

        curActors <- env.Actors
        curJob <- Some job

        // wait on finish
        do! Task.Delay(duration, curCancelToken.Token)

        // stop execution
        env.ScenarioTimer.Stop()
        do! stop(curJob, env.Actors)
    }

    member x.Scenario = scenario
    member x.WarmUp() = run(scenario.WarmUpDuration)
    member x.Run() = run(scenario.Duration)
    member x.Stop() = stop(curJob, curActors)

    member x.GetScenarioStats(executionTime: TimeSpan option) =
        let duration = if executionTime.IsSome then executionTime.Value
                       else scenario.Duration
        curActors
        |> Array.collect(fun x -> x.GetStepResults duration)
        |> ScenarioStats.create scenario duration
