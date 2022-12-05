module Tests.Concurrency.ScenarioScheduler

open System
open Swensen.Unquote
open Xunit
open FsToolkit.ErrorHandling
open NBomber
open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Domain.Scheduler
open NBomber.Domain.Scheduler.ScenarioScheduler
open NBomber.FSharp

let getRandomValue minRate maxRate = 42

[<Theory>]
[<InlineData(100, 200, 50, 150)>]    // ramp-up
[<InlineData(0, 100, 99, 99)>]       // ramp-up
[<InlineData(70, 100, 1, 70)>]       // ramp-up
[<InlineData(70, 100, 100, 100)>]    // ramp-up
[<InlineData(300, 100, 50, 200)>]    // ramp-down
[<InlineData(1_000, 100, 99, 109)>]  // ramp-down
[<InlineData(1_000, 100, 100, 100)>] // ramp-down
[<InlineData(1_000, 100, 2, 982)>]   // ramp-down
[<InlineData(1_000, 0, 100, 0)>]   // ramp-down
let ``schedule should correctly handle ramp-up or ramp-down for RampingInject``
    (prevCopiesCount: int, rate: int, timeProgress: int, result: int) =

    let constWorkingActorCount = 0

    let simulation = {
        Value = RampingInject(rate, TimeSpan.Zero, TimeSpan.Zero)
        StartTime = TimeSpan.Zero
        EndTime = TimeSpan.Zero
        Duration = TimeSpan.Zero
        PrevActorCount = prevCopiesCount
    }

    let struct (command, copiesCount) =
        ScenarioScheduler.Test.schedule getRandomValue simulation timeProgress constWorkingActorCount

    match command with
    | SchedulerCommand.InjectOneTimeActors -> test <@ copiesCount = result @>
    | _                                    -> failwith "invalid command"

[<Theory>]
[<InlineData(100, 200, 50, 200)>] // inject
[<InlineData(0, 100, 99, 100)>]   // inject
[<InlineData(70, 100, 1, 100)>]   // inject
[<InlineData(70, 0, 100, 0)>]     // inject
let ``schedule should correctly handle Inject``
    (prevCopiesCount: int, rate: int, timeProgress: int, result: int) =

    let constWorkingActorCount = 0

    let simulation = {
        Value = Inject(rate, TimeSpan.Zero, TimeSpan.Zero)
        StartTime = TimeSpan.Zero
        EndTime = TimeSpan.Zero
        Duration = TimeSpan.Zero
        PrevActorCount = prevCopiesCount
    }

    let struct (command, copiesCount) =
        ScenarioScheduler.Test.schedule getRandomValue simulation timeProgress constWorkingActorCount

    match command with
    | SchedulerCommand.InjectOneTimeActors -> test <@ copiesCount = result @>
    | _                                    -> failwith "invalid command"

[<Theory>]
[<InlineData(0, 20, 0, 3, 1)>]       // ramp-up
[<InlineData(0, 100, 50, 51, 1)>]    // ramp-up
[<InlineData(100, 200, 130, 31, 1)>] // ramp-up
[<InlineData(0, 100, 90, 95, 5)>]    // ramp-up
let ``schedule should correctly handle ramp-up for RampingConstant``
    (prevCopiesCount: int, copiesCount: int, currentConstActorCount: int, timeProgress: int, result: int) =

    let simulation = {
        Value = RampingConstant(copiesCount, TimeSpan.Zero)
        StartTime = TimeSpan.Zero
        EndTime = TimeSpan.Zero
        Duration = TimeSpan.Zero
        PrevActorCount = prevCopiesCount
    }

    let struct (command, copiesCount) =
        ScenarioScheduler.Test.schedule getRandomValue simulation timeProgress currentConstActorCount

    match command with
    | SchedulerCommand.AddConstantActors -> test <@ copiesCount = result @>
    | _                                  -> failwith "invalid command"

[<Theory>]
[<InlineData(100, 0, 50, 51, 1)>]    // ramp-down
[<InlineData(200, 100, 170, 31, 1)>] // ramp-down
[<InlineData(100, 0, 10, 95, 5)>]    // ramp-down
let ``schedule should correctly handle ramp-down for RampingConstant``
    (prevCopiesCount: int, copiesCount: int, currentConstActorCount: int, timeProgress: int, result: int) =

    let simulation = {
        Value = RampingConstant(copiesCount, TimeSpan.Zero)
        StartTime = TimeSpan.Zero
        EndTime = TimeSpan.Zero
        Duration = TimeSpan.Zero
        PrevActorCount = prevCopiesCount
    }

    let struct (command, copiesCount) =
        ScenarioScheduler.Test.schedule getRandomValue simulation timeProgress currentConstActorCount

    match command with
    | SchedulerCommand.RemoveConstantActors -> test <@ copiesCount = result @>
    | _                                     -> failwith "invalid command"

