module internal NBomber.Domain.Concurrency.ScenarioTimeLine

open System
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Errors

type EndTime = TimeSpan
type ScenarioTimeLine = (EndTime * ConcurrencyStrategy) list

let validateStrategy (strategy: ConcurrencyStrategy) =
    result {
        let checkZeroCopies (copiesCount) =
            if copiesCount = 0u then AppError.createResult(CopiesCountZero)
            else Ok copiesCount

        let checkDuration (duration) =
            if duration >= TimeSpan.FromSeconds(1.0) then AppError.createResult(DurationIsLessThan1Sec duration)
            else Ok duration

        match strategy with
        | Constant (copiesCount, duration) ->
            let! vCopiesCount = checkZeroCopies copiesCount
            let! vDuration = checkDuration duration
            return strategy

        | AddPerSec (copiesCount, duration) ->
            let! vCopiesCount = checkZeroCopies copiesCount
            let! vDuration = checkDuration duration
            return strategy

        | RampTo (copiesCount, duration) ->
            let! vCopiesCount = checkZeroCopies copiesCount
            let! vDuring = checkDuration duration
            return strategy
    }

let rec build (startTime: TimeSpan, strategies: ConcurrencyStrategy list): Result<ScenarioTimeLine,AppError> =
    result {
        match strategies with
        | [] -> return List.empty
        | strategy::tail ->
            match! validateStrategy(strategy) with
            | Constant (copiesCount, duration) ->
                let endTime = startTime + duration
                let! timeLine = build(endTime, tail)
                return (endTime, strategy) :: timeLine

            | AddPerSec (copiesCount, duration) ->
                let endTime = startTime + duration
                let! timeLine = build(endTime, tail)
                return (endTime, strategy) :: timeLine

            | RampTo (copiesCount, during) ->
                let endTime = startTime + during
                let! timeLine = build(endTime, tail)
                return (endTime, strategy) :: timeLine
    }

let getRunningStrategy (timeLine: ScenarioTimeLine, currentTime: TimeSpan) =
    timeLine
    |> List.tryFind(fun (endTime,_) -> currentTime <= endTime)
    |> Option.map(snd)
