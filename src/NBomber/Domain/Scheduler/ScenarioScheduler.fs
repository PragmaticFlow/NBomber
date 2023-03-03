module internal NBomber.Domain.Scheduler.ScenarioScheduler

open System
open System.Threading
open System.Threading.Tasks
open System.Timers
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

let private emptyExec (createActors: int -> int -> ScenarioActor[]) (actorPool: ResizeArray<ScenarioActor>) (scheduledActorCount: int) = actorPool

type ScenarioScheduler(scnCtx: ScenarioContextArgs, scenarioClusterCount: int) =

    let _log = scnCtx.Logger.ForContext<ScenarioScheduler>()
    let _randomGen = Random()
    let _statsActor = scnCtx.ScenarioStatsActor
    let _scnCancelToken = scnCtx.ScenarioCancellationToken.Token
    let mutable _scenario = scnCtx.Scenario
    let mutable _warmupTimer = new Timer()
    let mutable _currentSimulation = _scenario.LoadSimulations.Head
    let mutable _cachedSimulationStats = Unchecked.defaultof<LoadSimulationStats>
    let mutable _pauseDuration = TimeSpan.Zero
    let mutable _isWorking = false
    let _scnTimer = scnCtx.ScenarioTimer

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
        _statsActor.Publish StartUseTempBuffer

    let buildRealtimeStats (duration: TimeSpan) =
        let simulationStats = getCurrentSimulationStats()
        let reply = TaskCompletionSource<ScenarioStats>()
        _statsActor.Publish(BuildReportingStats(reply, simulationStats, duration))
        reply.Task

    let commitRealtimeStats (duration) =
        let reply = TaskCompletionSource<ScenarioStats>()
        _statsActor.Publish(BuildReportingStats(reply, _cachedSimulationStats, duration))
        _statsActor.Publish FlushTempBuffer
        reply.Task

    let getFinalStats () =
        let simulationStats = getCurrentSimulationStats()
        let executedDuration = Scenario.getExecutedDuration _scenario
        let reply = TaskCompletionSource<ScenarioStats>()
        _statsActor.Publish(GetFinalStats(reply, simulationStats, executedDuration, _pauseDuration))
        reply.Task

    let getRandomValue minRate maxRate =
        _randomGen.Next(minRate, maxRate)

    let stop () =
        if _isWorking then
            _isWorking <- false

            _scenario <-
                if not scnCtx.ScenarioCancellationToken.IsCancellationRequested then
                    scnCtx.ScenarioCancellationToken.Cancel()
                    Scenario.setExecutedDuration _scenario _scenario.PlanedDuration
                else
                    Scenario.setExecutedDuration _scenario _scnTimer.Elapsed

            _constantScheduler.Stop()
            _oneTimeScheduler.Stop()
            _scnTimer.Stop()
            _warmupTimer.Stop()

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
                    stop()
                    scnCtx.ExecStopCommand(StopCommand.StopTest $"Stopping test because of too many fails. Scenario '{_scenario.ScenarioName}' contains '{scnCtx.ScenarioStatsActor.ScenarioFailCount}' fails.")                                        

                let startInterval = _scnTimer.Elapsed
                let timeProgress = LoadSimulation.calcTimeProgress currentTime simulation.Duration

                let struct (command, copiesCount) =
                    schedule getRandomValue simulation timeProgress _constantScheduler.ScheduledActorCount

                match command with
                | AddConstantActors    -> _constantScheduler.AddActors copiesCount
                | RemoveConstantActors -> _constantScheduler.RemoveActors copiesCount
                | InjectOneTimeActors  -> _oneTimeScheduler.InjectActors copiesCount
                | DoNothing            -> ()

                try
                    do! Task.Delay(simulationInterval - intervalDrift, _scnCancelToken)
                    
                    let endInterval = _scnTimer.Elapsed                    
                    intervalDrift <- calcTimeDrift startInterval endInterval simulationInterval
                    
                    currentTime <- currentTime + simulationInterval
                with
                | _ -> ()  // operation cancel

            // update pauseDuration to calculate correctly RPS
            match simulation.Value with
            | Pause duration -> _pauseDuration <- _pauseDuration + duration
            | _              -> ()

        stop()
    }

    member _.Working = _isWorking
    member _.Scenario = _scenario
    member _.AllRealtimeStats = scnCtx.ScenarioStatsActor.AllRealtimeStats
    member _.ConsoleScenarioStats = scnCtx.ScenarioStatsActor.ConsoleScenarioStats

    member _.Start(bombingCancelToken) =
        if scnCtx.ScenarioOperation = ScenarioOperation.WarmUp && _scenario.WarmUpDuration.IsSome then
            _warmupTimer <- new Timer(_scenario.WarmUpDuration.Value.TotalMilliseconds)
            _warmupTimer.Elapsed.Add(fun _ -> stop())
            _warmupTimer.Start()

        start(bombingCancelToken) :> Task

    member _.Stop() =
        scnCtx.ScenarioCancellationToken.Cancel()
        stop()

    member _.AddStatsFromAgent(stats) = scnCtx.ScenarioStatsActor.Publish(AddFromAgent stats)
    member _.PrepareForRealtimeStats() = prepareForRealtimeStats()
    member _.CommitRealtimeStats(duration) = commitRealtimeStats duration
    member _.BuildRealtimeStats(duration) = buildRealtimeStats duration
    member _.GetFinalStats() = getFinalStats()

    interface IDisposable with
        member _.Dispose() =
            scnCtx.ScenarioCancellationToken.Cancel()
            stop()

module Test =

    let schedule getRandomValue simulation timeProgress currentConstActorCount = schedule getRandomValue simulation timeProgress currentConstActorCount
    let scheduleCleanPrevSimulation simulation currentConstActorCount = scheduleCleanPrevSimulation simulation currentConstActorCount
    let calcTimeDrift startInterval endInterval simulationInterval = calcTimeDrift startInterval endInterval simulationInterval
