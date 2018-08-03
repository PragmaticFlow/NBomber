﻿namespace rec NBomber.FSharp

open System
open System.Threading.Tasks
open System.Runtime.InteropServices

open NBomber
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
    let forScenario (assertion: AssertionStats -> bool) = Assertion.Scenario(AssertionFunc(assertion))
    let forTestFlow (flowName, assertion: AssertionStats -> bool) = Assertion.TestFlow(flowName, AssertionFunc(assertion))
    let forStep (flowName, stepName, assertion: AssertionStats -> bool) = Assertion.Step(flowName, stepName, AssertionFunc(assertion))

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

    let addAssertions(assertions: Assertion list) (scenario: Scenario) =
        { scenario with Assertions = assertions |> Seq.toArray }

    let run (scenario: Scenario) = ScenarioRunner.Run(scenario)
