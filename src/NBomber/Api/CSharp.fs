namespace rec NBomber.CSharp

open System
open System.Collections.Generic
open System.Threading.Tasks

open NBomber.Contracts
open NBomber.FSharp

type Step =    
    static member CreateRequest(name: string, execute: Func<Request,Task<Response>>) = Step.createRequest(name, execute.Invoke)
    static member CreateListener(name: string, listeners: IStepListenerChannel) = Step.createListener(name, listeners)
    static member CreatePause(duration) = Step.createPause(duration)
    static member CreateListenerChannel() = Step.createListenerChannel()

type Assert =
    static member ForScenario (assertion: Func<AssertionStats, bool>) = Assertion.Scenario(assertion)
    static member ForTestFlow (flowName, assertion: Func<AssertionStats, bool>) = Assertion.TestFlow(flowName, assertion)
    static member ForStep (flowName, stepName, assertion: Func<AssertionStats, bool>) = Assertion.Step(flowName, stepName, assertion)

type ScenarioBuilder(scenarioName: string) =
    
    let flows = Dictionary<string, TestFlow>()
    let mutable testInit = None
    let mutable asserts = Array.empty        

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
        
    member x.AddAssertions(assertions : Assertion[]) =
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
