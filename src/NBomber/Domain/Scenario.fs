module internal NBomber.Domain.Scenario

open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Errors
open NBomber.Domain.DomainTypes

let create (config: Contracts.Scenario) =
    { ScenarioName = config.ScenarioName
      InitStep = config.TestInit |> Option.map(fun x -> Step.getRequest(x :?> Step))
          
      TestFlows = config.TestFlows 
                  |> Array.mapi(fun i config -> TestFlow.create(i, config))
                  |> Array.map(TestFlow.initFlow)

      Duration = config.Duration }
          
let runInit (scenario: Scenario) =
    match scenario.InitStep with
    | Some step -> 
        try                 
            let req = { CorrelationId = Constants.InitId; Payload = null }
            step.Execute(req).Wait()                
            Ok(scenario)
        with ex -> ex.Message |> InitStepError |> Error
    | None      -> Ok(scenario)

let warmUp (scenario: Scenario) =                
    let errors = 
        scenario.TestFlows 
        |> Array.map(fun flow -> TestFlow.warmUpFlow(flow).Result)
        |> Array.filter(Result.isError)
        |> Array.map(Result.getError)
        
    if errors.Length > 0 then
        errors |> WarmUpError |> Error
    else
        Ok(scenario)