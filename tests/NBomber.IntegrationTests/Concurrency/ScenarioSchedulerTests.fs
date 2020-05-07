module Tests.ScenarioScheduler

open System

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Domain.Concurrency.Scheduler
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.FSharp

[<Theory>]
[<InlineData(100, 200, 50, 150)>]    // ramp-up
[<InlineData(0, 100, 99, 99)>]       // ramp-up
[<InlineData(70, 100, 1, 70)>]       // ramp-up
[<InlineData(70, 100, 100, 100)>]    // ramp-up
[<InlineData(300, 100, 50, 200)>]    // ramp-down
[<InlineData(1_000, 100, 99, 109)>]  // ramp-down
[<InlineData(1_000, 100, 100, 100)>] // ramp-down
[<InlineData(1_000, 100, 2, 982)>]   // ramp-down
let ``schedule should correctly handle ramp-up or ramp-down for RampScenariosPerSec``
    (prevSegmentCopiesCount: int, copiesCount: int, timeProgress: int, result: int) =

    let constWorkingActorCount = 0

    let timeSegment = {
        StartTime = TimeSpan.MinValue
        EndTime = TimeSpan.MinValue
        Duration = TimeSpan.MinValue
        PrevSegmentCopiesCount = prevSegmentCopiesCount
        LoadSimulation = RampScenariosPerSec(copiesCount, TimeSpan.MinValue)
    }

    match ScenarioScheduler.schedule(timeSegment, timeProgress, constWorkingActorCount).Head with
    | InjectOneTimeActors count -> test <@ count = result @>
    | _                         -> failwith "invalid command"

[<Theory>]
[<InlineData(0, 20, 0, 3, 1)>]       // ramp-up
[<InlineData(0, 100, 50, 51, 1)>]    // ramp-up
[<InlineData(100, 200, 130, 31, 1)>] // ramp-up
[<InlineData(0, 100, 90, 95, 5)>]    // ramp-up
let ``schedule should correctly handle ramp-up for RampConcurrentScenarios``
    (prevSegmentCopiesCount: int, copiesCount: int, constWorkingActorCount: int, timeProgress: int, result: int) =

    let timeSegment = {
        StartTime = TimeSpan.MinValue
        EndTime = TimeSpan.MinValue
        Duration = TimeSpan.MinValue
        PrevSegmentCopiesCount = prevSegmentCopiesCount
        LoadSimulation = RampConcurrentScenarios(copiesCount, TimeSpan.MinValue)
    }

    match ScenarioScheduler.schedule(timeSegment, timeProgress, constWorkingActorCount).Head with
    | AddConstantActors count -> test <@ count = result @>
    | _                       -> failwith "invalid command"

[<Theory>]
[<InlineData(100, 0, 50, 51, 1)>]    // ramp-down
[<InlineData(200, 100, 170, 31, 1)>] // ramp-down
[<InlineData(100, 0, 10, 95, 5)>]    // ramp-down
let ``schedule should correctly handle ramp-down for RampConcurrentScenarios``
    (prevSegmentCopiesCount: int, copiesCount: int, constWorkingActorCount: int, timeProgress: int, result: int) =

    let timeSegment = {
        StartTime = TimeSpan.MinValue
        EndTime = TimeSpan.MinValue
        Duration = TimeSpan.MinValue
        PrevSegmentCopiesCount = prevSegmentCopiesCount
        LoadSimulation = RampConcurrentScenarios(copiesCount, TimeSpan.MinValue)
    }

    match ScenarioScheduler.schedule(timeSegment, timeProgress, constWorkingActorCount).Head with
    | RemoveConstantActors count -> test <@ count = result @>
    | _                          -> failwith "invalid command"

