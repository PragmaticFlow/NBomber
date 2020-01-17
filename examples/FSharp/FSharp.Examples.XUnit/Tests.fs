module Tests

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``XUnit test`` () =

    let step1 = Step.create("simple step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok(sizeBytes = 1024)
    })
    
    let scenario =
        Scenario.create "xunit hello world" [step1]
        |> Scenario.withConcurrentCopies 1
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 2.0)        
    
    let result = NBomberRunner.registerScenarios [scenario]
                 |> NBomberRunner.runTest
    
    match result with
    | Ok allStats ->        
        let stepStats = allStats |> Array.find(fun x -> x.StepName = "simple step")
        test <@ stepStats.OkCount > 2 @>
        test <@ stepStats.RPS > 8 @>
        test <@ stepStats.Percent75 >= 100 @>
        test <@ stepStats.DataMinKb = 1.0 @>
        test <@ stepStats.AllDataMB >= 0.01 @>
    
    | Error msg -> failwith msg
        
        
        
                 
                     