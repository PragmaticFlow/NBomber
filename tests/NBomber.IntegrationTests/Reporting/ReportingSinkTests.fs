module Tests.ReportingSinkTests

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain
open NBomber.FSharp

//todo: test that multiply sink will be invoked correctly
//todo: test that stop timer stops sending metrics in case when stopping is still executing
//todo: test cluster stats

[<Fact>]
let ``IReportingSink.SaveStatistics should be invoked many times during test execution to send realtime stats`` () =
    
    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario =
        Scenario.create "realtime stats scenario" [okStep]
        |> Scenario.withDuration(TimeSpan.FromSeconds 10.0)

    let mutable statsInvokedCounter = 0

    let reportingSink = { new IReportingSink with
                            member x.StartTest(_) = Task.CompletedTask
                            member x.SaveStatistics(_, stats) =
                                // 1 invoke per 5 sec
                                statsInvokedCounter <- statsInvokedCounter + 1
                                Task.CompletedTask
                            member x.SaveReports(_, _) = Task.CompletedTask
                            member x.FinishTest(_) = Task.CompletedTask }                        
    
    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.runTest
    
    test <@ statsInvokedCounter >= 2 @> // 1 invoke as realtime and 1 invoke at the end
    
[<Fact>]
let ``NBomberRunner.saveStatisticsTo should be invoked with correct operation type = WarmUp or Bombing`` () =
    
    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario =
        Scenario.create "realtime stats scenario" [okStep]
        |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 10.0)
        |> Scenario.withDuration(TimeSpan.FromSeconds 10.0)

    let mutable warmUpCounter = 0
    let mutable bombingCounter = 0
    let mutable completeCounter = 0

    let reportingSink = { new IReportingSink with
                            member x.StartTest(_) = Task.CompletedTask
                            
                            member x.SaveStatistics(_, stats) =
                                match stats.[0].NodeInfo.CurrentOperation with
                                | NodeOperationType.WarmUp   -> warmUpCounter <- warmUpCounter + 1
                                | NodeOperationType.Bombing  -> bombingCounter <- bombingCounter + 1
                                | NodeOperationType.Complete -> completeCounter <- completeCounter + 1
                                | _        -> failwith "operation type is invalid for SaveStatistics"
                                Task.CompletedTask
                                
                            member x.SaveReports(_, _) = Task.CompletedTask
                            member x.FinishTest(_) = Task.CompletedTask }    
    
    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.runTest
    
    test <@ warmUpCounter > 0 && bombingCounter > 0 && completeCounter = 1 @>
    test <@ warmUpCounter = bombingCounter @>