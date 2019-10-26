module RealtimeStatisticsScenario

open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open System
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Sinks.InfluxDB

let run () =    

    // it's a very basic HTTP example, don't use it for production testing
    // for production purposes use NBomber.Http which use performance optimizations
    // you can find more here: https://github.com/PragmaticFlow/NBomber.Http

    let influxDb = InfluxDBSink(url = "http://localhost:8086", dbName = "default")
    
    let httpClient = new HttpClient()

    let step = Step.create("GET html", fun context -> task {
        let! response = httpClient.GetAsync("https://gitter.im",
                                            context.CancellationToken)
        
        match response.IsSuccessStatusCode with
        | true  -> let size = int response.Content.Headers.ContentLength.Value
                   return Response.Ok(sizeBytes = size)
        | false -> return Response.Fail() 
    })
    
    let scenario = Scenario.create "test_gitter" [step]
                   |> Scenario.withConcurrentCopies 100
                   |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 30.0)
                   |> Scenario.withDuration(TimeSpan.FromSeconds 60.0)

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.saveStatisticsTo influxDb
    |> NBomberRunner.runInConsole