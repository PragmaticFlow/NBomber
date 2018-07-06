namespace rec NBomber

open System
open System.Linq
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.InteropServices

type StepName = string

type Step = {
    Name: StepName    
    Execute: unit -> Task
} with
  static member Create(name: StepName, execute: Func<Task>) =
    { Name = name; Execute = execute.Invoke }

  static member CreatePause(delay: TimeSpan) =    
    { Name = "pause"; Execute = (fun () -> Task.Delay(delay)) }

type TestFlow = {
    Name: string
    Steps: Step[]
    ConcurrentCopies: int
}

type Scenario = {
    Name: string
    InitStep: Step option
    Flows: TestFlow[]
    Interval: TimeSpan
}

type ScenarioBuilder(scenarioName: string) =
    
    let flows = Dictionary<string, TestFlow>()
    let mutable initStep = None    

    let validateFlow (flow) =
        let uniqCount = flow.Steps |> Array.map(fun c -> c.Name) |> Array.distinct |> Array.length
        
        if flow.Steps.Length <> uniqCount then
            failwith "all steps in test flow should have unique names"

    member x.Init(initFunc: Func<Task>) =
        let flow = { Name = "init"; Execute = initFunc.Invoke }
        initStep <- Some(flow)
        x

    member x.AddTestFlow(name: string, steps: Step[],
                         [<Optional; DefaultParameterValue(0)>] concurrentCopies: int) =

        let flow = { Name = name; Steps = steps; ConcurrentCopies = concurrentCopies }
        x.AddTestFlow(flow)

    member x.AddTestFlow(job: TestFlow) =
        validateFlow(job)        
        flows.[job.Name] <- job
        x

    member x.Build(interval: TimeSpan) =
        let testFlows = flows
                        |> Seq.map (|KeyValue|)
                        |> Seq.map (fun (name,job) -> job)
                        |> Seq.toArray

        { Name = scenarioName
          InitStep = initStep
          Flows = testFlows
          Interval = interval }