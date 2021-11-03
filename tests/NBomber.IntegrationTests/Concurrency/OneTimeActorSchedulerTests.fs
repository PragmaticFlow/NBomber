module Tests.Concurrency.OneTimeActorScheduler

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open Swensen.Unquote
open Xunit
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Domain
open NBomber.Domain.Stats
open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.OneTimeActorScheduler
open NBomber.Extensions.InternalExtensions

let internal baseScenario =
    Scenario.create "test_scn" [Step.createPause(milliseconds 100)]
    |> Scenario.withLoadSimulations [KeepConstant(100, seconds 2)]
    |> List.singleton
    |> Scenario.createScenarios
    |> Result.getOk
    |> List.head

let internal logger = LoggerConfiguration().CreateLogger()

let internal baseDep = {
    Logger = logger
    CancellationToken = CancellationToken.None
    GlobalTimer = Stopwatch()
    Scenario = baseScenario
    ScenarioStatsActor = ScenarioStatsActor.create logger baseScenario Constants.DefaultSendStatsInterval
    ExecStopCommand = fun _ -> ()
}

[<Fact>]
let ``InjectActors should start actors if there is no actors`` () =
    use scheduler = new OneTimeActorScheduler(baseDep, exec)

    let initCount = scheduler.ScheduledActorCount
    scheduler.InjectActors(20)
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ initCount = 0 @>
    test <@ scheduler.ScheduledActorCount = 20 @>
    test <@ workingActors.Length = 20 @>

[<Fact>]
let ``InjectActors should execute actors once until next turn`` () =
    use scheduler = new OneTimeActorScheduler(baseDep, exec)

    scheduler.InjectActors(20)
    Task.Delay(seconds 5).Wait()
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ scheduler.ScheduledActorCount = 20 @>
    test <@ workingActors.Length = 0 @>

[<Fact>]
let ``Stop should stop all working actors`` () =
    use scheduler = new OneTimeActorScheduler(baseDep, exec)

    scheduler.InjectActors(20)
    scheduler.Stop()
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ scheduler.ScheduledActorCount = 20 @>
    test <@ workingActors.Length = 0 @>
