namespace rec NBomber

type AssertionInfo = {    
    OkCount: int
    FailCount: int
    ExceptionCount: int
}

type Assert = AssertionInfo -> bool

type AssertScope = 
    | Step     of FlowName * StepName * Assert
    | TestFlow of FlowName * Assert
    | Scenario of Assert


//module internal Assertions =

//    let apply (flows: FlowInfo[], assertions: Assert[]) =         
//        assertions 
//        |> Array.mapi (fun i assertion -> executeAssertion(flows, i+1, assertion)) |> checkIfAllOk

//    let executeAssertion(flows:FlowInfo[], i:Index, assertion:Assert) =
//        match assertion with
//            | All assertion -> assertForAll(flows, i, assertion)

//            | Flow (flowName, assertion) -> 
//                flows |> Seq.tryFind (fun f -> f.FlowName = flowName) |> assertForFlow <| (i, flowName, assertion)

//            | Step (flowName, stepName, assertion) -> 
//                flows |> Seq.tryFind (fun f -> f.FlowName = flowName) |> (function
//                    | Some f -> f.Steps
//                                |> Seq.tryFind (fun s -> s.StepName = stepName)
//                                |> assertForStep <| (i, stepName, assertion)
//                    | None -> NotFound(i, Scope.Flow, flowName))

//    let checkIfAllOk (asserted:seq<Asserted>) =
//        asserted |> Seq.forall(fun a -> Asserted.isOk(a)) |> function 
//                    | true -> asserted |> Seq.length |> 
//                                function 
//                                | 0 -> [||] 
//                                | length -> [| sprintf "Assertions: %i - OK" length |]
//                    | _ -> asserted |> Seq.choose(fun result -> Asserted.getMessage(result)) |> Seq.toArray

//    let assertForAll (flows:FlowInfo[], i:Index, assertion:Assertion) =
//         flows |> Seq.map (fun f -> Some(f) |> assertForFlow <| (i, f.FlowName, assertion))
//               |> Seq.exists (fun a -> Asserted.isFailed(a)) |> Asserted.create <| (i, Scope.All, "")  

//    let assertForFlow (flow:FlowInfo option) (i:Index, flowName:string, assertion:Assertion) =
//        match flow with
//        | Some f -> f.Steps 
//                    |> Seq.map (fun s -> Some(s) |> assertForStep <| (i, s.StepName, assertion))
//                    |> Seq.exists(fun a -> Asserted.isFailed(a)) 
//                    |> Asserted.create <| (i, Scope.Flow, f.FlowName)        
//        | None -> NotFound(i, Scope.Flow, flowName)      

//    let assertForStep (step:StepInfo option) (i:Index, stepName:string, assertion:Assertion) =
//        match step with
//        | Some s -> s   |> AssertionStats.Create
//                        |> assertion.Invoke
//                        |> Asserted.create <| (i, Scope.Step, s.StepName)                    
//        | None -> NotFound(i, Scope.Step, stepName)