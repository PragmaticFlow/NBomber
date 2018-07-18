namespace rec NBomber

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.InteropServices

open FSharp.Control.Tasks.V2.ContextInsensitive

type MsgCount = int
type FlowName = string
type StepName = string

type Request = obj

[<Struct>]
type Response = {
    IsOk: bool
    Payload: obj
}   

type ReqStep = {
    StepName: StepName
    Execute: Request -> Task<Response>
}

type ListenStep = {
    StepName: StepName    
    Listener: Listener
}

type PauseStep =
    static member Create(duration) = Pause(duration)

type Step =
    | Request of ReqStep
    | Listen  of ListenStep
    | Pause   of TimeSpan    

type TestFlow = {
    FlowName: FlowName
    Steps: Step[]
    ConcurrentCopies: int
}

type Scenario = {
    ScenarioName: string
    InitStep: ReqStep option
    Flows: TestFlow[]
    Duration: TimeSpan
}


type Response with
    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj) = { IsOk = true; Payload = payload }
    static member Fail(error: string) = { IsOk = false; Payload = error }

type ReqStep with
    static member Create(name: StepName, execute: Func<Request,Task<Response>>) =
        Request({ StepName = name; Execute = execute.Invoke })   

type ListenStep with
    static member Create(name: StepName) = 
        Listen({ StepName = name; Listener = Listener() })

type Step with    
    static member internal isRequest(step) = match step with | Request _ -> true | _ -> false
    static member internal isPush(step)    = match step with | Listen _    -> true | _ -> false
    static member internal isPause(step)   = match step with | Pause _   -> true | _ -> false    
    static member internal getName(step)   = 
        match step with
        | Request r -> r.StepName
        | Listen p    -> p.StepName
        | Pause t   -> "pause"

type Listener() =
    let mutable msgCounter = 0
    let mutable tcs = null
    
    let init () = 
        msgCounter <- 0
        tcs <- null

    member x.MsgCount = msgCounter

    member internal x.WaitOnResponse() = 
        tcs <- TaskCompletionSource<Response*MsgCount>()
        tcs.Task
        
    member x.ReceivedMsg(msg: Response, finishStep: bool) =
        msgCounter <- msgCounter + 1        
        if finishStep = true then            
            tcs.SetResult(msg, msgCounter)
            init()

type ScenarioBuilder(scenarioName: string) =
    
    let flows = Dictionary<string, TestFlow>()
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

    member x.Build(duration: TimeSpan) =
        let testFlows = flows
                        |> Seq.map (|KeyValue|)
                        |> Seq.map (fun (name,job) -> job)
                        |> Seq.toArray

        { ScenarioName = scenarioName
          InitStep = initStep
          Flows = testFlows
          Duration = duration }


module FSharpAPI =

    let scenario (scenarioName: string) =
        { ScenarioName = scenarioName
          InitStep = None
          Flows = Array.empty
          Duration = TimeSpan.FromSeconds(10.0) }

    let init (initFunc: Request -> Task<Response>) (scenario: Scenario) =
        let step = { StepName = "init"; Execute = initFunc }
        { scenario with InitStep = Some(step) }

    let addTestFlow (flow: TestFlow) (scenario: Scenario) =
        { scenario with Flows = Array.append scenario.Flows [|flow|] }

    let build (interval: TimeSpan) (scenario: Scenario) =
        { scenario with Duration = interval }