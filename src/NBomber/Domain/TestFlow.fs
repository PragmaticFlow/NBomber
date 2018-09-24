module internal NBomber.Domain.TestFlow

open System
open System.Diagnostics

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber
open NBomber.Contracts
open NBomber.Domain.Errors
open NBomber.Domain.DomainTypes
    
let create (flowIndex: int, config: Contracts.TestFlow) =
    
    let createCorrelationId (flowIndex, concurrentCopies) =
        [|0 .. concurrentCopies - 1|] 
        |> Array.map(fun flowCopyNumber -> String.Format("{0}_{1}", flowIndex, flowCopyNumber))
        |> Set.ofArray

    { FlowName = config.FlowName
      Steps = config.Steps |> Seq.map(fun x -> x :?> Step) |> Seq.toArray
      CorrelationIds = createCorrelationId(flowIndex, config.ConcurrentCopies) }        

let initFlow (flow: TestFlow) =

    let initListeners (flow: TestFlow) (listStep: ListenerStep) = 
        flow.CorrelationIds
        |> Set.toArray
        |> Array.map(StepListener)
        |> Array.append([|StepListener(Constants.WarmUpId)|])
        |> listStep.Listeners.Init 

    flow.Steps
    |> Array.filter(Step.isListener)
    |> Array.map(Step.getListener)        
    |> Array.iter(initListeners(flow))
    flow

let warmUpFlow (flow: TestFlow) = task {
    let timer = Stopwatch()        
    let steps = flow.Steps |> Array.filter(fun st -> not(Step.isPause st))        
    let mutable request = { CorrelationId = Constants.WarmUpId; Payload = null }
    let mutable result = Ok()
    let mutable skipStep = false

    for st in steps do
        if not skipStep then                
            let! (response,_) = Step.execStep(st, request, timer)
            if response.IsOk then
                request <- { request with Payload = response.Payload }
            else 
                skipStep <- true                        
                result <- Error({ FlowName = flow.FlowName
                                  StepName = Step.getName(st)
                                  Error = response.Payload.ToString() })
    return result
} 