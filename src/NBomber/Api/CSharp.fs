namespace NBomber.CSharp

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open NBomber
open NBomber.Contracts
open NBomber.FSharp

type ConnectionPool =
    static member Create<'TConnection>(name: string, openConnection: Func<'TConnection>, [<Optional;DefaultParameterValue(null:obj)>] closeConnection: Action<'TConnection>, [<Optional;DefaultParameterValue(Domain.DomainTypes.Constants.DefaultConnectionsCount)>] ?connectionsCount: int) = 
        let close = if isNull closeConnection then (new Action<'TConnection>(fun _ -> ()))
                    else closeConnection
        let count = defaultArg connectionsCount Domain.DomainTypes.Constants.DefaultConnectionsCount
        FSharp.ConnectionPool.create(name, openConnection.Invoke, close.Invoke, count)    
    
    static member None = FSharp.ConnectionPool.none

type Step =    
    static member CreatePull(name: string, pool: IConnectionPool<'TConnection>, execute: Func<PullContext<'TConnection>,Task<Response>>) = FSharp.Step.createPull(name, pool, execute.Invoke)
    static member CreatePush(name: string, pool: IConnectionPool<'TConnection>, handler: Func<PushContext<'TConnection>,Task>) = FSharp.Step.createPush(name, pool, handler.Invoke)
    static member CreatePause(duration) = FSharp.Step.createPause(duration)

type Assertion =    
    static member ForStep (stepName, assertion: Func<AssertStats, bool>) = Assertion.forStep(stepName, assertion.Invoke)    

[<Extension>]
type ScenarioBuilder =

    static member CreateScenario(name: string, [<System.ParamArray>]steps: IStep[]) =
        FSharp.Scenario.create(name, Seq.toList(steps))
    
    [<Extension>]
    static member WithTestInit(scenario: Scenario, initFunc: Action) = 
        scenario |> FSharp.Scenario.withTestInit(initFunc.Invoke)

    [<Extension>]
    static member WithTestClean(scenario: Scenario, cleanFunc: Action) = 
        scenario |> FSharp.Scenario.withTestClean(cleanFunc.Invoke)

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