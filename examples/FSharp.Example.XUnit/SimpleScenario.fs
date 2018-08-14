module rec SimpleScenario

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let private step1 = Step.createRequest("Step", fun req -> task {
    do! Task.Delay(TimeSpan.FromSeconds(2.0))
    return Response.Ok()
})

let buildScenario () =
    Scenario.create("Scenario")
    |> Scenario.addTestFlow({ FlowName = "Flow"; Steps = [step1]; ConcurrentCopies = 100 })   
    |> Scenario.withDuration(TimeSpan.FromSeconds(10.0))
