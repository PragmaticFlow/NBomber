module HelloWorldScenario

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let run () =

    let step = Step.create("simple step", fun context -> task {
        // you can do any logic here: go to http, websocket etc

        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario = Scenario.create "Hello World!" [step]
                   |> Scenario.withLoadSimulations [
                       KeepConstant(copies 10, seconds 10)
                   ]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.runInConsole
