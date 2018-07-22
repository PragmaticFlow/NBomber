namespace rec NBomber

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.InteropServices
open FSharp.Control.Tasks.V2.ContextInsensitive

type StepName = string
type FlowName = string

[<Struct>]
type Request = {
    FlowId: int
    Payload: obj
}

type Latency = int64
type ExceptionCount = int

[<Struct>]
type Response = {
    IsOk: bool
    Payload: obj
}   

type RequestStep = {
    StepName: StepName
    Execute: Request -> Task<Response>
}

type ListenerStep = {
    StepName: StepName    
    Listeners: StepListeners
}

type Step =
    | Request  of RequestStep
    | Listener of ListenerStep
    | Pause    of TimeSpan    

type TestFlow = {
    FlowName: FlowName
    Steps: Step[]
    ConcurrentCopies: int
}

type Scenario = {
    ScenarioName: string
    InitStep: RequestStep option
    Flows: TestFlow[]
    Asserts: Assert[]
    Duration: TimeSpan
}

type StepInfo = {
    StepName: string
    Latencies: Latency[]
    ThrownException: exn option
    OkCount: int
    FailCount: int
    ExceptionCount: ExceptionCount
}

type AssertionStats = {
    OkCount: int
    FailCount: int
    ExceptionCount: int
} with
    static member Create (step:StepInfo) =
        { OkCount = step.OkCount; FailCount = step.FailCount; ExceptionCount = step.ExceptionCount }

type Scope = All | Flow | Step
type Target = string
type Index = int

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

type Assertion = delegate of AssertionStats -> bool

type Assert = 
    | All of Assertion
    | Flow of string * Assertion
    | Step of string * string * Assertion
    static member ForAll (assertion) = All(assertion)
    static member ForFlow (flowName, assertion) = Flow(flowName, assertion)
    static member ForStep (flowName, stepName, assertion) = Step(flowName, stepName, assertion)

type Response with
    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj) = { IsOk = true; Payload = payload }
    static member Fail(error: string) = { IsOk = false; Payload = error }

type Step with 
    static member CreateRequest(name: StepName, execute: Func<Request,Task<Response>>) =
        Request({ StepName = name; Execute = execute.Invoke })   

    static member CreateListener(name: StepName, listeners: StepListeners) =
        Listener({ StepName = name; Listeners = listeners })

    static member CreatePause(duration) = Pause(duration)

    static member internal isRequest(step)  = match step with | Request _  -> true | _ -> false
    static member internal isListener(step) = match step with | Listener _ -> true | _ -> false
    static member internal isPause(step)    = match step with | Pause _    -> true | _ -> false    
    
    static member internal getName(step)    = 
        match step with
        | Request r  -> r.StepName
        | Listener p -> p.StepName
        | Pause t    -> "pause"
        
    static member internal getListener(step) = 
        match step with 
        | Listener p -> p
        | _          -> failwith "step is not a Listener"


type StepListeners() =

    let mutable listeners = Array.empty

    member internal x.Init(items: FlowListener[]) = 
        listeners <- items

    member internal x.Get(flowId: int) = listeners.[flowId]

    member x.Notify(flowId: int, response: Response) =
        try
            listeners.[flowId].Notify(response)
        with _ -> ()
        

type internal FlowListener() = 

    let mutable tcs = TaskCompletionSource<Response>()

    member x.Notify(response: Response) = 
        if not tcs.Task.IsCompleted then tcs.SetResult(response)

    member x.GetResponse() = 
        tcs <- TaskCompletionSource<Response>()
        tcs.Task


type ScenarioBuilder(scenarioName: string) =
    
    let flows = Dictionary<string, TestFlow>()
    let mutable asserts = [||]

    let mutable initStep = None    

    //let validateFlow (flow) =
    //    let uniqCount = flow.Steps |> Array.map(fun c -> c.StepName) |> Array.distinct |> Array.length
        
    //    if flow.Steps.Length <> uniqCount then
    //        failwith "all steps in test flow should have unique names"

    member x.Init(initFunc: Func<Request,Task<Response>>) =
        let step = { StepName = "init"; Execute = initFunc.Invoke }
        initStep <- Some(step)
        x    

    member x.AddTestFlow(flow: TestFlow) =
        //validateFlow(flow)        
        flows.[flow.FlowName] <- flow
        x

    member x.AddTestFlow(name: string, steps: Step[], concurrentCopies: int) =
        let flow = { FlowName = name; Steps = steps; ConcurrentCopies = concurrentCopies }
        x.AddTestFlow(flow)

    member x.AddAsserts(assertions : Assert[]) =
        asserts <- assertions
        x
    member x.Build(duration: TimeSpan) =
        let testFlows = flows
                        |> Seq.map (|KeyValue|)
                        |> Seq.map (fun (name,job) -> job)
                        |> Seq.toArray

        { ScenarioName = scenarioName
          InitStep = initStep
          Flows = testFlows
          Asserts = asserts
          Duration = duration }

module StepInfo =

    let create(stepName, responseResults: List<Response*Latency>, 
               exceptions: (Option<exn>*ExceptionCount)) =
    
        let results = responseResults.ToArray()
    
        { StepName = stepName 
          Latencies = results |> Array.map(snd)
          ThrownException = fst(exceptions)                                  
          OkCount = results 
                    |> Array.map(fst)
                    |> Array.filter(fun stRes -> stRes.IsOk)
                    |> Array.length                                  
          FailCount = results 
                      |> Array.map(fst)
                      |> Array.filter(fun stRes -> not(stRes.IsOk))
                      |> Array.length
          ExceptionCount = snd(exceptions) }

    let toString(stepInfo: StepInfo) =
        String.Format("{0} (OK:{1}, Failed:{2}, Exceptions:{3})", stepInfo.Latencies.Length,
                      stepInfo.OkCount, stepInfo.FailCount, stepInfo.ExceptionCount)
                      
module FSharpAPI =

    let scenario (scenarioName: string) =
        { ScenarioName = scenarioName
          InitStep = None
          Flows = Array.empty
          Asserts = Array.empty
          Duration = TimeSpan.FromSeconds(10.0) }

    let init (initFunc: Request -> Task<Response>) (scenario: Scenario) =
        let step = { StepName = "init"; Execute = initFunc }
        { scenario with InitStep = Some(step) }

    let addTestFlow (flow: TestFlow) (scenario: Scenario) =
        { scenario with Flows = Array.append scenario.Flows [|flow|] }

    let addAsserts (asserts: Assert[]) (scenario: Scenario) =
        { scenario with Asserts = Array.append scenario.Asserts asserts }

    let build (interval: TimeSpan) (scenario: Scenario) =
        { scenario with Duration = interval }