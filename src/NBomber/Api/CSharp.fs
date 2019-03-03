namespace NBomber.CSharp

open System
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open NBomber
open NBomber.Contracts
open NBomber.FSharp

type ConnectionPool =
    static member Create<'TConnection>(name: string, openConnection: Func<'TConnection>, [<Optional;DefaultParameterValue(null:obj)>] closeConnection: Action<'TConnection>, [<Optional;DefaultParameterValue(Domain.Constants.DefaultConnectionsCount)>] ?connectionsCount: int) = 
        let close = if isNull closeConnection then (new Action<'TConnection>(fun _ -> ()))
                    else closeConnection
        let count = defaultArg connectionsCount Domain.Constants.DefaultConnectionsCount
        FSharp.ConnectionPool.create(name, openConnection.Invoke, close.Invoke, count)    
    
    static member None = FSharp.ConnectionPool.none

type Step =    
    static member Create(name: string, pool: IConnectionPool<'TConnection>, execute: Func<StepContext<'TConnection>,Task<Response>>) = FSharp.Step.create(name, pool, execute.Invoke)        

type Assertion =    
    static member ForStep (stepName, assertion: Func<Statistics, bool>, [<Optional;DefaultParameterValue(null:string)>]label: string) =         
        if isNull label then Assertion.forStep(stepName, assertion.Invoke)
        else Assertion.forStep(stepName, assertion.Invoke, label)

[<Extension>]
type ScenarioBuilder =

    static member CreateScenario(name: string, [<System.ParamArray>]steps: IStep[]) =
        FSharp.Scenario.create name (Seq.toList steps)
    
    [<Extension>]
    static member WithTestInit(scenario: Scenario, initFunc: Func<CancellationToken,Task>) = 
        { scenario with TestInit = Some initFunc.Invoke }

    [<Extension>]
    static member WithTestClean(scenario: Scenario, cleanFunc: Func<CancellationToken,Task>) = 
        { scenario with TestClean = Some cleanFunc.Invoke }

    [<Extension>]
    static member WithAssertions(scenario: Scenario, [<System.ParamArray>]assertions: IAssertion[]) = 
        scenario |> FSharp.Scenario.withAssertions(Seq.toList(assertions))    

    [<Extension>]
    static member WithConcurrentCopies(scenario: Scenario, concurrentCopies: int) = 
        scenario |> FSharp.Scenario.withConcurrentCopies(concurrentCopies)

    [<Extension>]
    static member WithWarmUpDuration(scenario: Scenario, duration: TimeSpan) = 
        scenario |> FSharp.Scenario.withWarmUpDuration(duration)
    
    [<Extension>]
    static member WithDuration(scenario: Scenario, duration: TimeSpan) = 
        scenario |> FSharp.Scenario.withDuration(duration)

[<Extension>]
type NBomberRunner =    
        
    static member RegisterScenarios([<System.ParamArray>]scenarios: Contracts.Scenario[]) = 
        scenarios |> Seq.toList |> FSharp.NBomberRunner.registerScenarios    

    [<Extension>]
    static member LoadConfig(context: NBomberContext, path: string) =
        context |> FSharp.NBomberRunner.loadConfig(path)

    [<Extension>]
    static member WithReportFileName(context: NBomberContext, reportFileName: string) =
        context |> FSharp.NBomberRunner.withReportFileName(reportFileName)

    [<Extension>]
    static member WithReportFormats(context: NBomberContext, [<System.ParamArray>]reportFormats: ReportFormat[]) =
        let formats = reportFormats |> Seq.toList
        context |> FSharp.NBomberRunner.withReportFormats(formats)   

    [<Extension>]
    static member SaveStatisticsTo(context: NBomberContext, statisticsSink: IStatisticsSink) = 
        context |> FSharp.NBomberRunner.saveStatisticsTo(statisticsSink)

    [<Extension>]
    static member Run(context: NBomberContext) =
        FSharp.NBomberRunner.run(context)
        
    [<Extension>]
    static member RunInConsole(context: NBomberContext) =
        FSharp.NBomberRunner.runInConsole(context)

    [<Extension>]
    static member RunTest(context: NBomberContext) =
        FSharp.NBomberRunner.runTest(context)