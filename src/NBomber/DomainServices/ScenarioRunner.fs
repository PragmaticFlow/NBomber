module internal NBomber.DomainServices.ScenarioRunner

open System.Collections.Generic
open System.Threading

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Statistics

type ScenarioActor(correlationId: string, scenario: Scenario) =
    
    let mutable currentTask = None        
    let mutable currentCts = None
    let stepsWithoutPause = scenario.Steps |> Array.filter(fun st -> not(Step.isPause st))
    let latencies = List<List<Response*Latency>>()

    let init () =
        stepsWithoutPause |> Array.iter(fun _ -> latencies.Add(List<Response*Latency>()))
    do init()

    member x.Run() = 
        currentCts <- Some(new CancellationTokenSource())
        currentTask <- Some(Step.runSteps(scenario.Steps, correlationId, latencies, currentCts.Value.Token))

    member x.Stop() = if currentCts.IsSome then currentCts.Value.Cancel()
        
    member x.GetResults() =
        stepsWithoutPause
        |> Array.mapi(fun i st -> StepStats.create(Step.getName(st), latencies.[i]))

type ScenarioRunner(scenario: Scenario) =                       

    let createActors () =
        scenario.CorrelationIds |> Array.map(fun id -> ScenarioActor(id, scenario))

    let mutable finished = false
    let actors = createActors()
    let scnTimer = new System.Timers.Timer()

    let stop () =         
        actors |> Array.iter(fun x -> x.Stop())
        scnTimer.Stop()
        finished <- true

    let run () =        
        stop()
        finished <- false
        scnTimer.Start()
        actors |> Array.iter(fun x -> x.Run())            

    do scnTimer.Interval <- scenario.Duration.TotalMilliseconds
       scnTimer.Elapsed.Add(fun _ -> stop())

    member x.Finished = finished
    member x.Scenario = scenario
    member x.WarmUp() = Scenario.warmUp(scenario)
    member x.RunInit() = Scenario.runInit(scenario)
    member x.Run() = run()
    member x.Stop() = stop()
    
    member x.GetResult() =
        actors
        |> Array.collect(fun actor -> actor.GetResults())
        |> ScenarioStats.create(scenario)