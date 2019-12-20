module CustomReportingScenario

open System
open System.Threading.Tasks
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

// it's a very basic CustomReportingSink example to give you a playground for writing your own custom sink
// for production purposes use https://github.com/PragmaticFlow/NBomber.Sinks.InfluxDB

type CustomReportingSink() =
    
    interface IReportingSink with
        member x.StartTest(testInfo: TestInfo) =
            Task.CompletedTask
            
        member x.SaveStatistics(testInfo: TestInfo, stats: Statistics[]) =
            Task.CompletedTask
            
        member x.SaveReports(testInfo: TestInfo, reportFiles: ReportFile[]) =
            Task.CompletedTask
            
        member x.FinishTest(testInfo: TestInfo) =
            Task.CompletedTask
        
let run () =
    
    let httpClient = new HttpClient()

    let step = Step.create("GET html", fun context -> task {
        use! response = httpClient.GetAsync("https://nbomber.com",
                                            context.CancellationToken)
        
        match response.IsSuccessStatusCode with
        | true  -> let size = int response.Content.Headers.ContentLength.Value
                   return Response.Ok(sizeBytes = size)
        | false -> return Response.Fail() 
    })
    
    let scenario = Scenario.create "test_gitter" [step]
                   |> Scenario.withConcurrentCopies 10
                   |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 30.0)
                   |> Scenario.withDuration(TimeSpan.FromSeconds 60.0)

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportingSinks([CustomReportingSink()], TimeSpan.FromSeconds 10.0)    
    |> NBomberRunner.runInConsole