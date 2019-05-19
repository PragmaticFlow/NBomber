﻿module HttpScenario

open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =    

    // it's a very basic HTTP example, don't use it for production testing
    // for production purposes use NBomber.Http which use performance optimizations
    // you can find more here: https://github.com/PragmaticFlow/NBomber.Http

    let httpClient = new HttpClient()

    let step = Step.create("GET html", fun context -> task {        
        let! response = httpClient.GetAsync("https://gitter.im",
                                            context.CancellationToken)
        
        match response.IsSuccessStatusCode with
        | true  -> let size = int response.Content.Headers.ContentLength.Value
                   return Response.Ok(sizeBytes = size)
        | false -> return Response.Fail() 
    })
        
    Scenario.create "test_gitter" [step]