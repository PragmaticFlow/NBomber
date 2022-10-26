module Tests.Concurrency.OneTimeActorScheduler

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
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.Domain.ScenarioContext
open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.Scheduler.OneTimeActorScheduler
open NBomber.Extensions.Internal

let internal baseScenario =
    Scenario.create("test_scn", fun ctx -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })
    |> Scenario.withLoadSimulations [KeepConstant(100, seconds 30)]
    |> List.singleton
    |> Scenario.createScenarios
    |> Result.getOk
    |> List.head

let internal logger = LoggerConfiguration().CreateLogger()

let internal baseScnDep = {
    Logger = logger
    Scenario = baseScenario
    ScenarioCancellationToken = new CancellationTokenSource()
    ScenarioTimer = Stopwatch()
    ScenarioOperation = ScenarioOperation.Bombing
    ScenarioStatsActor = ScenarioStatsActor(logger, baseScenario, Constants.DefaultReportingInterval)
    ExecStopCommand = fun _ -> ()
    MaxFailCount = Constants.DefaultMaxFailCount
}

[<Fact>]
let ``InjectActors should start actors if there is no actors`` () =
    use scheduler = new OneTimeActorScheduler(baseScnDep, exec)

    let initCount = scheduler.ScheduledActorCount
    scheduler.InjectActors(5)
    Task.Delay(milliseconds 10).Wait()
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ initCount = 0 @>
    test <@ scheduler.ScheduledActorCount = 5 @>
    test <@ workingActors.Length = 5 @>

[<Fact>]
let ``InjectActors should execute actors once until next turn`` () =
    use scheduler = new OneTimeActorScheduler(baseScnDep, exec)

    scheduler.InjectActors(5)
    Task.Delay(seconds 2).Wait()
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ scheduler.ScheduledActorCount = 5 @>
    test <@ workingActors.Length = 0 @>

[<Fact>]
let ``Stop should stop all working actors`` () =
    use scheduler = new OneTimeActorScheduler(baseScnDep, exec)

    scheduler.InjectActors(5)
    Task.Delay(milliseconds 10).Wait()
    scheduler.Stop()
    Task.Delay(seconds 2).Wait()
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ scheduler.ScheduledActorCount = 5 @>
    test <@ workingActors.Length = 0 @>
