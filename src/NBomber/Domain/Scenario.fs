module internal NBomber.Domain.Scenario

open System.Diagnostics

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Errors
open NBomber.Domain.DomainTypes

let create (config: Contracts.Scenario) =
    { ScenarioName = config.ScenarioName
      TestInit = config.TestInit |> Option.map(fun x -> Step.getRequest(x :?> Step))
      Steps = config.Steps |> Array.map(fun x -> x :?> Step) |> Seq.toArray
      Assertions = config.Assertions |> Array.map(fun x -> x :?> Assertion) 
      ConcurrentCopies = config.ConcurrentCopies      
      Duration = config.Duration }
          
let runInit (scenario: Scenario) =
    match scenario.TestInit with
    | Some step -> 
        try                 
            let req = { CorrelationId = Constants.InitId; Payload = null }
            let response = step.Execute(req).Result
            if response.IsOk then Ok(scenario)
            else Error <| InitStepError("init step failed.")
        with ex -> ex.Message |> InitStepError |> Error
    | None      -> Ok(scenario)

let warmUp (scenario: Scenario) = task {               
    let timer = Stopwatch()
    let steps = scenario.Steps |> Array.filter(fun st -> not(Step.isPause st))        
    let mutable request = { CorrelationId = Constants.WarmUpId; Payload = null }
    let mutable result = Ok(scenario)
    let mutable skipStep = false

    for st in steps do
        if not skipStep then                
            let! (response,_) = Step.execStep(st, request, timer)
            if response.IsOk then
                request <- { request with Payload = response.Payload }
            else
                skipStep <- true
                result <- { ScenarioName = scenario.ScenarioName
                            StepName = Step.getName(st)
                            Error = response.Payload.ToString() }
                          |> WarmUpError |> Error
    return result
}