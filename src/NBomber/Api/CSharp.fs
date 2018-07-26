namespace rec NBomber.CSharp

open System
open System.Collections.Generic
open System.Threading.Tasks

open NBomber.Contracts
open NBomber.Domain
open NBomber.FSharp

type Step =    
    static member CreateRequest(name: string, execute: Func<Request,Task<Response>>) = Step.createRequest(name, execute.Invoke)
    static member CreateListener(name: string, listeners: IStepListenerChannel) = Step.createListener(name, listeners)
    static member CreatePause(duration) = Step.createPause(duration)
    static member CreateListenerChannel() = Step.createListenerChannel()

type ScenarioBuilder(scenarioName: string) =
    
    let flows = Dictionary<string, TestFlowConfig>()
    let mutable testInit = None        

    member x.AddTestInit(initFunc: Func<Request,Task<Response>>) =
        let step = Step.CreateRequest(Constants.InitId, initFunc)        
        testInit <- Some(step)
        x

    member x.AddTestFlow(name: string, steps: IStep[], concurrentCopies: int) =        
        let flowConfig = { FlowName = name
                           Steps = steps
                           ConcurrentCopies = concurrentCopies }
                           
        flows.[flowConfig.FlowName] <- flowConfig
        x
        
    member x.Build(duration: TimeSpan): ScenarioConfig =
        let flowConfigs = flows
                          |> Seq.map (|KeyValue|)
                          |> Seq.map (fun (name,job) -> job)
                          |> Seq.toArray

        { ScenarioName = scenarioName
          TestInit = testInit
          TestFlows = flowConfigs          
          Duration = duration }