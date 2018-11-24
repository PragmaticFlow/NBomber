module internal NBomber.Domain.DomainTypes

open System
open System.Collections.Generic
open System.Threading.Tasks

open NBomber.Contracts

module Constants =

    [<Literal>]
    let WarmUpId = "warm_up_flow"

    [<Literal>]
    let InitId = "init_step"

type StepName = string
type FlowName = string
type ScenarioName = string

type StepListener(correlationId: CorrelationId) = 

    let mutable tcs = TaskCompletionSource<Response>()    

    member x.CorrelationId = correlationId

    member x.Notify(response: Response) = 
        if not tcs.Task.IsCompleted then tcs.SetResult(response)

    member x.GetResponse() = 
        tcs <- TaskCompletionSource<Response>()
        tcs.Task

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

type AssertFunc = AssertStats -> bool

type StepAssertion = {
    StepName: StepName
    ScenarioName: ScenarioName
    AssertFunc: AssertFunc
}

type ScenarioAssertion = {
    ScenarioName: ScenarioName
    AssertFunc: AssertFunc
}

type Assertion = 
    | Step     of StepAssertion
    | Scenario of ScenarioAssertion
    interface IAssertion

type Scenario = {    
    ScenarioName: ScenarioName
    TestInit: RequestStep option    
    Steps: Step[]
    Assertions: Assertion[]
    ConcurrentCopies: int
    Duration: TimeSpan    
}