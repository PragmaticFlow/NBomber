[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.LoadTimeLine

open System
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain.DomainTypes

let validateSimulation (simulation: LoadSimulation) =
    result {
        let checkCopies (copies) =
            if copies <= 0 then
                simulation.ToString() |> CopiesCountIsZeroOrNegative |> AppError.createResult
            else Ok copies

        let checkDuration (duration) =
            if duration < TimeSpan.FromSeconds(1.0) then
                simulation.ToString() |> DurationIsLessThan1Sec |> AppError.createResult

            elif duration > TimeSpan.FromDays(10.0) then
                simulation.ToString() |> DurationIsBiggerThan10Days |> AppError.createResult

            else Ok duration

        match simulation with
        | KeepConcurrentScenarios (copies, duration)
        | RampConcurrentScenarios (copies, duration)
        | InjectScenariosPerSec (copies, duration)
        | RampScenariosPerSec (copies, duration) ->
            let! _ = checkCopies copies
            let! _ = checkDuration duration
            return simulation
    }

let createTimeLine (simulations: LoadSimulation list) =

    let rec create (startTime: TimeSpan, simulations: LoadSimulation list) =
        result {
            match simulations with
            | [] -> return List.empty
            | simulation :: tail ->
                match! validateSimulation simulation with
                | KeepConcurrentScenarios (_, duration)
                | RampConcurrentScenarios (_, duration)
                | InjectScenariosPerSec (_, duration)
                | RampScenariosPerSec (_, duration) ->
                    let endTime = startTime + duration
                    let timeLineItem = { EndTime = endTime; LoadSimulation = simulation }
                    let! timeLine = create(endTime, tail)
                    return timeLineItem :: timeLine
        }

    create(TimeSpan.Zero, simulations)

let createWithDuration (loadSimulations: Contracts.LoadSimulation list) = result {
    let! timeLine = loadSimulations |> createTimeLine
    let timeItem = timeLine |> List.last
    return {| LoadTimeLine = timeLine; ScenarioDuration = timeItem.EndTime |}
}

let createSimulationFromSettings (settings: Configuration.LoadSimulationSettings) =
    match settings with
    | Configuration.KeepConcurrentScenarios (c,d) -> KeepConcurrentScenarios(c, d.TimeOfDay)
    | Configuration.RampConcurrentScenarios (c,d) -> LoadSimulation.RampConcurrentScenarios(c, d.TimeOfDay)
    | Configuration.InjectScenariosPerSec (c,d)   -> LoadSimulation.InjectScenariosPerSec(c, d.TimeOfDay)
    | Configuration.RampScenariosPerSec (c,d)     -> LoadSimulation.RampScenariosPerSec(c, d.TimeOfDay)

let getRunningSimulation (timeLine: LoadTimeLine, currentTime: TimeSpan) =
    timeLine
    |> List.tryFind(fun x -> currentTime <= x.EndTime)
    |> Option.map(fun x -> x.LoadSimulation)
