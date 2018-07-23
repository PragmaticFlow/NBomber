namespace rec NBomber.CSharp

open System
open System.Collections.Generic
open System.Threading.Tasks

open NBomber

type StepFactory =    
    
    static member CreateRequest(name: string, execute: Func<Request,Task<Response>>) =
        Request({ StepName = name; Execute = execute.Invoke })

    static member CreateListener(name: string, listeners: StepListeners) =
        Listener({ StepName = name; Listeners = listeners })

    static member CreatePause(duration) = Pause(duration)

type ScenarioBuilder(scenarioName: string) =
    
    let flows = Dictionary<string, TestFlow>()
    let mutable initStep = None        

    member x.AddInit(initFunc: Func<Request,Task<Response>>) =
        let step = { StepName = "init"; Execute = initFunc.Invoke }
        initStep <- Some(step)
        x

    member x.AddTestFlow(name: string, steps: Step[], concurrentCopies: int) =        
        let flowIndex = flows.Count
        let flow = TestFlow.create(flowIndex, name, steps, concurrentCopies)
        flows.[flow.FlowName] <- flow
        x
        
    member x.Build(duration: TimeSpan) =
        let testFlows = flows
                        |> Seq.map (|KeyValue|)
                        |> Seq.map (fun (name,job) -> job)
                        |> Seq.toArray

        { ScenarioName = scenarioName
          InitStep = initStep
          Flows = testFlows          
          Duration = duration }