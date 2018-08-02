namespace rec NBomber.CSharp

open System
open System.Collections.Generic
open System.Threading.Tasks

open NBomber.Contracts
open NBomber.FSharp
open NBomber.Assertions

type Step =    
    static member CreateRequest(name: string, execute: Func<Request,Task<Response>>) = Step.createRequest(name, execute.Invoke)
    static member CreateListener(name: string, listeners: IStepListenerChannel) = Step.createListener(name, listeners)
    static member CreatePause(duration) = Step.createPause(duration)
    static member CreateListenerChannel() = Step.createListenerChannel()

type Assert =
    static member ForScenario (assertion:Func<AssertionInfo, bool>) = AssertionScope.Scenario(assertion)
    static member ForTestFlow (flowName, assertion:Func<AssertionInfo, bool>) = AssertionScope.TestFlow(flowName, assertion)
    static member ForStep (flowName, stepName, assertion:Func<AssertionInfo, bool>) = AssertionScope.Step(flowName, stepName, assertion)

type ScenarioBuilder(scenarioName: string) =
    
    let flows = Dictionary<string, TestFlow>()
    let mutable testInit = None
    let mutable asserts = [||]        

    member x.AddTestInit(initFunc: Func<Request,Task<Response>>) =
        let step = Step.CreateRequest(NBomber.Domain.Constants.InitId, initFunc)        
        testInit <- Some(step)
        x

    member x.AddTestFlow(name: string, steps: IStep[], concurrentCopies: int) =        
        let flowConfig = { FlowName = name
                           Steps = steps
                           ConcurrentCopies = concurrentCopies }
                           
        flows.[flowConfig.FlowName] <- flowConfig
        x
        
     member x.AddAssertions(assertions : AssertionScope[]) =
         asserts <- assertions
         x
         
    member x.Build(duration: TimeSpan): Scenario =
        let flowConfigs = flows
                          |> Seq.map (|KeyValue|)
                          |> Seq.map (fun (name,job) -> job)
                          |> Seq.toArray

        { ScenarioName = scenarioName
          TestInit = testInit
          TestFlows = flowConfigs          
          Duration = duration
          Assertions = asserts }
