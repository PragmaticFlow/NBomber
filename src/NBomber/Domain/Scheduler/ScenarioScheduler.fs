module internal NBomber.Domain.Scheduler.ScenarioScheduler

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open System.Timers

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Scheduler.ConstantActorScheduler
open NBomber.Domain.Scheduler.OneTimeActorScheduler
open NBomber.Domain.ScenarioContext

[<Struct>]
type SchedulerCommand =
    | AddConstantActors
    | RemoveConstantActors
    | InjectOneTimeActors
    | DoNothing

let inline calcTimeDrift (startInterval: TimeSpan) (endInterval: TimeSpan) (simulationInterval: TimeSpan) =
    let realDuration = endInterval - startInterval

    if realDuration > simulationInterval then
        realDuration - simulationInterval
    else
        TimeSpan.Zero

let inline calcScheduleByTime
    (copiesCount: int)
    (prevSegmentCopiesCount: int)
    (timeSegmentProgress: int) =

    let value = copiesCount - prevSegmentCopiesCount
    let result = (float value / 100.0 * float timeSegmentProgress) + float prevSegmentCopiesCount
    int(Math.Round(result, 0, MidpointRounding.AwayFromZero))

let inline schedule
    (getRandomValue: int -> int -> int) // min -> max -> result
    simulation
    timeProgress
    currentConstActorCount : struct (SchedulerCommand * int) =

    match simulation.Value with
    | RampingConstant (copiesCount, _) ->
        let scheduled = calcScheduleByTime copiesCount simulation.PrevActorCount timeProgress
        let scheduleNow = scheduled - currentConstActorCount
        if scheduleNow > 0 then AddConstantActors, scheduleNow
        elif scheduleNow < 0 then RemoveConstantActors, (Math.Abs scheduleNow)
        else DoNothing, 0

    | KeepConstant (copiesCount, _) ->
        if currentConstActorCount < copiesCount then AddConstantActors, (copiesCount - currentConstActorCount)
        elif currentConstActorCount > copiesCount then RemoveConstantActors, (currentConstActorCount - copiesCount)
        else DoNothing, 0

    | RampingInject (copiesCount, _, _) ->
        let scheduled = calcScheduleByTime copiesCount simulation.PrevActorCount timeProgress
        InjectOneTimeActors, (Math.Abs scheduled)

    | Inject (copiesCount, _, _) ->
        InjectOneTimeActors, copiesCount

    | InjectRandom (minRate, maxRate, _, _) ->
        let copiesCount = getRandomValue minRate maxRate
        InjectOneTimeActors, copiesCount

    | Pause _ ->
        if currentConstActorCount > 0 then RemoveConstantActors, currentConstActorCount
        else DoNothing, 0

let inline scheduleCleanPrevSimulation (simulation) (currentConstActorCount) : struct (SchedulerCommand * int) =
    if currentConstActorCount > 0 then
        match simulation.Value with
        | RampingConstant _ -> DoNothing, 0
        | KeepConstant _    -> DoNothing, 0
        | RampingInject _
        | Inject _
        | InjectRandom _
        | Pause _ -> RemoveConstantActors, currentConstActorCount
    else
        DoNothing, 0

let private emptyExec (createActors: int -> int -> ScenarioActor[]) (actorPool: ResizeArray<ScenarioActor>) (scheduledActorCount: int) (injectInterval: TimeSpan) = actorPool

