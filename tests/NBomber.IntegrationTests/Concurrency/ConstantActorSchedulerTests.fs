module Tests.Concurrency.ConstantActorScheduler

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open Swensen.Unquote
open FsToolkit.ErrorHandling
open Xunit

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Domain
open NBomber.Domain.Stats
open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ConstantActorScheduler
open NBomber.Extensions.InternalExtensions

let internal baseScenario =
    Scenario.create "test_scn" [Step.createPause(milliseconds 100)]
    |> Scenario.withLoadSimulations [KeepConstant(100, seconds 30)]
    |> List.singleton
    |> Scenario.createScenarios
    |> Result.getOk
    |> List.head

let internal logger = LoggerConfiguration().CreateLogger()

let internal baseDep = {
    Logger = logger
    CancellationToken = CancellationToken.None
    ScenarioGlobalTimer = Stopwatch()
    Scenario = baseScenario
    ScenarioStatsActor = ScenarioStatsActor.create logger baseScenario Constants.DefaultSendStatsInterval
    ExecStopCommand = fun _ -> ()
}

[<Fact>]
let ``AddActors should start actors if there is no actors`` () =
    use scheduler = new ConstantActorScheduler(baseDep, exec)

    let initCount = scheduler.ScheduledActorCount
    scheduler.AddActors(20)
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ initCount = 0 @>
    test <@ scheduler.ScheduledActorCount = 20 @>
    test <@ workingActors.Length = 20 @>

[<Fact>]
let ``AddActors should start actors to run forever until the finish of scenario duration`` () =
    use scheduler = new ConstantActorScheduler(baseDep, exec)

    scheduler.AddActors(10)
    Task.Delay(seconds 5).Wait()
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ scheduler.ScheduledActorCount = 10 @>
    test <@ workingActors.Length = 10 @>
    test <@ scheduler.AvailableActors.Length = 10 @>

[<Fact>]
let ``RemoveActors should stop some actors and keep them in actor pool`` () =
    use scheduler = new ConstantActorScheduler(baseDep, exec)

    scheduler.AddActors(10)
    scheduler.RemoveActors(5)
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ scheduler.ScheduledActorCount = 5 @>
    test <@ workingActors.Length = 5 @>
    test <@ scheduler.AvailableActors.Length = 10 @>

[<Fact>]
let ``Stop should stop all working actors`` () =
    use scheduler = new ConstantActorScheduler(baseDep, exec)

    scheduler.AddActors(10)
    scheduler.Stop()
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ scheduler.ScheduledActorCount = 10 @>
    test <@ workingActors.Length = 0 @>
    test <@ scheduler.AvailableActors.Length = 10 @>
