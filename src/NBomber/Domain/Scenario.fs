namespace rec NBomber

open System
open System.Collections.Generic
open System.Threading.Tasks
open System.Runtime.InteropServices

type CorrelationId = string
type StepName = string
type FlowName = string

[<Struct>]
type Request = {
    CorrelationId: CorrelationId
    Payload: obj
}

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
    CorrelationIds: Set<CorrelationId>
}

type Scenario = {
    ScenarioName: string
    InitStep: RequestStep option
    Flows: TestFlow[]    
    Duration: TimeSpan
}   

type StepListeners() =

    let mutable listeners = Dictionary<CorrelationId,StepListener>()

    member internal x.Init(items: StepListener[]) = 
        items |> Array.iter(fun x -> listeners.Add(x.CorrelationId, x))

    member internal x.Get(correlationId: string) = listeners.[correlationId]

    member x.Notify(correlationId: string, response: Response) =
        match listeners.TryGetValue(correlationId) with
        | true, listener -> listener.Notify(response)
        | _              -> ()                

type internal StepListener(correlationId: string) = 

    let mutable tcs = TaskCompletionSource<Response>()    

    member x.CorrelationId = correlationId

    member x.Notify(response: Response) = 
        if not tcs.Task.IsCompleted then tcs.SetResult(response)

    member x.GetResponse() = 
        tcs <- TaskCompletionSource<Response>()
        tcs.Task


type Response with
    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj) = { IsOk = true; Payload = payload }
    static member Fail(error: string) = { IsOk = false; Payload = error }

module internal Step =
    
    let isRequest(step)  = match step with | Request _  -> true | _ -> false
    let isListener(step) = match step with | Listener _ -> true | _ -> false
    let isPause(step)    = match step with | Pause _    -> true | _ -> false    
    
    let getName(step) = 
        match step with
        | Request s  -> s.StepName
        | Listener s -> s.StepName
        | Pause t    -> "pause"
        
    let getListener(step) = 
        match step with 
        | Listener l -> l
        | _          -> failwith "step is not a Listener"

module internal TestFlow =
    
    let create (flowIndex, name, steps, concurrentCopies) =
        { FlowName = name
          Steps = steps
          CorrelationIds = createCorrelationId(flowIndex, concurrentCopies) }

    let initFlow (flow: TestFlow) =        
        flow.Steps
        |> Array.filter(Step.isListener)
        |> Array.map(Step.getListener)        
        |> Array.iter(initListeners(flow))

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

module internal Constants =

    [<Literal>]
    let WarmUpId = "warm_up_flow"

    [<Literal>]
    let InitId = "init_up_step"