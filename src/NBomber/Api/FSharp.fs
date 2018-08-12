namespace rec NBomber.FSharp

open System
open System.Threading.Tasks
open System.Runtime.InteropServices

open NBomber
open NBomber.Contracts
open NBomber.ScenarioRunner

module Step =
    open NBomber.Domain

    let createRequest (name: string, execute: Request -> Task<Response>) = 
        Request({ StepName = name; Execute = execute }) :> IStep  
    
    let createListener (name: string, listeners: IStepListenerChannel) = 
        let ls = listeners :?> StepListenerChannel
        Listener({ StepName = name; Listeners = ls }) :> IStep

    let createPause (duration) = Pause(duration) :> IStep

    let createListenerChannel () = StepListenerChannel() :> IStepListenerChannel

module Assertion =
    open NBomber.Domain 

    let forScenario (assertion: AssertionStats -> bool) = Scenario(assertion) :> IAssertion
    let forTestFlow (flowName, assertion: AssertionStats -> bool) = TestFlow(flowName, assertion) :> IAssertion
    let forStep (flowName, stepName, assertion: AssertionStats -> bool) = Step(flowName, stepName, assertion) :> IAssertion

module Scenario =
    
    let create (name: string): Scenario =        
        { ScenarioName = name
          TestInit = None
          TestFlows = Array.empty
          Duration = TimeSpan.FromSeconds(10.0)
          Assertions = Array.empty }

    let addTestInit (initFunc: Request -> Task<Response>) (scenario: Scenario) =
        let step = Step.createRequest(Domain.Constants.InitId, initFunc)
        { scenario with TestInit = Some(step) }

    let addTestFlow (testFlow: Contracts.TestFlow) (scenario: Scenario) =        
        { scenario with TestFlows = Array.append scenario.TestFlows [|testFlow|] }

    let withDuration (duration: TimeSpan) (scenario: Scenario) =
        { scenario with Duration = duration }

    let run (scenario: Scenario) = Run(scenario, Array.empty, true) |> ignore

    let runWithAssertions (assertions: IAssertion[]) (scenario: Scenario) =
        Run(scenario, assertions, true) |> ignore

    let print (assertions: IAssertion[]) (scenario: Scenario) = Print(scenario, assertions)

    let applyAssertions (assertions: IAssertion[]) (scenario: Scenario) =
        Run(scenario, assertions, false) |> Assertions.test

    
