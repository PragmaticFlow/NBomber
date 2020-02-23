module internal NBomber.Domain.Concurrency.LoadSimulation

open System
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain
open NBomber.Errors

let validateSimulation (simulation: LoadSimulation) =
    result {
        let checkZeroCopies (copies) =
            if copies <= 0 then AppError.createResult(CopiesCountZero) // NumberIsZeroOrNegative()
            else Ok copies

        let checkDuration (duration) =
            if duration < TimeSpan.FromSeconds(1.0) then AppError.createResult(DurationIsLessThan1Sec duration)
            else Ok duration

        match simulation with
        | KeepConcurrentScenarios (copies, duration)
        | RampConcurrentScenarios (copies, duration)
        | InjectScenariosPerSec (copies, duration)
        | RampScenariosPerSec (copies, duration) ->
            let! vCopies = checkZeroCopies copies
            let! vDuration = checkDuration duration
            return simulation
    }

let rec createTimeLine (startTime: TimeSpan, simulations: LoadSimulation list): Result<LoadTimeLine,AppError> =
    result {
        match simulations with
        | [] -> return List.empty
        | simulation :: tail ->
            match! validateSimulation(simulation) with
            | KeepConcurrentScenarios (_, duration)
            | RampConcurrentScenarios (_, duration)
            | InjectScenariosPerSec (_, duration)
            | RampScenariosPerSec (_, duration) ->
                let endTime = startTime + duration
                let! timeLine = createTimeLine(endTime, tail)
                return (endTime, simulation) :: timeLine
    }

let getRunningSimulation (timeLine: LoadTimeLine, currentTime: TimeSpan) =
    timeLine
    |> List.tryFind(fun (endTime,_) -> currentTime <= endTime)
    |> Option.map snd
