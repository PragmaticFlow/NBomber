[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.LoadTimeLine

open System
open System.Globalization

open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Errors
open NBomber.Domain.DomainTypes

module Validation =

    let validate (simulation: LoadSimulation) =
        result {
            let checkCopies (copies) =
                if copies <= 0 then
                    simulation.ToString() |> CopiesCountIsZeroOrNegative |> AppError.createResult
                else Ok copies

            let checkRate (rate) =
                if rate <= 0 then
                    simulation.ToString() |> RateIsZeroOrNegative |> AppError.createResult
                else Ok rate

            let checkDuration (duration) =
                if duration < Constants.MinSimulationDuration then
                    simulation.ToString() |> SimulationIsSmallerThanMin |> AppError.createResult

                elif duration > Constants.MaxSimulationDuration then
                    simulation.ToString() |> SimulationIsBiggerThanMax |> AppError.createResult

                else Ok duration

            match simulation with
            | RampConstant (copies, duration)
            | KeepConstant (copies, duration) ->
                let! _ = checkCopies copies
                let! _ = checkDuration duration
                return simulation

            | RampPerSec (rate, duration)
            | InjectPerSec (rate, duration) ->
                let! _ = checkRate rate
                let! _ = checkDuration duration
                return simulation

            | InjectPerSecRandom (minRate, maxRate, duration) ->
                let! _ = checkRate minRate
                let! _ = checkRate maxRate
                let! _ = checkDuration duration
                return simulation
        }

module TimeLineHistory =

    let filterRealtime (history: TimeLineHistoryRecord list) =

        let filterBombing (history: TimeLineHistoryRecord list) =
            history
            |> List.filter(fun x -> x.ScenarioStats |> Array.exists(fun x -> x.CurrentOperation = OperationType.Bombing))

        history
        |> List.sortBy(fun x -> x.Duration)
        |> List.groupBy(fun x -> x.Duration)
        |> List.collect(fun (duration,history) ->
            if history.Length > 1 then history |> filterBombing
            else history
        )

module TimeLineHistoryRecord =

    let create (scnStats: ScenarioStats[]) = {
        Duration = scnStats[0].Duration
        ScenarioStats = scnStats
    }

let createTimeLine (simulations: LoadSimulation list) =

    let rec create (startTime: TimeSpan, prevSegmentCopiesCount: int, simulations: LoadSimulation list) =
        result {
            match simulations with
            | [] -> return List.empty
            | simulation :: tail ->
                match! Validation.validate simulation with
                | RampConstant (copiesCount, duration)
                | KeepConstant (copiesCount, duration)
                | RampPerSec (copiesCount, duration)
                | InjectPerSec (copiesCount, duration) ->
                    let endTime = startTime + duration
                    let timeSegment = {
                        StartTime = startTime
                        EndTime = endTime
                        Duration = duration
                        PrevSegmentCopiesCount = prevSegmentCopiesCount
                        LoadSimulation = simulation
                    }
                    let! timeLine = create(endTime, copiesCount, tail)
                    return timeSegment :: timeLine

                | InjectPerSecRandom (_, maxRate, duration) ->
                    let endTime = startTime + duration
                    let timeSegment = {
                        StartTime = startTime
                        EndTime = endTime
                        Duration = duration
                        PrevSegmentCopiesCount = prevSegmentCopiesCount
                        LoadSimulation = simulation
                    }
                    let! timeLine = create(endTime, maxRate, tail)
                    return timeSegment :: timeLine
        }

    create(TimeSpan.Zero, 0, simulations)

let createWithDuration (loadSimulations: LoadSimulation list) = result {
    let! timeLine = loadSimulations |> createTimeLine
    let timeItem = timeLine |> List.last
    return {| LoadTimeLine = timeLine; ScenarioDuration = timeItem.EndTime |}
}

let createSimulationFromSettings (settings: Configuration.LoadSimulationSettings) =
    match settings with
    | Configuration.RampConstant (c,time) -> RampConstant(c, TimeSpan.ParseExact(time, "hh\:mm\:ss", CultureInfo.InvariantCulture))
    | Configuration.KeepConstant (c,time) -> KeepConstant(c, TimeSpan.ParseExact(time, "hh\:mm\:ss", CultureInfo.InvariantCulture))
    | Configuration.RampPerSec (c,time)   -> RampPerSec(c, TimeSpan.ParseExact(time, "hh\:mm\:ss", CultureInfo.InvariantCulture))
    | Configuration.InjectPerSec (c,time) -> InjectPerSec(c, TimeSpan.ParseExact(time, "hh\:mm\:ss", CultureInfo.InvariantCulture))

let getSimulationName (simulation) =
    match simulation with
    | RampConstant _   -> "ramp_constant"
    | KeepConstant _   -> "keep_constant"
    | RampPerSec     _ -> "ramp_per_sec"
    | InjectPerSec   _ -> "inject_per_sec"
    | InjectPerSecRandom _ -> "inject_per_sec_random"

let getRunningTimeSegment (timeLine: LoadTimeLine, currentTime: TimeSpan) =
    timeLine
    |> List.tryFind(fun x -> currentTime <= x.EndTime)

let calcTimeSegmentProgress (currentTime: TimeSpan) (timeSegment: LoadTimeSegment) =
    let relativeTimePointOnSegment = currentTime.TotalMilliseconds - timeSegment.StartTime.TotalMilliseconds
    let result = relativeTimePointOnSegment / timeSegment.Duration.TotalMilliseconds * 100.0
    int(Math.Round(result, 0, MidpointRounding.AwayFromZero))

let correctTimeProgress (progress: int) =
    let result = progress + 5
    if result > 100 then 100
    else result

let createSimulationStats (simulation: LoadSimulation,
                           constantActorCount: int,
                           onetimeActorCount: int) =
    let value =
        match simulation with
        | RampConstant _ -> constantActorCount
        | KeepConstant _ -> constantActorCount
        | RampPerSec   _ -> onetimeActorCount
        | InjectPerSec _ -> onetimeActorCount
        | InjectPerSecRandom _ -> onetimeActorCount

    { SimulationName = getSimulationName(simulation); Value = value }
