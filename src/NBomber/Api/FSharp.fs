namespace rec NBomber.FSharp

open System
open System.Threading.Tasks
open System.Runtime.InteropServices

open NBomber
open NBomber.Contracts
open NBomber.Domain

module Step =

    let createRequest (name: string, execute: Request -> Task<Response>) = 
        Request({ StepName = name; Execute = execute }) :> IStep  
    
    let createListener (name: string, listeners: IStepListenerChannel) = 
        let ls = listeners :?> StepListenerChannel
        Listener({ StepName = name; Listeners = ls }) :> IStep

    let createPause (duration) = Pause(duration) :> IStep

    let createListenerChannel () = StepListenerChannel() :> IStepListenerChannel
    
module Scenario =
    
    let create (name: string): ScenarioConfig =        
        { ScenarioName = name
          TestInit = None
          TestFlows = Array.empty
          Duration = TimeSpan.FromSeconds(10.0) }

    let addTestInit (initFunc: Request -> Task<Response>) (scenario: ScenarioConfig) =
        let step = Step.createRequest(Constants.InitId, initFunc)
        { scenario with TestInit = Some(step) }

    let addTestFlow (testFlow: TestFlowConfig) (scenario: ScenarioConfig) =        
        { scenario with TestFlows = Array.append scenario.TestFlows [|testFlow|] }

    let build (duration: TimeSpan) (scenario: ScenarioConfig) =
        { scenario with Duration = duration }

    let run (scenario: ScenarioConfig) = ScenarioRunner.Run(scenario)