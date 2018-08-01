namespace rec NBomber.Assertions

open System

type AssertionFunc = Func<AssertionInfo, bool>

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

type AssertionScope = 
        | Step     of string * string * AssertionFunc
        | TestFlow of string * AssertionFunc
        | Scenario of AssertionFunc

type Position = int

type ScopeResult = 
    | Success
    | Failure of AssertionScope * Position
    | NotFound of AssertionScope * Position

type AssertionResult = 
    | AssertionOk of int
    | AsserionFailure of (string * string) list

module Assertions =

    let apply (scenarioName: string, flows: AssertionInfo[], assertions: AssertionScope[]) =         
       assertions 
       |> Array.mapi (fun i assertion -> executeAssertion(scenarioName, flows, i+1, assertion))
       |> checkIfAllAreOk |> function
       | true -> AssertionOk(assertions |> Seq.length)
       | _ -> [ ("", "") ] |> AsserionFailure

    let executeAssertion(scenarioName:string, flows: AssertionInfo[], i: int, scope: AssertionScope) =
       match scope with
           | Scenario (assertion) ->
                flows
                |> Seq.groupBy (fun f -> f.FlowName)
                |> Seq.map (fun (_, steps) -> steps |> executeForSteps <| assertion)
                |> Seq.exists (fun (result, _) -> not result)
                |> function
                | true -> Success
                | _ -> Failure(scope, i)

           | TestFlow (flowName, assertion) ->
                flows
                |> Seq.filter (fun f -> f.FlowName = flowName)
                |> executeForSteps <| (assertion)
                |> function
                | true, _ -> Success
                | _, 0 -> Failure(scope, i)
                | _ -> NotFound(scope, i)   

           | Step (flowName, stepName, assertion) -> 
               flows
               |> Seq.filter (fun x -> x.FlowName = flowName && x.StepName = stepName)
               |> executeForSteps <| assertion
               |> function
               | true, _ -> Success
               | _, 0 -> Failure(scope, i)
               | _ -> NotFound(scope, i)    
                                                            
    let executeForSteps steps assertion =
        steps |> Seq.map(fun s -> s |> assertion.Invoke) |> Seq.exists(not), steps |> Seq.length

    let checkIfAllAreOk asserted = asserted |> Seq.forall(fun a -> a |> function | Success -> true | _ -> false)