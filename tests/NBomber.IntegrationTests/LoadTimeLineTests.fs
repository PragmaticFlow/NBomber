module Tests.LoadTimeLine

open System

open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain

[<Property>]
let ``LoadTimeLine.createTimeLine should correctly calculate and order time within timeline`` (simulations: LoadSimulation list) =
    result {
        let! timeLine = LoadTimeLine.createTimeLine(simulations)

        let startTime =
            timeLine
            |> List.tryHead
            |> Option.map(fun x -> x.EndTime)
            |> Option.defaultValue TimeSpan.Zero

        let endTime =
            timeLine
            |> List.tryLast
            |> Option.map(fun x -> x.EndTime)
            |> Option.defaultValue TimeSpan.Zero

        let ascendingOrderByTime = timeLine |> List.sortBy(fun x -> x.EndTime)

        test <@ timeLine.Length = simulations.Length  @>
        test <@ startTime <= endTime @>
        test <@ timeLine = ascendingOrderByTime @>
    }
    |> ignore

[<Property>]
let ``LoadTimeLine.getRunningSimulation should correctly determine and return running timeline``
    (currentTimeTicks: uint32, timeLine: LoadTimeLine) =

    let currentTime = TimeSpan(int64 currentTimeTicks)
    match LoadTimeLine.getRunningSimulation(timeLine, currentTime) with
    | Some simulation ->

        let runningSimulation =
            timeLine
            |> List.pick(fun x -> if currentTime <= x.EndTime then Some x.LoadSimulation else None)

        test <@ runningSimulation = simulation @>
        test <@ timeLine.Length > 0 @>

    | None ->
        let timeIsOver = timeLine |> List.forall(fun x -> currentTime > x.EndTime)
        test <@ timeIsOver = true @>
