module Tests.ScenarioSchedulerTests

open System

open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain.Concurrency.Scheduler
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler

[<Property>]
let ``ScenarioScheduler.calcScheduleTickProgress should always return a value bigger or equal 1``
    (scheduleTickInterval: float) =

    // condition
    scheduleTickInterval > 0.0 ==> lazy

    let result = ScenarioScheduler.calcScheduleTickProgress(scheduleTickInterval)
    test <@ result >= 1.0 @>

[<Property>]
let ``ScenarioScheduler.schedule should handle KeepConcurrentScenarios correctly``
    (scheduleTickIntervalMs: uint32, constWorkingActorCount: uint32, oneTimeActorPerSecCount: uint32,
     copiesCount: uint32, during: TimeSpan) =

    // condition
    copiesCount > 0u ==> lazy

    let simulation = KeepConcurrentScenarios(int copiesCount, during)
    let result = ScenarioScheduler.schedule(float scheduleTickIntervalMs, int constWorkingActorCount,
                                            int oneTimeActorPerSecCount, simulation)

    result
    |> List.iter(function
        | AddConstantActors scheduled    -> test <@ scheduled <> 0 @>
                                            test <@ scheduled + int constWorkingActorCount = int copiesCount @>

        | RemoveConstantActors scheduled -> test <@ scheduled <> 0 @>
                                            test <@ int constWorkingActorCount - scheduled = int copiesCount @>

        | DoNothing                      -> test <@ copiesCount = constWorkingActorCount @>
        | _                              -> failwith "wrong simulation"
    )

    test <@ result.Length = 1 @>

[<Property>]
let ``ScenarioScheduler.schedule should handle RampConcurrentScenarios correctly``
    (scheduleTickIntervalMs: uint32, constWorkingActorCount: uint32, oneTimeActorPerSecCount: uint32,
     copiesCount: uint32, during: TimeSpan) =

    // condition
    (scheduleTickIntervalMs > 0u && float scheduleTickIntervalMs < during.TotalMilliseconds
     && copiesCount > 0u && during > TimeSpan.Zero) ==> lazy

    let simulation = RampConcurrentScenarios(int copiesCount, during)
    let result = ScenarioScheduler.schedule(float scheduleTickIntervalMs, int constWorkingActorCount,
                                            int oneTimeActorPerSecCount, simulation)
    result
    |> List.iter(function
        | AddConstantActors scheduled    -> test <@ scheduled <> 0 @>
                                            test <@ uint32 scheduled + constWorkingActorCount <= copiesCount @>

        | RemoveConstantActors scheduled -> test <@ scheduled <> 0 @>
                                            test <@ int constWorkingActorCount - scheduled >= int copiesCount @>

        | DoNothing                      -> test <@ copiesCount = constWorkingActorCount @>
        | _                              -> failwith "wrong simulation"
    )

    test <@ result.Length = 1 @>
