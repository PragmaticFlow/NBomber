module HttpScenario

open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =    

    // it's a very basic HTTP example, don't use it for production testing
    // for production purposes use NBomber.Http which use performance optimizations
    // you can find more here: https://github.com/PragmaticFlow/NBomber.Http

    let httpClient = new HttpClient()

    let step1 = Step.create("GET html", ConnectionPool.none, fun context -> task {        
        let! response = httpClient.GetAsync("https://gitter.im")        
        return if response.IsSuccessStatusCode 
                  then Response.Ok()
               else Response.Fail() 
    })
        
    Scenario.create "test_gitter" [step1]