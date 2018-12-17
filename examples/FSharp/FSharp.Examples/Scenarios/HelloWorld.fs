module HelloWorldScenario

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =    

    let pool = ConnectionPool.none

    let step1 = Step.createPull("simple step", pool, fun context -> task {        
        // you can do any logic here: go to http, websocket etc
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok() 
    })
        
    Scenario.create("Hello World from NBomber!", [step1])