namespace rec NBomber.CSharp

open System
open System.Collections.Generic
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open NBomber
open NBomber.Contracts
open NBomber.FSharp

type Step =    
    static member CreateRequest(name: string, execute: Func<Request,Task<Response>>) = Step.createRequest(name, execute.Invoke)
    static member CreateListener(name: string, listeners: IStepListenerChannel) = Step.createListener(name, listeners)
    static member CreatePause(duration) = Step.createPause(duration)
    static member CreateListenerChannel() = Step.createListenerChannel()

type Assertion =
    static member ForScenario (assertion: Func<AssertionStats, bool>) = Assertion.forScenario(assertion.Invoke)
    static member ForTestFlow (flowName, assertion: Func<AssertionStats, bool>) = Assertion.forTestFlow(flowName, assertion.Invoke)
    static member ForStep (stepName, flowName, assertion: Func<AssertionStats, bool>) = Assertion.forStep(stepName, flowName, assertion.Invoke)
    
type ScenarioBuilder(scenarioName: string) =
    
    let mutable testInit = None       
    let flows = Dictionary<string, TestFlow>()
    let mutable asserts = Array.empty    

    member x.WithTestInit(initFunc: Func<Request,Task<Response>>) =
        let step = Step.CreateRequest(NBomber.Domain.Constants.InitId, initFunc)        
        testInit <- Some(step)
        x

    member x.AddTestFlow(name: string, steps: IStep[], concurrentCopies: int) =        
        let flowConfig = { FlowName = name
                           Steps = steps
                           ConcurrentCopies = concurrentCopies }
                           
        flows.[flowConfig.FlowName] <- flowConfig
        x
         
    member x.WithAssertions(assertions: IAssertion[]) =
        asserts <- assertions
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
          Assertions = asserts }

[<Extension>]
type ScenarioExt =    

    [<Extension>]
    static member Run(scenario: Contracts.Scenario) = 
        ScenarioRunner.run(scenario, false) |> ignore    
    
    [<Extension>]
    static member RunInConsole(scenario: Contracts.Scenario) =
        ScenarioRunner.runInConsole(scenario)

    [<Extension>]
    static member RunTest(scenario: Contracts.Scenario, [<Optional;DefaultParameterValue(null:IAssertion[])>]assertions: IAssertion[]) =
        let scn = if isNull(assertions) then scenario
                  else  { scenario with Assertions = assertions }
        ScenarioRunner.run(scn, false) |> AssertIntegration.run
