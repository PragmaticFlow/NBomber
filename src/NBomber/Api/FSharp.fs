namespace NBomber.FSharp

open System
open System.Threading.Tasks

open NBomber
open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Infra.Dependency
open NBomber.DomainServices

type ConnectionPool =   

    static member create<'TConnection>(name: string, openConnection: unit -> 'TConnection, ?closeConnection: 'TConnection -> unit, ?connectionsCount: int) =        
        let count = defaultArg connectionsCount Constants.DefaultConnectionsCount        
        { PoolName = name; OpenConnection = openConnection; CloseConnection = closeConnection
          ConnectionsCount = count; AliveConnections = Array.empty }
          :> IConnectionPool<'TConnection>

    static member none =
        { PoolName = "empty_pool"; OpenConnection = (fun _ -> ());
          CloseConnection = None; ConnectionsCount = 1; AliveConnections = Array.empty }
          :> IConnectionPool<unit>

module Step =        

    let createAction (name: string, pool: IConnectionPool<'TConnection>, execute: StepContext<'TConnection> -> Task<Response>) =                
        let p = pool :?> ConnectionPool<'TConnection>
        
        let newOpen = fun () -> p.OpenConnection() :> obj

        let newClose = match p.CloseConnection with
                       | Some func -> Some <| fun (c: obj) -> c :?> 'TConnection |> func
                       | None      -> None
        
        let newPool = { PoolName = p.PoolName
                        OpenConnection = newOpen
                        CloseConnection = newClose
                        ConnectionsCount = p.ConnectionsCount
                        AliveConnections = Array.empty }        
        
        let newExecute = 
            fun (context: StepContext<obj>) ->                 
                let newContext = { CorrelationId = context.CorrelationId
                                   CancellationToken = context.CancellationToken
                                   Connection = context.Connection :?> 'TConnection
                                   Payload = context.Payload }
                execute(newContext)

        Action({ StepName = name; ConnectionPool = newPool; Execute = newExecute; CurrentContext = None }) :> IStep

    let createPause (duration) = Pause(duration) :> IStep    

type Assertion =

    static member forStep (stepName, assertion: Statistics -> bool, ?label: string) =         
        Domain.DomainTypes.Assertion.Step({ StepName = stepName; ScenarioName = ""; AssertFunc = assertion; Label = label }) :> IAssertion

module Scenario =        
    
    let create (name: string, steps: IStep list): Contracts.Scenario =
        { ScenarioName = name
          TestInit = None
          TestClean = None
          Steps = List.toArray(steps)
          Assertions = Array.empty          
          ConcurrentCopies = Constants.DefaultConcurrentCopies
          WarmUpDuration = TimeSpan.FromSeconds(Constants.DefaultWarmUpDurationInSec)
          Duration = TimeSpan.FromSeconds(Constants.DefaultScenarioDurationInSec) }

    let withTestInit (initFunc: unit -> unit) (scenario: Contracts.Scenario) =                
        { scenario with TestInit = Some initFunc }

    let withTestClean (cleanFunc: unit -> unit) (scenario: Contracts.Scenario) =
        { scenario with TestClean = Some cleanFunc }

    let withAssertions (assertions: IAssertion list) (scenario: Contracts.Scenario) =        
        let asrts = assertions
                    |> Seq.cast<Domain.DomainTypes.Assertion>
                    |> Seq.map(function | Step x -> Step({ x with ScenarioName = scenario.ScenarioName}))
                    |> Seq.map(fun x -> x :> IAssertion)
                    |> Seq.toArray

        { scenario with Assertions = asrts }    

    let withConcurrentCopies (concurrentCopies: int) (scenario: Contracts.Scenario) =
        { scenario with ConcurrentCopies = concurrentCopies }

    let withWarmUpDuration (duration: TimeSpan) (scenario: Contracts.Scenario) =        
        { scenario with WarmUpDuration = duration }    

    let withDuration (duration: TimeSpan) (scenario: Contracts.Scenario) =        
        { scenario with Duration = duration }

module NBomberRunner = 
    open System.IO
    open Serilog
    open NBomber.Configuration    
    open NBomber.Infra        

    let registerScenarios (scenarios: Contracts.Scenario list) = 
        { Scenarios = List.toArray(scenarios)
          NBomberConfig = None
          ReportFileName = None
          ReportFormats = [|ReportFormat.Txt; ReportFormat.Html; ReportFormat.Csv; ReportFormat.Md|]
          StatisticsSink = None }

    let registerScenario (scenario: Contracts.Scenario) = 
        registerScenarios([scenario])

    let withReportFileName (reportFileName: string) (context: NBomberContext) =
        { context with ReportFileName = Some reportFileName }

    let withReportFormats (reportFormats: ReportFormat list) (context: NBomberContext) =
        let formats = reportFormats |> List.toArray
        { context with ReportFormats = formats }    

    let loadConfig (path: string) (context: NBomberContext) =
        let config = path |> File.ReadAllText |> NBomberConfig.parse
        { context with NBomberConfig = config }

    let saveStatisticsTo (statisticsSink: IStatisticsSink) (context: NBomberContext) =
        { context with StatisticsSink = Some statisticsSink }

    let run (context: NBomberContext) =
        let nodeType = NBomberContext.getNodeType(context)
        let dep = Dependency.create(Process, nodeType)
        NBomberRunner.run(dep, context)

    let runInConsole (context: NBomberContext) =
        let mutable run = true
        while run do
            let nodeType = NBomberContext.getNodeType(context)
            let dep = Dependency.create(Console, nodeType)
            NBomberRunner.run(dep, context)
            Log.Information("Repeat the same test one more time? (y/n)")
        
            let userInput = Console.ReadLine()
            run <- List.contains userInput ["y"; "Y"; "yes"; "Yes"]

    let runTest (context: NBomberContext) =
        let nodeType = NBomberContext.getNodeType(context)
        let dep = Dependency.create(Test, nodeType)
        NBomberRunner.run(dep, context)