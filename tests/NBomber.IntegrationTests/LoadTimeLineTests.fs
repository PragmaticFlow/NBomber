module Tests.LoadTimeLine

open System

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes

[<Property>]
let ``createTimeLine should correctly calculate and order time within timeline`` (simulations: LoadSimulation list) =
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
let internal ``getRunningTimeSegment should correctly determine and return running time segment``
    (currentTimeTicks: uint32, timeLine: LoadTimeLine) =

    let currentTime = TimeSpan(int64 currentTimeTicks)
    match LoadTimeLine.getRunningTimeSegment(timeLine, currentTime) with
    | Some timeSegment ->

        let runningSimulation =
            timeLine
            |> List.pick(fun x -> if currentTime <= x.EndTime then Some x.LoadSimulation else None)

        test <@ runningSimulation = timeSegment.LoadSimulation @>
        test <@ timeLine.Length > 0 @>

    | None ->
        let timeIsOver = timeLine |> List.forall(fun x -> currentTime > x.EndTime)
        test <@ timeIsOver = true @>

[<Fact>]
let ``calcTimeSegmentProgress should correctly calculate progress for concrete segment`` () =

    let currentTime = TimeSpan.FromSeconds 22.0
    let startTime = TimeSpan.FromSeconds 20.0
    let endTime = TimeSpan.FromSeconds 60.0
    let duration = endTime - startTime

    let timeSegment_1 = {
        StartTime = startTime
        EndTime = endTime
        Duration = duration
        PrevSegmentCopiesCount = 0
        LoadSimulation = LoadSimulation.KeepConstant(10, duration)
    }

    let timeSegment_2 = {
        StartTime = startTime.Add startTime
        EndTime = endTime.Add startTime
        Duration = duration
        PrevSegmentCopiesCount = 0
        LoadSimulation = LoadSimulation.KeepConstant(10, duration)
    }

    let progress_1 = LoadTimeLine.calcTimeSegmentProgress currentTime timeSegment_1
    let progress_2 = LoadTimeLine.calcTimeSegmentProgress (currentTime.Add startTime) timeSegment_2

    test <@ progress_1 = 5 @>
    test <@ progress_1 = progress_2 @>