type ScenarioScheduler(scnCtx: ScenarioContextArgs, scenarioClusterCount: int) =

    let _randomGen = Random()
    let _statsActor = scnCtx.ScenarioStatsActor
    let _scnCancelToken = scnCtx.ScenarioCancellationToken.Token
    let _scnTimer = Stopwatch()

    let mutable _scenario = scnCtx.Scenario
    let mutable _warmupTimer = new Timer()
    let mutable _currentSimulation = _scenario.LoadSimulations.Head
    let mutable _cachedSimulationStats = Unchecked.defaultof<LoadSimulationStats>
    let mutable _pauseDuration = TimeSpan.Zero
    let mutable _isWorking = false

    let _constantScheduler =
        if _scenario.IsEnabled then new ConstantActorScheduler(scnCtx, ConstantActorScheduler.exec)
        else new ConstantActorScheduler(scnCtx, emptyExec)

    let _oneTimeScheduler =
        if _scenario.IsEnabled then new OneTimeActorScheduler(scnCtx, OneTimeActorScheduler.exec)
        else new OneTimeActorScheduler(scnCtx, emptyExec)

    let getConstantActorCount () = _constantScheduler.ScheduledActorCount * scenarioClusterCount
    let getOneTimeActorCount () = _oneTimeScheduler.ScheduledActorCount * scenarioClusterCount

    let getCurrentSimulationStats () =
        LoadSimulation.createSimulationStats _currentSimulation.Value (getConstantActorCount()) (getOneTimeActorCount())

    let prepareForRealtimeStats () =
        _cachedSimulationStats <- getCurrentSimulationStats()

    let buildRealtimeStats (duration: TimeSpan) =
        let simulationStats = getCurrentSimulationStats()
        let reply = TaskCompletionSource<ScenarioStats>()
        _statsActor.Publish(BuildReportingStats(reply, simulationStats, duration))
        reply.Task

    let commitRealtimeStats (duration) =
        let reply = TaskCompletionSource<ScenarioStats>()
        _statsActor.Publish(BuildReportingStats(reply, _cachedSimulationStats, duration))
        reply.Task

    let getFinalStats () =
        let simulationStats = getCurrentSimulationStats()
        let executedDuration = Scenario.getExecutedDuration _scenario
        let reply = TaskCompletionSource<ScenarioStats>()
        _statsActor.Publish(GetFinalStats(reply, simulationStats, executedDuration, _pauseDuration))
        reply.Task

    let getRandomValue minRate maxRate =
        _randomGen.Next(minRate, maxRate)

    let waitOnWorkingActors () = backgroundTask {

        let getWorkingActorCount () =
            _constantScheduler.AvailableActors
            |> Seq.append _oneTimeScheduler.AvailableActors
            |> Concurrency.ScenarioActorPool.getWorkingActors
            |> Seq.length

        let mutable counter = 0

        while counter < Constants.MaxWaitWorkingActorsSec do
            let actorsCount = getWorkingActorCount()
            if actorsCount > 0 then
                do! Task.Delay Constants.ONE_SECOND
                counter <- counter + 1
            else
                counter <- Constants.MaxWaitWorkingActorsSec
    }

    let stop (currentTime: TimeSpan option) = backgroundTask {
        scnCtx.ScenarioCancellationToken.Cancel()

        if _isWorking then
            _isWorking <- false

            let executedDuration =
                currentTime
                |> Option.defaultValue _scnTimer.Elapsed

            _scenario <- Scenario.setExecutedDuration _scenario executedDuration

            (_constantScheduler :> IDisposable).Dispose()
            (_oneTimeScheduler :> IDisposable).Dispose()
            _scnTimer.Stop()
            _warmupTimer.Stop()

            do! waitOnWorkingActors()
    }

    let start (testHostCancelToken: CancellationToken) = backgroundTask {
        _isWorking <- true
        _scnTimer.Start()
        let mutable currentTime = TimeSpan.Zero

        for simulation in _scenario.LoadSimulations do
            currentTime <- TimeSpan.Zero
            _currentSimulation <- simulation

            // if we need switch from ClosedModel simulation to OpenModel simulation
            // in this case we need to schedule clean for prev simulation (stop all our ClosedModel's actors)
            let struct (command, copiesCount) =
                scheduleCleanPrevSimulation simulation _constantScheduler.ScheduledActorCount

            match command with
            | RemoveConstantActors -> _constantScheduler.RemoveActors copiesCount
            | _ -> ()

            let simulationInterval = LoadSimulation.getSimulationInterval simulation.Value
            let mutable intervalDrift = TimeSpan.Zero

            while _isWorking && currentTime < simulation.Duration
                  && not _scnCancelToken.IsCancellationRequested
                  && not testHostCancelToken.IsCancellationRequested do

                if _statsActor.ScenarioFailCount >= _scenario.MaxFailCount then
                    stop(Some currentTime) |> ignore
                    scnCtx.ExecStopCommand(StopCommand.StopTest $"Stopping test because of too many fails. Scenario '{_scenario.ScenarioName}' contains '{scnCtx.ScenarioStatsActor.ScenarioFailCount}' fails.")

                let startInterval = _scnTimer.Elapsed
                let timeProgress = LoadSimulation.calcTimeProgress currentTime simulation.Duration

                let struct (command, copiesCount) =
                    schedule getRandomValue simulation timeProgress _constantScheduler.ScheduledActorCount

                match command with
                | AddConstantActors    -> _constantScheduler.AddActors(copiesCount, simulationInterval)
                | RemoveConstantActors -> _constantScheduler.RemoveActors copiesCount
                | InjectOneTimeActors  -> _oneTimeScheduler.InjectActors(copiesCount, simulationInterval)
                | DoNothing            -> ()

                try
                    let interval = simulationInterval - intervalDrift
                    if interval > TimeSpan.Zero then
                        do! Task.Delay(interval, _scnCancelToken)
                    else
                        do! Task.Delay(simulationInterval, _scnCancelToken)

                    let endInterval = _scnTimer.Elapsed
                    intervalDrift <- calcTimeDrift startInterval endInterval simulationInterval

                    currentTime <- currentTime + simulationInterval
                    scnCtx.CurrentTimeBucket <- scnCtx.CurrentTimeBucket + simulationInterval
                with
                | _ -> ()  // operation cancel

            // update pauseDuration to calculate correctly RPS
            match simulation.Value with
            | Pause duration -> _pauseDuration <- _pauseDuration + duration
            | _              -> ()

        stop(Some currentTime) |> ignore
    }

    member this.Working = _isWorking
    member this.Scenario = _scenario
    member this.AllRealtimeStats = scnCtx.ScenarioStatsActor.AllRealtimeStats
    member this.ConsoleScenarioStats = scnCtx.ScenarioStatsActor.ConsoleScenarioStats

    member this.Start(bombingCancelToken) =
        if scnCtx.ScenarioOperation = ScenarioOperation.WarmUp && _scenario.WarmUpDuration.IsSome then
            _warmupTimer <- new Timer(_scenario.WarmUpDuration.Value.TotalMilliseconds)
            _warmupTimer.Elapsed.Add(fun _ -> stop(None) |> ignore)
            _warmupTimer.Start()

        start(bombingCancelToken) :> Task

    member this.Stop() = stop(None) :> Task

    member this.AddStatsFromAgent(stats) = scnCtx.ScenarioStatsActor.Publish(AddFromAgent stats)
    member this.PrepareForRealtimeStats() = prepareForRealtimeStats()
    member this.CommitRealtimeStats(duration) = commitRealtimeStats duration
    member this.BuildRealtimeStats(duration) = buildRealtimeStats duration
    member this.GetFinalStats() = getFinalStats()

    interface IDisposable with
        member this.Dispose() =
            stop(None) |> ignore
            (scnCtx.ScenarioStatsActor :> IDisposable).Dispose()

module Test =

    let schedule getRandomValue simulation timeProgress currentConstActorCount = schedule getRandomValue simulation timeProgress currentConstActorCount
    let scheduleCleanPrevSimulation simulation currentConstActorCount = scheduleCleanPrevSimulation simulation currentConstActorCount
    let calcTimeDrift startInterval endInterval simulationInterval = calcTimeDrift startInterval endInterval simulationInterval
