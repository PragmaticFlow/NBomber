module internal NBomber.Domain.Scheduler.ScenarioScheduler

open System
open System.Runtime.CompilerServices
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

type SchedulerCommand =
    | AddConstantActors = 0
    | RemoveConstantActors = 1
    | InjectOneTimeActors = 2
    | DoNothing = 3

let inline calcScheduleByTime
    (copiesCount: int)
    (prevSegmentCopiesCount: int)
    (timeSegmentProgress: int) =

    let value = copiesCount - prevSegmentCopiesCount
    let result = (float value / 100.0 * float timeSegmentProgress) + float prevSegmentCopiesCount
    int(Math.Round(result, 0, MidpointRounding.AwayFromZero))

[<MethodImpl(MethodImplOptions.AggressiveInlining)>]
let schedule
    (getRandomValue: int -> int -> int) // min -> max -> result
    simulation
    timeProgress
    currentConstActorCount : struct (SchedulerCommand * int) =

    match simulation.Value with
    | RampingConstant (copiesCount, _) ->
        let scheduled = calcScheduleByTime copiesCount simulation.PrevActorCount timeProgress
        let scheduleNow = scheduled - currentConstActorCount
        if scheduleNow > 0 then SchedulerCommand.AddConstantActors, scheduleNow
        elif scheduleNow < 0 then SchedulerCommand.RemoveConstantActors, (Math.Abs scheduleNow)
        else SchedulerCommand.DoNothing, 0

    | KeepConstant (copiesCount, _) ->
        if currentConstActorCount < copiesCount then SchedulerCommand.AddConstantActors, (copiesCount - currentConstActorCount)
        elif currentConstActorCount > copiesCount then SchedulerCommand.RemoveConstantActors, (currentConstActorCount - copiesCount)
        else SchedulerCommand.DoNothing, 0

    | RampingInject (copiesCount, _, _) ->
        let scheduled = calcScheduleByTime copiesCount simulation.PrevActorCount timeProgress
        SchedulerCommand.InjectOneTimeActors, (Math.Abs scheduled)

    | Inject (copiesCount, _, _) ->
        SchedulerCommand.InjectOneTimeActors, copiesCount

    | InjectRandom (minRate, maxRate, _, _) ->
        let copiesCount = getRandomValue minRate maxRate
        SchedulerCommand.InjectOneTimeActors, copiesCount

let inline scheduleCleanPrevSimulation simulation currentConstActorCount : struct (SchedulerCommand * int) =
    if currentConstActorCount > 0 then
        match simulation.Value with
        | RampingInject _
        | Inject _
        | InjectRandom _ -> SchedulerCommand.RemoveConstantActors, currentConstActorCount
        | _              -> SchedulerCommand.DoNothing, 0
    else
        SchedulerCommand.DoNothing, 0

let emptyExec (scnCtx: ScenarioContextArgs) (actorPool: ScenarioActor list) (scheduledActorCount: int) = actorPool

type ScenarioScheduler(scnCtx: ScenarioContextArgs, scenarioClusterCount: int) =

    let _log = scnCtx.Logger.ForContext<ScenarioScheduler>()
    let _randomGen = Random()
    let _statsActor = scnCtx.ScenarioStatsActor
    let mutable _scenario = scnCtx.Scenario
    let mutable _warmupTimer = new Timer()
    let mutable _currentSimulation = _scenario.LoadSimulations.Head.Value
    let mutable _cachedSimulationStats = Unchecked.defaultof<LoadSimulationStats>
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
        LoadSimulation.createSimulationStats _currentSimulation (getConstantActorCount()) (getOneTimeActorCount())

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
        let duration = Scenario.getExecutedDuration _scenario
        let reply = TaskCompletionSource<ScenarioStats>()
        _statsActor.Publish(GetFinalStats(reply, simulationStats, duration))
        reply.Task

    let getRandomValue minRate maxRate =
        _randomGen.Next(minRate, maxRate)

    let stop () =
        if _isWorking then
            _isWorking <- false
            scnCtx.ScenarioCancellationToken.Cancel()
            _constantScheduler.Stop()
            _oneTimeScheduler.Stop()
            _scenario <- Scenario.setExecutedDuration _scenario scnCtx.ScenarioTimer.Elapsed
            scnCtx.ScenarioTimer.Stop()
            _warmupTimer.Stop()

    let start () = backgroundTask {

        _isWorking <- true
        let mutable currentTime = TimeSpan.Zero
        scnCtx.ScenarioTimer.Restart()

        for simulation in _scenario.LoadSimulations do
            currentTime <- TimeSpan.Zero
            _currentSimulation <- simulation.Value

            // if we need switch from ClosedModel simulation to OpenModel simulation
            // in this case we need to schedule clean for prev simulation (stop all our ClosedModel's actors)
            let struct (command, copiesCount) =
                scheduleCleanPrevSimulation simulation _constantScheduler.ScheduledActorCount

            match command with
            | SchedulerCommand.RemoveConstantActors -> _constantScheduler.RemoveActors copiesCount
            | _ -> ()

            let struct (interval, duration) =
                LoadSimulation.getSimulationInterval simulation.Value

            while _isWorking && currentTime < duration do
                if _statsActor.ScenarioFailCount >= _scenario.MaxFailCount then
                    stop()
                    scnCtx.ExecStopCommand(StopCommand.StopTest $"Stopping test because of too many fails. Scenario '{_scenario.ScenarioName}' contains '{scnCtx.ScenarioStatsActor.ScenarioFailCount}' fails.")

                let timeProgress = LoadSimulation.calcTimeProgress currentTime duration

                let struct (command, copiesCount) =
                    schedule getRandomValue simulation timeProgress _constantScheduler.ScheduledActorCount

                match command with
                | SchedulerCommand.AddConstantActors    -> _constantScheduler.AddActors copiesCount
                | SchedulerCommand.RemoveConstantActors -> _constantScheduler.RemoveActors copiesCount
                | SchedulerCommand.InjectOneTimeActors  -> _oneTimeScheduler.InjectActors copiesCount
                | _ -> ()

                do! Task.Delay interval
                currentTime <- currentTime + interval

        stop()
    }

    member _.Working = _isWorking
    member _.Scenario = _scenario
    member _.AllRealtimeStats = scnCtx.ScenarioStatsActor.AllRealtimeStats
    member _.ConsoleScenarioStats = scnCtx.ScenarioStatsActor.ConsoleScenarioStats

    member _.Start() =
        if scnCtx.ScenarioOperation = ScenarioOperation.WarmUp && _scenario.WarmUpDuration.IsSome then
            _warmupTimer <- new Timer(_scenario.WarmUpDuration.Value.TotalMilliseconds)
            _warmupTimer.Elapsed.Add(fun _ -> stop())
            _warmupTimer.Start()

        start() :> Task

    member _.Stop() = stop()

    member _.AddStatsFromAgent(stats) = scnCtx.ScenarioStatsActor.Publish(AddFromAgent stats)
    member _.PrepareForRealtimeStats() = prepareForRealtimeStats()
    member _.CommitRealtimeStats(duration) = commitRealtimeStats duration
    member _.BuildRealtimeStats(duration) = buildRealtimeStats duration
    member _.GetFinalStats() = getFinalStats()

    interface IDisposable with
        member _.Dispose() = stop()
