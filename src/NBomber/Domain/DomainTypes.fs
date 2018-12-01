module internal NBomber.Domain.DomainTypes

open System
open System.Collections.Generic
open System.Threading.Tasks
open System.Runtime.CompilerServices

open NBomber.Contracts

[<assembly: InternalsVisibleTo("NBomber.IntegrationTests")>]
do()

module Constants =

    [<Literal>]
    let WarmUpId = "warm_up_step"

    [<Literal>]
    let InitId = "init_step"

    [<Literal>]
    let DefaultDurationInSeconds = 20.0

    [<Literal>]
    let DefaultConcurrentCopies = 50

type StepName = string
type FlowName = string
type ScenarioName = string

type PushListener(id: string) = 

    let mutable tcs = TaskCompletionSource<Response>()    

    static member BuildId(correlationId: string, pushStepName: string) = 
        correlationId + "_" + pushStepName

    member x.Id = id

    member x.Notify(response: Response) = 
        if not tcs.Task.IsCompleted then tcs.SetResult(response)

    member x.GetResponse() = 
        tcs <- TaskCompletionSource<Response>()
        tcs.Task

type GlobalUpdatesChannel() =

    let mutable listeners = Dictionary<CorrelationId,PushListener>()        

    member x.Init(allCorrelationIds: string[], pushStepNames: string[]) = 
        listeners.Clear()        
        allCorrelationIds
        |> Array.collect(fun id -> pushStepNames 
                                   |> Array.map(fun stepName -> PushListener.BuildId(id, stepName)))
        |> Array.append(pushStepNames 
                        |> Array.map(fun x -> PushListener.BuildId(Constants.WarmUpId, x)))
        |> Array.map(PushListener)
        |> Array.iter(fun x -> listeners.Add(x.Id, x))

    member x.GetPushListener(correlationId: string, pushStepName: string) = 
        let id = PushListener.BuildId(correlationId, pushStepName)
        listeners.[id]

    interface IGlobalUpdatesChannel with
        member x.ReceivedUpdate(correlationId: string, pushStepName: string, response: Response) =
            let id = PushListener.BuildId(correlationId, pushStepName)
            match listeners.TryGetValue(id) with
            | true, listener -> listener.Notify(response)
            | _              -> ()

type PullStep = {
    StepName: StepName
    Execute: Request -> Task<Response>
}

type PushStep = {
    StepName: StepName    
    UpdatesChannel: GlobalUpdatesChannel
}

type Step =
    | Pull  of PullStep
    | Push  of PushStep
    | Pause of TimeSpan 
    interface IStep
        with member x.Name = match x with
                             | Pull s -> s.StepName
                             | Push s -> s.StepName
                             | Pause s -> "pause"
            

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
    TestInit: PullStep option    
    Steps: Step[]
    Assertions: Assertion[]
    ConcurrentCopies: int
    CorrelationIds: CorrelationId[]
    Duration: TimeSpan
}