module Tests.LoadSimulationTests

open System

open FsCheck.Xunit
open Swensen.Unquote
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Concurrency

[<Property>]
let ``LoadSimulation.createTimeLine should correctly calculate and order time within timeline`` (simulations: LoadSimulation list) =
    result {
        let! timeLine = LoadSimulation.createTimeLine(TimeSpan.Zero, simulations)

        let startTime = List.tryHead timeLine |> Option.map fst |> Option.defaultValue TimeSpan.Zero
        let endTime = List.tryLast timeLine |> Option.map fst |> Option.defaultValue TimeSpan.Zero
        let ascendingOrderByTime = timeLine |> List.sortBy(fun (time,_) -> time)

        test <@ timeLine.Length = simulations.Length  @>
        test <@ startTime <= endTime @>
        test <@ timeLine = ascendingOrderByTime @>
    }
    |> ignore

[<Property>]
let ``LoadSimulation.getRunningSimulation should correctly determine and return running timeline``
    (currentTimeTicks: int32, timeLine: LoadTimeLine) =

    let currentTime = TimeSpan(int64 currentTimeTicks)
    match LoadSimulation.getRunningSimulation(timeLine, currentTime) with
    | Some simulation ->
        let runningSimulation =
            timeLine
            |> List.find(fun (endTime,_) -> currentTime <= endTime)
            |> snd

        test <@ runningSimulation = simulation @>
        test <@ timeLine.Length > 0 @>

    | None ->
        let timeIsOver = timeLine |> List.forall(fun (endTime,_) -> currentTime > endTime)
        test <@ timeIsOver = true @>
