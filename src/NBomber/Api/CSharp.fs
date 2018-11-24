namespace NBomber.CSharp

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices

open NBomber
open NBomber.Configuration
open NBomber.Contracts
open NBomber.FSharp

type Step =    
    static member CreateRequest(name: string, execute: Func<Request,Task<Response>>) = Step.createRequest(name, execute.Invoke)
    static member CreateListener(name: string, listeners: IStepListenerChannel) = Step.createListener(name, listeners)
    static member CreatePause(duration) = Step.createPause(duration)
    static member CreateListenerChannel() = Step.createListenerChannel()

type Assertion =    
    static member ForStep (stepName, assertion: Func<AssertStats, bool>) = Assertion.forStep(stepName, assertion.Invoke)
    static member ForScenario (assertion: Func<AssertStats, bool>) = Assertion.forScenario(assertion.Invoke)

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