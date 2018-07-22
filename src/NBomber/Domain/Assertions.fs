namespace rec NBomber

open NBomber.Reporting

type Assert = AssertionInfo -> bool
type Index = int
type Scope = All | Flow | Step
type Target = string

type Assertion = delegate of AssertionInfo -> bool

type AssertionInfo = {    
    OkCount: int
    FailCount: int
    ExceptionCount: int
} with
     static member Create (step:StepInfo) =
         { OkCount = step.OkCount; FailCount = step.FailCount; ExceptionCount = step.ExceptionCount }

type Asserted = 
     | AssertOk
     | AssertAllFailed of Index
     | AssertFailed of Index * Scope * Target
     | NotFound of Index * Scope * Target    
 
     static member internal create result (index, scope, target) = result |> function
         | true -> AssertOk
         | _ -> target |> function | "" -> AssertAllFailed(index) | _ -> AssertFailed(index, scope, target)
 
     static member internal getMessage asserted = asserted |> function
         | AssertFailed(index, scope, target) -> Some(sprintf "Assertion #%i for %O '%s' FAILED" index scope target)
         | AssertAllFailed(index) -> Some(sprintf "Assertion #%i for All FAILED" index)
         | NotFound(index, scope, target) -> Some(sprintf "%O '%s' for Assertion #%i NOT FOUND" scope target index)
         | _ -> None
 
     static member internal isOk(step) = match step with | AssertOk _ -> true | _ -> false
     static member internal isFailed(step) = match step with | AssertFailed _    -> true | _ -> false

type AssertScope = 
    | Step     of string * string * Assertion
    | TestFlow of string * Assertion
    | Scenario of Assertion

module internal Assertions =

   let apply (flows: FlowInfo[], assertions: AssertScope[]) =         
       assertions 
       |> Array.mapi (fun i assertion -> executeAssertion(flows, i+1, assertion)) |> checkIfAllOk

   let executeAssertion(flows:FlowInfo[], i:Index, assertion:AssertScope) =
       match assertion with
           | Scenario (assertion) -> assertForAll(flows, i, assertion)

           | TestFlow (flowName, assertion) -> 
               flows |> Seq.tryFind (fun f -> f.FlowName = flowName) |> assertForFlow <| (i, flowName, assertion)

           | Step (flowName, stepName, assertion) -> 
               flows |> Seq.tryFind (fun f -> f.FlowName = flowName) |> (function
                   | Some f -> f.Steps
                               |> Seq.tryFind (fun s -> s.StepName = stepName)
                               |> assertForStep <| (i, stepName, assertion)
                   | None -> NotFound(i, Scope.Flow, flowName))

   let checkIfAllOk (asserted:seq<Asserted>) =
       asserted |> Seq.forall(fun a -> Asserted.isOk(a)) |> function 
                   | true -> asserted |> Seq.length |> 
                               function 
                               | 0 -> [||] 
                               | length -> [| sprintf "Assertions: %i - OK" length |]
                   | _ -> asserted |> Seq.choose(fun result -> Asserted.getMessage(result)) |> Seq.toArray

   let assertForAll (flows:FlowInfo[], i:Index, assertion:Assertion) =
        flows |> Seq.map (fun f -> Some(f) |> assertForFlow <| (i, f.FlowName, assertion))
              |> Seq.exists (fun a -> Asserted.isFailed(a)) |> Asserted.create <| (i, Scope.All, "")  

   let assertForFlow (flow:FlowInfo option) (i:Index, flowName:string, assertion:Assertion) =
       match flow with
       | Some f -> f.Steps 
                   |> Seq.map (fun s -> Some(s) |> assertForStep <| (i, s.StepName, assertion))
                   |> Seq.exists(fun a -> Asserted.isFailed(a)) 
                   |> Asserted.create <| (i, Scope.Flow, f.FlowName)        
       | None -> NotFound(i, Scope.Flow, flowName)      

   let assertForStep (step:StepInfo option) (i:Index, stepName:string, assertion:Assertion) =
       match step with
       | Some s -> s   |> AssertionInfo.Create
                       |> assertion.Invoke
                       |> Asserted.create <| (i, Scope.Step, s.StepName)                    
       | None -> NotFound(i, Scope.Step, stepName)