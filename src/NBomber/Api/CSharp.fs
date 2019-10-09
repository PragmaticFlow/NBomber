namespace NBomber.CSharp

#nowarn "3211"

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Configuration

type ConnectionPool =
    static member Create<'TConnection>(name: string, openConnection: Func<'TConnection>, [<Optional;DefaultParameterValue(null:obj)>] closeConnection: Action<'TConnection>, [<Optional;DefaultParameterValue(0:int)>] connectionsCount: int) = 
        let close = if isNull closeConnection then (new Action<'TConnection>(ignore))
                    else closeConnection
        
        if connectionsCount = 0 then
            FSharp.ConnectionPool.create(name, openConnection.Invoke, close.Invoke)            
        else 
            FSharp.ConnectionPool.create(name, openConnection.Invoke, close.Invoke, connectionsCount)
    
    static member None = FSharp.ConnectionPool.none

type Step =    
    static member Create(name: string, 
                         execute: Func<StepContext<'TConnection>,Task<Response>>,
                         pool: IConnectionPool<'TConnection>,
                         [<Optional;DefaultParameterValue(Domain.Constants.DefaultRepeatCount:int)>]repeatCount: int,
                         [<Optional;DefaultParameterValue(Domain.Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) = 
        FSharp.Step.create(name, pool, execute.Invoke, repeatCount, doNotTrack)

    static member Create(name: string,
                         execute: Func<StepContext<unit>,Task<Response>>,
                         [<Optional;DefaultParameterValue(Domain.Constants.DefaultRepeatCount:int)>]repeatCount: int,
                         [<Optional;DefaultParameterValue(Domain.Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =
        Step.Create(name, execute, ConnectionPool.None, repeatCount, doNotTrack)

    static member CreatePause(duration: TimeSpan) =
        FSharp.Step.createPause(duration)

type Assertion =    
    static member ForStep(stepName, assertion: Func<Statistics, bool>, [<Optional;DefaultParameterValue(null:string)>]label: string) =         
        if isNull label then Assertion.forStep(stepName, assertion.Invoke)
        else Assertion.forStep(stepName, assertion.Invoke, label)

[<Extension>]
type ScenarioBuilder =
    
    /// Creates scenario with steps which will be executed sequentially.
    static member CreateScenario(name: string, [<System.ParamArray>]steps: IStep[]) =
        FSharp.Scenario.create name (Seq.toList steps)
    
    [<Extension>]
    static member WithTestInit(scenario: Scenario, initFunc: Func<ScenarioContext,Task>) = 
        { scenario with TestInit = Some initFunc.Invoke }

    [<Extension>]
    static member WithTestClean(scenario: Scenario, cleanFunc: Func<ScenarioContext,Task>) = 
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
    static member WithOutWarmUp(scenario: Scenario) = 
        scenario |> FSharp.Scenario.withOutWarmUp
    
    [<Extension>]
    static member WithDuration(scenario: Scenario, duration: TimeSpan) = 
        scenario |> FSharp.Scenario.withDuration(duration)

[<Extension>]
type NBomberRunner =
    
    /// Registers scenarios in NBomber environment. Scenarios will be run in parallel.
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