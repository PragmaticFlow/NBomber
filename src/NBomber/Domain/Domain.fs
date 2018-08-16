module internal rec NBomber.Domain

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Errors

type CorrelationId = string
type Latency = int64
type ExceptionCount = int
type StepName = string
type FlowName = string

type RequestStep = {
    StepName: StepName
    Execute: Request -> Task<Response>
}

type ListenerStep = {
    StepName: StepName    
    Listeners: StepListenerChannel
}  

type Step =
    | Request  of RequestStep
    | Listener of ListenerStep
    | Pause    of TimeSpan 
    interface IStep

type TestFlow = {    
    FlowName: FlowName
    Steps: Step[]    
    CorrelationIds: Set<CorrelationId>
}

type Scenario = {
    ScenarioName: string
    InitStep: RequestStep option
    TestFlows: TestFlow[]    
    Assertions: Assertion[]
    Duration: TimeSpan    
}

type AssertionFunc = AssertionStats -> bool

type Assertion = 
    | Step     of stepName:string * flowName:string * AssertionFunc
    | TestFlow of flowName:string * AssertionFunc
    | Scenario of AssertionFunc
    interface IAssertion

type StepListenerChannel() =

    let mutable listeners = Dictionary<CorrelationId,StepListener>()

    member x.Init(items: StepListener[]) = 
        listeners.Clear()
        items |> Array.iter(fun x -> listeners.Add(x.CorrelationId, x))

    member x.Get(correlationId: string) = listeners.[correlationId]

    interface IStepListenerChannel with
        member x.Notify(correlationId: CorrelationId, response: Response) =
            match listeners.TryGetValue(correlationId) with
            | true, listener -> listener.Notify(response)
            | _              -> ()                

type StepListener(correlationId: string) = 

    let mutable tcs = TaskCompletionSource<Response>()    

    member x.CorrelationId = correlationId

    member x.Notify(response: Response) = 
        if not tcs.Task.IsCompleted then tcs.SetResult(response)

    member x.GetResponse() = 
        tcs <- TaskCompletionSource<Response>()
        tcs.Task


module Step =        

    let isRequest (step)  = match step with | Request _  -> true | _ -> false
    let isListener (step) = match step with | Listener _ -> true | _ -> false
    let isPause (step)    = match step with | Pause _    -> true | _ -> false    
    
    let getName (step) = 
        match step with
        | Request s  -> s.StepName
        | Listener s -> s.StepName
        | Pause t    -> "pause"
        
    let getRequest (step) =
        match step with
        | Request r -> r
        | _         -> failwith "step is not a Request"

    let getListener (step) = 
        match step with 
        | Listener l -> l
        | _          -> failwith "step is not a Listener"

    let execStep (step: Step, req: Request, timer: Stopwatch) = task {        
        timer.Restart()        
        match step with
        | Request r  -> let! resp = r.Execute(req)
                        timer.Stop()
                        let latency = Convert.ToInt64(timer.Elapsed.TotalMilliseconds)
                        return (resp, latency)        
        
        | Listener l -> let listener = l.Listeners.Get(req.CorrelationId)
                        let! resp = listener.GetResponse()
                        timer.Stop()
                        let latency = Convert.ToInt64(timer.Elapsed.TotalMilliseconds)
                        return (resp, latency)
        
        | Pause time -> do! Task.Delay(time)
                        return (Response.Ok(req), int64(0))
    }

    let runSteps (steps: Step[], correlationId: CorrelationId,
                  latencies: List<List<Response*Latency>>,
                  exceptions: List<Option<exn>*ExceptionCount>, ct: CancellationToken) = task {
        
        do! Task.Delay(10)        
        let timer = Stopwatch()

        while not ct.IsCancellationRequested do
            
            let mutable request = { CorrelationId = correlationId; Payload = null }
            let mutable skipStep = false
            let mutable stepIndex = 0

            for st in steps do
                if not skipStep && not ct.IsCancellationRequested then
                    try
                        let! (response,latency) = Step.execStep(st, request, timer)
                            
                        if not(Step.isPause st) then                        
                            latencies.[stepIndex].Add(response,latency)

                        if response.IsOk then 
                            request <- { request with Payload = response.Payload }
                            stepIndex <- stepIndex + 1
                        else
                            skipStep <- true
                        
                    with ex -> let (_, exCount) = exceptions.[stepIndex]
                               let newCount = exCount + 1
                               exceptions.[stepIndex] <- (Some(ex), newCount)
                               skipStep <- true
    }

