module Tests.TimeLineTests

open System

open Xunit
open FsCheck.Xunit
open Swensen.Unquote
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.ScenarioTimeLine

[<Property>]
let ``TimeLine.build should correctly calculate and order time within timeline`` (strategies: ConcurrencyStrategy list) =
    result {
        let! timeLine = ScenarioTimeLine.build(TimeSpan.Zero, strategies)

        let startTime = List.tryHead timeLine |> Option.map fst |> Option.defaultValue TimeSpan.Zero
        let endTime = List.tryLast timeLine |> Option.map fst |> Option.defaultValue TimeSpan.Zero
        let ascendingOrderByTime = timeLine |> List.sortBy(fun (time,_) -> time)

        test <@ timeLine.Length = strategies.Length  @>
        test <@ startTime <= endTime @>
        test <@ timeLine = ascendingOrderByTime @>
    }
    |> ignore

[<Property>]
let ``TimeLine.getRunningStrategy should correctly determine and return running timeline`` (currentTimeTicks: int32,
                                                                                            timeLine: ScenarioTimeLine) =
    let currentTime = TimeSpan(int64 currentTimeTicks)
    let strategy = ScenarioTimeLine.getRunningStrategy(timeLine, currentTime)

    match strategy with
    | Some v ->
        let runningStrategy =
            timeLine
            |> List.find(fun (endTime,_) -> currentTime <= endTime)
            |> snd

        test <@ runningStrategy = v @>
        test <@ timeLine.Length > 0 @>

    | None ->
        let timeIsOver = timeLine |> List.forall(fun (endTime,_) -> currentTime > endTime)
        test <@ timeIsOver = true @>
