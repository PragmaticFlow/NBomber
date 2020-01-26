module Tests.TimeLineTests

open System

open Xunit
open FsCheck.Xunit
open Swensen.Unquote
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.TimeLine

[<Property>]
let ``TimeLine.build should correctly calculate timeline`` (strategies: ConcurrencyStrategy list) =
    result {
        let! timeLine = TimeLine.build(TimeSpan.Zero, strategies)

        let head = List.tryHead timeLine |> Option.map fst |> Option.defaultValue TimeSpan.Zero
        let tail = List.tryLast timeLine |> Option.map fst |> Option.defaultValue TimeSpan.Zero

        test <@ timeLine.Length = strategies.Length  @>
        test <@ head <= tail @>
    }
    |> ignore

[<Property>]
let ``TimeLine.getRunningStrategy should correctly determine and return running timeline`` (currentTimeTicks: int32,
                                                                                            timeLine: TimeLine) =
    let currentTime = TimeSpan(int64 currentTimeTicks)
    let strategy = TimeLine.getRunningStrategy(timeLine, currentTime)

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