[<Property>]
let ``schedule should correctly handle KeepConcurrentScenarios``
    (prevSegmentCopiesCount: uint32, copiesCount: uint32, constWorkingActorCount: uint32, timeProgress: uint32) =

    // condition
    copiesCount > 0u ==> lazy

    let timeSegment = {
        StartTime = TimeSpan.MinValue
        EndTime = TimeSpan.MinValue
        Duration = TimeSpan.MinValue
        PrevSegmentCopiesCount = int prevSegmentCopiesCount
        LoadSimulation = KeepConcurrentScenarios(int copiesCount, TimeSpan.MinValue)
    }

    let commands = ScenarioScheduler.schedule(timeSegment, int timeProgress, int constWorkingActorCount)
    commands
    |> List.iter(function
        | AddConstantActors scheduled    -> test <@ scheduled <> 0 @>
                                            test <@ scheduled + int constWorkingActorCount = int copiesCount @>

        | RemoveConstantActors scheduled -> test <@ scheduled <> 0 @>
                                            test <@ int constWorkingActorCount - scheduled = int copiesCount @>

        | DoNothing                      -> test <@ copiesCount = constWorkingActorCount @>

        | _ -> failwith "invalid command"
    )

    test <@ commands.Length = 1 @>

[<Property>]
let ``schedule should correctly handle InjectScenariosPerSec``
    (prevSegmentCopiesCount: uint32, copiesCount: uint32, constWorkingActorCount: uint32, timeProgress: uint32) =

    // condition
    copiesCount > 0u ==> lazy

    let commandCount = ref 1 // for some reason F# doesn't compile mutable commandCount with FsCheck(==> lazy)

    let timeSegment = {
        StartTime = TimeSpan.MinValue
        EndTime = TimeSpan.MinValue
        Duration = TimeSpan.MinValue
        PrevSegmentCopiesCount = int prevSegmentCopiesCount
        LoadSimulation = InjectScenariosPerSec(int copiesCount, TimeSpan.MinValue)
    }

    let commands = ScenarioScheduler.schedule(timeSegment, int timeProgress, int constWorkingActorCount)
    commands
    |> List.iter(function
        | InjectOneTimeActors scheduled  -> test <@ scheduled <> 0 @>

        | RemoveConstantActors scheduled -> test <@ scheduled <> 0 @>
                                            commandCount := 2

        | _ -> failwith "invalid command"
    )

    test <@ commands.Length = commandCount.Value @>

[<Property>]
let ``schedule should correctly handle RampScenariosPerSec``
    (prevSegmentCopiesCount: uint32, copiesCount: uint32, constWorkingActorCount: uint32, timeProgress: uint32) =

    // condition
    (copiesCount > 5u && timeProgress > 10u && timeProgress <= 100u) ==> lazy

    let commandCount = ref 1 // for some reason F# doesn't compile mutable commandCount with FsCheck(==> lazy)

    let timeSegment = {
        StartTime = TimeSpan.MinValue
        EndTime = TimeSpan.MinValue
        Duration = TimeSpan.MinValue
        PrevSegmentCopiesCount = int prevSegmentCopiesCount
        LoadSimulation = RampScenariosPerSec(int copiesCount, TimeSpan.MinValue)
    }

    let commands = ScenarioScheduler.schedule(timeSegment, int timeProgress, int constWorkingActorCount)
    commands
    |> List.iter(function
        | InjectOneTimeActors scheduled  -> test <@ scheduled <> 0 @>

        | RemoveConstantActors scheduled -> test <@ scheduled <> 0 @>
                                            commandCount := 2

        | _ -> failwith "invalid command"
    )

    test <@ commands.Length = commandCount.Value @>

[<Fact>]
let ``should run InjectOneTimeActors correctly`` () =

    let step1 = Step.create("step_1", fun context -> task {
        return Response.Ok(42)
    })

    let scenario = Scenario.create "hello_world_scenario" [step1;] //step2]
                   |> Scenario.withoutWarmUp
                   |> Scenario.withLoadSimulations [
                       InjectScenariosPerSec(copiesCount = 1, during = TimeSpan.FromSeconds 20.0)
                   ]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.runTest Array.empty
    |> Result.map(fun nodeStats ->
        let reqCount = nodeStats.RequestCount
        test <@ reqCount >= 20 && reqCount <= 21 @>
    )
    |> Result.mapError(failwith)
    |> ignore
