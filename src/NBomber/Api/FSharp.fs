namespace rec NBomber.FSharp

open System
open System.Threading.Tasks
open System.Runtime.InteropServices
open NBomber

module Step =

    let request (name: string, execute: Request -> Task<Response>) =
        Request({ StepName = name; Execute = execute })   

    let listener (name: string, listeners: StepListeners) =
        Listener({ StepName = name; Listeners = listeners })

    let pause (duration) = Pause(duration)


module Scenario =
    
    let create (name: string) =        
        { ScenarioName = name
          InitStep = None
          Flows = Array.empty
          Duration = TimeSpan.FromSeconds(10.0) }

    let addInit (initFunc: Request -> Task<Response>) (scenario: Scenario) =
        let step = { StepName = "init"; Execute = initFunc }
        { scenario with InitStep = Some(step) }

    let addTestFlow (name: string, steps: Step list, concurrentCopies: int) (scenario: Scenario) =
        let flowIndex = Array.length(scenario.Flows)    
        let flow = TestFlow.create(flowIndex, name, List.toArray(steps), concurrentCopies)
        { scenario with Flows = Array.append scenario.Flows [|flow|] }

    let build (duration: TimeSpan) (scenario: Scenario) =
        { scenario with Duration = duration }