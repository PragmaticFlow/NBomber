namespace rec NBomber.FSharp

open System
open System.Threading.Tasks
open System.Runtime.InteropServices

open NBomber
open NBomber.Assertions
open NBomber.Contracts

module Step =
    open NBomber.Domain

    let createRequest (name: string, execute: Request -> Task<Response>) = 
        Request({ StepName = name; Execute = execute }) :> IStep  
    
    let createListener (name: string, listeners: IStepListenerChannel) = 
        let ls = listeners :?> StepListenerChannel
        Listener({ StepName = name; Listeners = ls }) :> IStep

    let createPause (duration) = Pause(duration) :> IStep

    let createListenerChannel () = StepListenerChannel() :> IStepListenerChannel

module Assert =
    let forScenario (assertion:AssertionInfo -> bool) = Scenario(AssertionFunc(assertion))
    let forTestFlow (flowName, assertion:AssertionInfo -> bool) = TestFlow(flowName, AssertionFunc(assertion))
    let forStep (flowName, stepName, assertion:AssertionInfo -> bool) = Step(flowName, stepName, AssertionFunc(assertion))

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

    let build (duration: TimeSpan) (scenario: Scenario) =
        { scenario with Duration = duration }
        
    let addAssertions(assertions: AssertionScope[]) (scenario: Scenario) =
         { scenario with Assertions = assertions}
    let run (scenario: Scenario) = ScenarioRunner.Run(scenario)
