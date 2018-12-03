namespace NBomber.CSharp

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices

open NBomber
open NBomber.Contracts
open NBomber.FSharp

type GlobalUpdatesChannel =
    static member Instance = FSharp.GlobalUpdatesChannel.Instance

type Step =    
    static member CreatePull(name: string, execute: Func<Request,Task<Response>>) = FSharp.Step.createPull(name, execute.Invoke)
    static member CreatePush(name: string) = FSharp.Step.createPush(name)
    static member CreatePause(duration) = FSharp.Step.createPause(duration)

type Assertion =    
    static member ForStep (stepName, assertion: Func<AssertStats, bool>) = Assertion.forStep(stepName, assertion.Invoke)    

[<Extension>]
type ScenarioBuilder =

    static member CreateScenario(name: string, [<System.ParamArray>]steps: IStep[]) =
        FSharp.Scenario.create(name, Seq.toList(steps))
    
    [<Extension>]
    static member WithTestInit(scenario: Scenario, initFunc: Func<Request,Task<Response>>) = 
        scenario |> FSharp.Scenario.withTestInit(initFunc.Invoke)

    [<Extension>]
    static member WithAssertions(scenario: Scenario, [<System.ParamArray>]assertions: IAssertion[]) = 
        scenario |> FSharp.Scenario.withAssertions(Seq.toList(assertions))

    [<Extension>]
    static member WithConcurrentCopies(scenario: Scenario, concurrentCopies: int) = 
        scenario |> FSharp.Scenario.withConcurrentCopies(concurrentCopies)

    [<Extension>]
    static member WithDuration(scenario: Scenario, duration: TimeSpan) = 
        scenario |> FSharp.Scenario.withDuration(duration)

[<Extension>]
type NBomberRunner =    
        
    static member RegisterScenarios([<System.ParamArray>]scenarios: Contracts.Scenario[]) = 
        scenarios |> Seq.toList |> FSharp.NBomberRunner.registerScenarios    

    [<Extension>]
    static member LoadConfig(context: NBomberRunnerContext, path: string) =
        context |> FSharp.NBomberRunner.loadConfig(path)

    [<Extension>]
    static member Run(context: NBomberRunnerContext) =
        FSharp.NBomberRunner.run(context)
        
    [<Extension>]
    static member RunInConsole(context: NBomberRunnerContext) =
        FSharp.NBomberRunner.runInConsole(context)

    [<Extension>]
    static member RunTest(context: NBomberRunnerContext) =
        FSharp.NBomberRunner.runTest(context)