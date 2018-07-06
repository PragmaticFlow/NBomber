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

type Flow = {
    Name: string
    Steps: Step[]
    ConcurrentCopies: int
}

type Scenario = {
    Name: string
    InitFlow: Step option
    Flows: Flow[]
    Interval: TimeSpan
}

type ScenarioBuilder(scenarioName: string) =
    
    let flows = Dictionary<string, Flow>()
    let mutable initFlow = None    

    let validateFlow (flow) =
        let uniqCount = flow.Steps |> Array.map(fun c -> c.Name) |> Array.distinct |> Array.length
        
        if flow.Steps.Length <> uniqCount then
            failwith "all commands in job should have unique names"

    member x.Init(initFunc: Func<Task>) =
        let flow = { Name = "init"; Execute = initFunc.Invoke }
        initFlow <- Some(flow)
        x

    member x.AddFlow(name: string, commands: Step[],
                     [<Optional; DefaultParameterValue(0)>] concurrentCopies: int) =

        let flow = { Name = name; Steps = commands; ConcurrentCopies = concurrentCopies }
        x.AddFlow(flow)

    member x.AddFlow(job: Flow) =
        validateFlow(job)        
        flows.[job.Name] <- job
        x

    member x.Build(interval: TimeSpan) =
        let j = flows
                |> Seq.map (|KeyValue|)
                |> Seq.map (fun (name,job) -> job)
                |> Seq.toArray

        { Name = scenarioName
          InitFlow = initFlow
          Flows = j
          Interval = interval }