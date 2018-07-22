namespace rec NBomber.CSharp

open System
open System.Collections.Generic
open System.Threading.Tasks

open NBomber

type StepFactory =    
    
    static member Request(name: string, execute: Func<Request,Task<Response>>) =
        Request({ StepName = name; Execute = execute.Invoke })

    static member Listener(name: string, listeners: StepListeners) =
        Listener({ StepName = name; Listeners = listeners })

    static member Pause(duration) = Pause(duration)

type Assert =
    static member ForAll (assertion) = AssertScope.Scenario(assertion)
    static member ForFlow (flowName, assertion) = AssertScope.TestFlow(flowName, assertion)
    static member ForStep (flowName, stepName, assertion) = AssertScope.Step(flowName, stepName, assertion)

type ScenarioBuilder(scenarioName: string) =
    
    let flows = Dictionary<string, TestFlow>()

    let mutable asserts = [||]

    let mutable initStep = None    

    //let validateFlow (flow) =
    //    let uniqCount = flow.Steps |> Array.map(fun c -> c.StepName) |> Array.distinct |> Array.length
        
    //    if flow.Steps.Length <> uniqCount then
    //        failwith "all steps in test flow should have unique names"

    member x.AddInit(initFunc: Func<Request,Task<Response>>) =
        let step = { StepName = "init"; Execute = initFunc.Invoke }
        initStep <- Some(step)
        x

    member x.AddTestFlow(name: string, steps: Step[], concurrentCopies: int) =        
        let flowIndex = flows.Count
        let flow = TestFlow.create(flowIndex, name, steps, concurrentCopies)
        flows.[flow.FlowName] <- flow
        x

    member x.AddAsserts(assertions : AssertScope[]) =
         asserts <- assertions
         x
    member x.Build(duration: TimeSpan) =
        let testFlows = flows
                        |> Seq.map (|KeyValue|)
                        |> Seq.map (fun (name,job) -> job)
                        |> Seq.toArray

        { ScenarioName = scenarioName
          InitStep = initStep
          Flows = testFlows
          //Assertions = asserts
          Duration = duration }