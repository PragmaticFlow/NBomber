[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.LoadSimulation

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open FsToolkit.ErrorHandling
open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Extensions.Internal
open NBomber.Errors
open NBomber.Domain.DomainTypes

module Validation =

    let private validateSimulation (scnName: string) (simulation: Contracts.LoadSimulation) = result {
        let checkCopies copies =
            if copies < 0 then
                Error (CopiesCountIsNegative(scnName, simulation))
            else
                Ok copies

        let checkRate rate =
            if rate < 0 then
                Error (RateIsNegative(scnName, simulation))
            else
                Ok rate

        let checkMinMaxRate minRate maxRate =
            if minRate >= maxRate then
                Error (MinRateIsBiggerThanMax(scnName, simulation))
            else
                Ok ()

        let checkInterval interval duration =
            if interval > duration then
                Error (IntervalIsBiggerThanDuration(scnName, simulation))
            else
                Ok interval

        let checkDuration duration =
            if duration < Constants.MinSimulationDuration then
                Error (DurationIsSmallerThanMin(scnName, simulation))
            else
                Ok duration

        match simulation with
        | RampingConstant (copies, duration)
        | KeepConstant (copies, duration) ->
            let! _ = checkCopies copies
            let! _ = checkDuration duration
            return simulation

        | RampingInject (rate, interval, duration)
        | Inject (rate, interval, duration) ->
            let! _ = checkRate rate
            let! _ = checkInterval interval duration
            let! _ = checkDuration duration
            return simulation

        | InjectRandom (minRate, maxRate, interval, duration) ->
            let! _ = checkRate minRate
            let! _ = checkRate maxRate
            let! _ = checkMinMaxRate minRate maxRate
            let! _ = checkInterval interval duration
            let! _ = checkDuration duration
            return simulation

        | Pause duration ->
            let! _ = checkDuration duration
            return simulation
    }

    let private validateOnEmpty (simulations: Contracts.LoadSimulation list) =
        if List.isEmpty simulations then
            Error EmptySimulationsList
        else
            Ok simulations

    let validate (scnName: string) (simulations: Contracts.LoadSimulation list) = result {
        let _ = validateOnEmpty simulations

        return!
            simulations
            |> Seq.map(validateSimulation scnName)
            |> Result.sequence
            |> Result.mapError Seq.head
    }

let private createSimulation (startTime) (prevCopiesCount) (simulation: Contracts.LoadSimulation) =
    match simulation with
    | RampingConstant (_, duration)
    | KeepConstant (_, duration) ->
        let endTime = startTime + duration
        { Value = simulation
          StartTime = startTime
          EndTime = endTime
          Duration = duration
          PrevActorCount = prevCopiesCount }

    | RampingInject (_, _, duration)
    | Inject (_, _, duration) ->
        let endTime = startTime + duration
        { StartTime = startTime
          EndTime = endTime
          Duration = duration
          PrevActorCount = prevCopiesCount
          Value = simulation }

    | InjectRandom (_, _, _, duration) ->
        let endTime = startTime + duration
        { StartTime = startTime
          EndTime = endTime
          Duration = duration
          PrevActorCount = 0
          Value = simulation }

    | Pause duration ->
        let endTime = startTime + duration
        { StartTime = startTime
          EndTime = endTime
          Duration = duration
          PrevActorCount = prevCopiesCount
          Value = simulation }

let getPlanedDuration (simulations: LoadSimulation list) =
    simulations
    |> List.map(fun x -> x.Duration)
    |> List.fold(fun st v -> st + v) TimeSpan.Zero

let calcExecutedDuration (simulations: LoadSimulation list) (currentTime: TimeSpan) =
    let execPauseTime =
        simulations
        |> List.choose(fun x ->
            match x.Value with
            | Pause _ when x.EndTime <= currentTime -> Some x.Duration
            | _ -> None
        )
        |> List.fold(fun st v -> st + v) TimeSpan.Zero

    currentTime - execPauseTime

let create (scnName:  string) (simulations: Contracts.LoadSimulation list) =

    let getPrevCopiesCount (simulation) =
        match simulation with
        | RampingConstant (copies, _)
        | KeepConstant (copies, _) -> copies

        | RampingInject (rate, _, _)
        | Inject (rate, _, _) -> rate

        | InjectRandom (_, maxRate, _, _) -> maxRate
        | Pause _ -> 0

    result {
        let! all = Validation.validate scnName simulations
        let initState = createSimulation TimeSpan.Zero 0 (KeepConstant(0, TimeSpan.Zero))

        let simulations =
            all
            |> List.scan(fun prevState simulation ->
                let prevCopiesCount = getPrevCopiesCount prevState.Value
                createSimulation prevState.EndTime prevCopiesCount simulation) initState

            |> List.filter(fun x -> x.EndTime > TimeSpan.Zero)

        return simulations
    }
    |> Result.mapError AppError.create

let inline getSimulationInterval simulation =
    match simulation with
    | RampingConstant (copies, during)      -> Constants.ONE_SECOND
    | KeepConstant (copies, during)         -> Constants.ONE_SECOND
    | RampingInject (_, interval, during)   -> interval
    | Inject (rate, interval, during)       -> interval
    | InjectRandom (_, _, interval, during) -> interval
    | Pause during                          -> Constants.ONE_SECOND

let inline getSimulationName simulation =
    match simulation with
    | RampingConstant _ -> "ramping_constant"
    | KeepConstant _    -> "keep_constant"
    | RampingInject _   -> "ramping_inject"
    | Inject _          -> "inject"
    | InjectRandom _    -> "inject_random"
    | Pause _           -> "pause"

[<MethodImpl(MethodImplOptions.AggressiveInlining)>]
let calcTimeProgress (currentTime: TimeSpan) (duration: TimeSpan) =
    let result = currentTime.TotalMilliseconds / duration.TotalMilliseconds * 100.0
    let progress = int(Math.Round(result, 0, MidpointRounding.AwayFromZero))
    // correction
    if progress > 100 then 100
    else progress

let inline createSimulationStats simulation constantActorCount onetimeActorCount =
    let value =
        match simulation with
        | RampingConstant _ -> constantActorCount
        | KeepConstant _    -> constantActorCount
        | RampingInject _   -> onetimeActorCount
        | Inject _          -> onetimeActorCount
        | InjectRandom _    -> onetimeActorCount
        | Pause _           -> 0

    { SimulationName = getSimulationName simulation; Value = value }

module TimeLineHistory =

    let create (schedulersRealtimeStats: IReadOnlyDictionary<TimeSpan,ScenarioStats> seq) =
        schedulersRealtimeStats
        |> Seq.collect id
        |> Seq.groupBy(fun x -> x.Key)
        |> Seq.map(fun (duration, scnStats) ->
            let data = scnStats |> Seq.map(fun item ->  item.Value) |> Seq.toArray
            { ScenarioStats = data; Duration = duration }
        )
        |> Seq.sortBy(fun x -> x.Duration)
        |> Seq.toArray

module TimeLineHistoryRecord =

    let create (scnStats: ScenarioStats[]) = {
        Duration = scnStats[0].Duration
        ScenarioStats = scnStats
    }
