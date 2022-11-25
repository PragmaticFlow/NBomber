module Tests.Concurrency.ConstantActorScheduler

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
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.Domain.ScenarioContext
open NBomber.Domain.Concurrency
open NBomber.Domain.Scheduler.ConstantActorScheduler
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
}

[<Fact>]
let ``AddActors should start actors if there is no actors`` () =
    use scheduler = new ConstantActorScheduler(baseScnDep, exec)

    let initCount = scheduler.ScheduledActorCount
    scheduler.AddActors(5)
    Task.Delay(milliseconds 10).Wait()
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ initCount = 0 @>
    test <@ scheduler.ScheduledActorCount = 5 @>
    test <@ workingActors.Length = 5 @>

[<Fact>]
let ``AddActors should start actors to run forever until the finish of scenario duration`` () =
    use scheduler = new ConstantActorScheduler(baseScnDep, exec)

    scheduler.AddActors(5)
    Task.Delay(milliseconds 10).Wait()
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ scheduler.ScheduledActorCount = 5 @>
    test <@ workingActors.Length = 5 @>
    test <@ scheduler.AvailableActors.Length = 5 @>

[<Fact>]
let ``RemoveActors should stop some actors and keep them in actor pool`` () =
    use scheduler = new ConstantActorScheduler(baseScnDep, exec)

    scheduler.AddActors(10)
    Task.Delay(milliseconds 10).Wait()
    scheduler.RemoveActors(5)
    Task.Delay(seconds 2).Wait()
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ scheduler.ScheduledActorCount = 5 @>
    test <@ workingActors.Length = 5 @>
    test <@ scheduler.AvailableActors.Length = 10 @>

[<Fact>]
let ``Stop should stop all working actors`` () =
    use scheduler = new ConstantActorScheduler(baseScnDep, exec)

    scheduler.AddActors(5)
    Task.Delay(milliseconds 10).Wait()
    scheduler.Stop()
    Task.Delay(seconds 2).Wait()
    let workingActors = ScenarioActorPool.getWorkingActors scheduler.AvailableActors

    test <@ scheduler.ScheduledActorCount = 5 @>
    test <@ workingActors.Length = 0 @>
    test <@ scheduler.AvailableActors.Length = 5 @>
