namespace rec NBomber

open System
open System.Linq
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.InteropServices

open FSharp.Control.Tasks.V2.ContextInsensitive

type StepName = string

type StepResult =
    | Ok = 0
    | Fail = 1

type Step = {
    StepName: StepName    
    Execute: unit -> Task<StepResult>
} with
  static member Create(name: StepName, execute: Func<Task<StepResult>>) =
    { StepName = name; Execute = execute.Invoke }

  static member CreatePause(delay: TimeSpan) =    
    { StepName = "pause"
      Execute = (fun () -> task { do! Task.Delay(delay) 
                                  return StepResult.Ok }) }

type TestFlow = {
    FlowName: string
    Steps: Step[]
    ConcurrentCopies: int
}

type Scenario = {
    ScenarioName: string
    InitStep: Step option
    Flows: TestFlow[]
    Interval: TimeSpan
}

type ScenarioBuilder(scenarioName: string) =
    
    let flows = Dictionary<string, TestFlow>()
    let mutable initStep = None    

    let validateFlow (flow) =
        let uniqCount = flow.Steps |> Array.map(fun c -> c.StepName) |> Array.distinct |> Array.length
        
        if flow.Steps.Length <> uniqCount then
            failwith "all steps in test flow should have unique names"

    member x.Init(initFunc: Func<Task<StepResult>>) =
        let step = { StepName = "init"; Execute = initFunc.Invoke }
        initStep <- Some(step)
        x    

    member x.AddTestFlow(flow: TestFlow) =
        validateFlow(flow)        
        flows.[flow.FlowName] <- flow
        x

    member x.AddTestFlow(name: string, steps: Step[], concurrentCopies: int) =
        let flow = { FlowName = name; Steps = steps; ConcurrentCopies = concurrentCopies }
        x.AddTestFlow(flow)

    member x.Build(interval: TimeSpan) =
        let testFlows = flows
                        |> Seq.map (|KeyValue|)
                        |> Seq.map (fun (name,job) -> job)
                        |> Seq.toArray

        { ScenarioName = scenarioName
          InitStep = initStep
          Flows = testFlows
          Interval = interval }


module FSharpAPI =

    let scenario (scenarioName: string) =
        { ScenarioName = scenarioName
          InitStep = None
          Flows = Array.empty
          Interval = TimeSpan.FromSeconds(10.0) }

    let init (initFunc: unit -> Task<StepResult>) (scenario: Scenario) =
        let step = { StepName = "init"; Execute = initFunc }
        { scenario with InitStep = Some(step) }

    let addTestFlow (flow: TestFlow) (scenario: Scenario) =
        { scenario with Flows = Array.append scenario.Flows [|flow|] }

    let build (interval: TimeSpan) (scenario: Scenario) =
        { scenario with Interval = interval }