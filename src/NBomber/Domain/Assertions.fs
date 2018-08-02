namespace rec NBomber.Assertions

open System

type AssertionInfo = {
    StepName: string
    FlowName: string    
    OkCount: int
    FailCount: int
    ExceptionCount: int
    ThrownException: exn option
} with
     static member Create (stepName, flowName, okCount, failCount, exceptionCount, exn) =
         { StepName = stepName; FlowName = flowName; OkCount = okCount; FailCount = failCount;
            ExceptionCount = exceptionCount; ThrownException = exn}

type AssertionFunc = Func<AssertionInfo, bool>

type AssertionScope = 
    | Step     of string * string * AssertionFunc
    | TestFlow of string * AssertionFunc
    | Scenario of AssertionFunc

type AssertionResult = | Success | Failure of string

module Assertions =

    let apply (scenarioName: string, flows: AssertionInfo[], assertions: AssertionScope[]) =         
       assertions 
       |> Array.mapi (fun i assertion -> executeAssertion(scenarioName, flows, i+1, assertion))
       |> printAssertionResults

    let executeAssertion(scenarioName:string, flows: AssertionInfo[], i: int, assertion: AssertionScope) =
       match assertion with
           | Scenario (func) ->
                let result = flows
                            |> Seq.groupBy (fun f -> f.FlowName)
                            |> Seq.map (fun (_, steps) -> steps |> executeForSteps <| func)
                            |> Seq.exists (fun (result) -> result |> function | Some(x) -> x | None -> true)
                if result then Success
                else Failure(sprintf "Assertion #%i FAILED for Scenario %s" i scenarioName)

           | TestFlow (flowName, func) ->
                flows
                |> Seq.where (fun f -> f.FlowName = flowName)
                |> executeForSteps <| func
                |> function
                | Some(assertionResult) -> if assertionResult then Success
                                           else Failure(sprintf "Assertion #%i FAILED for Test Flow %s" i scenarioName)                                  
                | None -> Failure(sprintf "Assertion #%i NOT FOUND for Test flow %s" i flowName) 

           | Step (flowName, stepName, func) -> 
               flows
               |> Seq.filter (fun x -> x.FlowName = flowName && x.StepName = stepName)
               |> executeForSteps <| func
               |> function
               | Some(assertionResult) -> if assertionResult then Success
                                          else Failure(sprintf "Assertion #%i FAILED for Step %s" i scenarioName)
               | None -> Failure(sprintf "Assertion #%i NOT FOUND for Step %s" i stepName)  
                                                            
    let executeForSteps steps assertion : bool option =
        steps
        |> Seq.map(fun s -> s |> assertion.Invoke)
        |> Seq.toList
        |> function | [] -> None | stepResults -> stepResults |> Seq.exists(id) |> Some

    let private printAssertionResults (results: AssertionResult[]) =
      let allAreOk = results |> Seq.forall(fun a -> a |> function | Success -> true | _ -> false)
      if allAreOk then results |> Seq.length |> function | 0 -> Seq.empty | length -> seq [sprintf "Assertions: %i - OK" length]
      else results |> Seq.choose(fun x -> x |> function | Failure(msg) -> Some(msg) | _ -> None)