module TestFlow =
    
    let create (flowIndex, config: Contracts.TestFlow) =
        { FlowName       = config.FlowName
          Steps          = config.Steps |> Seq.map(fun x -> x :?> Step) |> Seq.toArray
          CorrelationIds = createCorrelationId(flowIndex, config.ConcurrentCopies) }
        |> initFlow

    let initFlow (flow: TestFlow) =        
        flow.Steps
        |> Array.filter(Step.isListener)
        |> Array.map(Step.getListener)        
        |> Array.iter(initListeners(flow))
        flow

    let initListeners (flow: TestFlow) (listStep: ListenerStep) = 
        flow.CorrelationIds
        |> Set.toArray
        |> Array.map(StepListener)
        |> Array.append([|StepListener(Constants.WarmUpId)|])
        |> listStep.Listeners.Init 

    let createCorrelationId (flowIndex: int, concurrentCopies: int) =
        [|0 .. concurrentCopies - 1|] 
        |> Array.map(fun flowCopyNumber -> String.Format("{0}_{1}", flowIndex, flowCopyNumber) )
        |> Set.ofArray    

    let warmUpFlow (flow: TestFlow) = task {
        let timer = Stopwatch()        
        let steps = flow.Steps |> Array.filter(fun st -> not(Step.isPause st))        
        let mutable request = { CorrelationId = Constants.WarmUpId; Payload = null }
        let mutable result = Ok()
        let mutable skipStep = false

        for st in steps do
            if not skipStep then        
                try            
                    let! (response,_) = Step.execStep(st, request, timer)
                    if response.IsOk then
                        request <- { request with Payload = response.Payload }
                    else 
                        skipStep <- true                        
                        result <- Error({ FlowName = flow.FlowName; StepName = Step.getName(st); Error = response.Payload.ToString() })

                with ex -> skipStep <- true
                           result <- Error({ FlowName = flow.FlowName; StepName = Step.getName(st); Error = ex.Message })
        return result
    }  

module Scenario =    

    let create (config: Contracts.Scenario) =
        { ScenarioName = config.ScenarioName
          InitStep = config.TestInit |> Option.map(fun x -> Step.getRequest(x :?> Step))
          TestFlows = config.TestFlows |> Array.mapi(fun i config -> TestFlow.create(i, config))
          Duration = config.Duration
          Assertions = config.Assertions |> Array.map(fun x -> x :?> Assertion)  }
          
    let init (scenario: Scenario) =
        match scenario.InitStep with
        | Some step -> 
            try                 
                let req = { CorrelationId = Constants.InitId; Payload = null }
                step.Execute(req).Wait()                
                Ok <| scenario
            with ex -> Error <| InitStepError(ex.Message)
        | None      -> Ok <| scenario

    let warmUp (scenario: Scenario) =                
        let errors = scenario.TestFlows 
                     |> Array.map(fun flow -> TestFlow.warmUpFlow(flow).Result)
                     |> Array.filter(Result.isError)
                     |> Array.map(Result.getError)
        
        if errors.Length > 0 then Error <| FlowErrors(errors)
        else Ok <| scenario

module Assertions =

    let applyAssertions (scenarioName: string, assertions: Assertion[]) (flows: AssertionStats[]) =         
       assertions 
       |> Array.mapi(fun i assertion -> applyAssertion(scenarioName, flows, i+1, assertion))
       |> printAssertionResults

    let private applyAssertion (scenarioName: string, flows: AssertionStats[], i: int, assertion: Assertion) : string option =
       match assertion with
       | Scenario func ->            
            flows
            |> Array.groupBy(fun flow -> flow.FlowName)
            |> Array.map(fun (_,steps) -> applyForSteps func steps)
            |> Array.exists(id)
            |> createAssertionResult("Scenario", scenarioName, i)

       | TestFlow (flowName,func) ->
           flows
           |> Array.filter(fun flow -> flow.FlowName = flowName)
           |> applyForSteps(func) 
           |> createAssertionResult("Test Flow", flowName, i)

       | Step (stepName,flowName,func) -> 
           flows
           |> Array.filter(fun x -> x.FlowName = flowName && x.StepName = stepName)
           |> applyForSteps(func)
           |> createAssertionResult("Step", stepName, i)

    let private applyForSteps (assertion: AssertionFunc) (steps: AssertionStats[]) =            
        steps
        |> Array.map(assertion)
        |> Array.exists(id)

    let private printAssertionResults (results: string option[]) =
        let allAreOk = results |> Array.forall(function | None -> true | _ -> false)

        if allAreOk then results |> Array.length |> Ok
        else results |> Array.choose(fun x -> match x with | None -> None | Some msg -> Some(msg)) |> Error
    
    let private createAssertionResult (scope: string, reference: string, position: int) (executed: bool) : string option =
        if executed then None
        else sprintf "Assertion #%i FAILED for %s '%s'" position scope reference |> Some
    
module Constants =

    [<Literal>]
    let WarmUpId = "warm_up_flow"

    [<Literal>]
    let InitId = "init_step"