[<Theory>]
[<InlineData(0, 20, 0, 1, 20)>]   // keep-constant add actors
[<InlineData(0, 20, 0, 8, 20)>]   // keep-constant add actors
[<InlineData(20, 20, 20, 1, 0)>]  // do nothing
[<InlineData(20, 10, 20, 1, 10)>] // keep-constant remove actors
let ``schedule should correctly handle KeepConstant``
    (prevCopiesCount: int, copiesCount: int, currentConstActorCount: int, timeProgress: int, result: int) =

    let simulation = {
        Value = KeepConstant(copiesCount, TimeSpan.Zero)
        StartTime = TimeSpan.Zero
        EndTime = TimeSpan.Zero
        Duration = TimeSpan.Zero
        PrevActorCount = prevCopiesCount
    }

    let struct (command, copiesCount) =
        ScenarioScheduler.Test.schedule getRandomValue simulation timeProgress currentConstActorCount

    match command with
    | SchedulerCommand.AddConstantActors    -> test <@ copiesCount = result @>
    | SchedulerCommand.DoNothing            -> test <@ copiesCount = 0 @>
    | SchedulerCommand.RemoveConstantActors -> test <@ copiesCount = result @>
    | _                                     -> failwith "invalid command"

[<Theory>]
[<InlineData(20, 0, 1, 0)>]     // do nothing
[<InlineData(0, 1, 1, 1)>]     // do nothing
[<InlineData(20, 10, 50, 10)>] // remove constant actors
let ``schedule should correctly handle Pause``
    (prevCopiesCount: int, currentConstActorCount: int, timeProgress: int, result: int) =

    let simulation = {
        Value = Pause(seconds 1)
        StartTime = TimeSpan.Zero
        EndTime = TimeSpan.Zero
        Duration = TimeSpan.Zero
        PrevActorCount = prevCopiesCount
    }

    let struct (command, copiesCount) =
        ScenarioScheduler.Test.schedule getRandomValue simulation timeProgress currentConstActorCount

    match command with
    | SchedulerCommand.RemoveConstantActors -> test <@ copiesCount = result @>
    | SchedulerCommand.DoNothing            -> test <@ copiesCount = 0 @>
    | _                                     -> failwith "invalid command"

[<Theory>]
[<InlineData(20, 0, 0)>]    // do nothing
[<InlineData(0, 1, 1)>]     // remove constant actors
[<InlineData(20, 10, 10)>] // remove constant actors
let ``scheduleCleanPrevSimulation should remove Constant actors in case of switch from Closed to Open model``
    (prevCopiesCount: int, currentConstActorCount: int, result: int) =

    let simulation = {
        Value = Pause(seconds 1)
        StartTime = TimeSpan.Zero
        EndTime = TimeSpan.Zero
        Duration = TimeSpan.Zero
        PrevActorCount = prevCopiesCount
    }

    let struct (command, copiesCount) =
        ScenarioScheduler.Test.scheduleCleanPrevSimulation simulation currentConstActorCount

    match command with
    | SchedulerCommand.RemoveConstantActors -> test <@ copiesCount = result @>
    | SchedulerCommand.DoNothing            -> test <@ copiesCount = 0 @>
    | _                                     -> failwith "invalid command"

[<Fact>]
[<Trait("CI", "disable")>]
let ``should run Inject correctly`` () =
    Scenario.create("hello_world_scenario", fun ctx -> task {
        return Response.ok()
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [Inject(rate = 2, interval = seconds 1, during = seconds 20)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.map(fun nodeStats ->
        let reqCount = nodeStats.ScenarioStats[0].Ok.Request.Count
        let rps = nodeStats.ScenarioStats[0].Ok.Request.RPS
        test <@ reqCount = 40 @>
        test <@ nodeStats.AllRequestCount = 40 @>
        test <@ rps = 2 @>
    )
    |> Result.mapError failwith
    |> ignore
