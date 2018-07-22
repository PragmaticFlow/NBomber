module rec NBomber.FSharp

open System
open System.Threading.Tasks

type Step with
    static member CreateRequest(name: string, execute: Request -> Task<Response>) =
        Request({ StepName = name; Execute = execute })   

    static member CreateListener(name: string, listeners: StepListeners) =
        Listener({ StepName = name; Listeners = listeners })

    static member CreatePause(duration) = Pause(duration)

let scenario (name: string) =
    { ScenarioName = name
      InitStep = None
      Flows = Array.empty
      Duration = TimeSpan.FromSeconds(10.0) }

let init (initFunc: Request -> Task<Response>) (scenario: Scenario) =
    let step = { StepName = "init"; Execute = initFunc }
    { scenario with InitStep = Some(step) }

let addTestFlow (name: string, steps: Step list, concurrentCopies: int) (scenario: Scenario) =
    let flowIndex = Array.length(scenario.Flows)    
    let flow = TestFlow.create(flowIndex, name, List.toArray(steps), concurrentCopies)
    { scenario with Flows = Array.append scenario.Flows [|flow|] }

let build (duration: TimeSpan) (scenario: Scenario) =
    { scenario with Duration = duration }