namespace rec NBomber.CSharp

open System
open System.Collections.Generic
open System.Threading.Tasks

open NBomber.Contracts
open NBomber.FSharp
open NBomber

type Step =    
    static member CreateRequest(name: string, execute: Func<Request,Task<Response>>) = Step.createRequest(name, execute.Invoke)
    static member CreateListener(name: string, listeners: IStepListenerChannel) = Step.createListener(name, listeners)
    static member CreatePause(duration) = Step.createPause(duration)
    static member CreateListenerChannel() = Step.createListenerChannel()

type Assertion =
    static member ForScenario (assertion: Func<AssertionStats, bool>) = Assert.forScenario(assertion.Invoke)
    static member ForTestFlow (flowName, assertion: Func<AssertionStats, bool>) = Assert.forTestFlow(flowName, assertion.Invoke)
    static member ForStep (flowName, stepName, assertion: Func<AssertionStats, bool>) = Assert.forStep(flowName, stepName, assertion.Invoke)

type ScenarioBuilder(scenarioName: string) =
    
    let flows = Dictionary<string, TestFlow>()
    let mutable testInit = None       

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
         
    member x.Build(duration: TimeSpan): Contracts.Scenario =
        let flowConfigs = flows
                          |> Seq.map (|KeyValue|)
                          |> Seq.map (fun (name,job) -> job)
                          |> Seq.toArray

        { ScenarioName = scenarioName
          TestInit = testInit
          TestFlows = flowConfigs          
          Duration = duration
          Assertions = Array.empty }

type ScenarioRunner =

    static member Run (scenario: Scenario) = ScenarioRunner.Run(scenario, Array.empty, true) |> ignore

    static member RunWithAssertions (scenario: Scenario, assertions: IAssertion[]) =
        ScenarioRunner.Run(scenario, assertions, true) |> ignore

    static member Print (scenario: Scenario, assertions: IAssertion[]) =
        ScenarioRunner.Print(scenario, assertions)

    static member ApplyAssertions (scenario: Scenario, assertions: IAssertion[]) =
        ScenarioRunner.Run(scenario, assertions, false) |> Assertions.test
