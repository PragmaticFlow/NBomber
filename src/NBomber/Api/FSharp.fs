﻿namespace NBomber.FSharp

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

    let createPull (name: string, pool: IConnectionPool<'TConnection>, execute: PullContext<'TConnection> -> Task<Response>) =                
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
            fun (context: PullContext<obj>) ->                 
                let newContext = { CorrelationId = context.CorrelationId
                                   Connection = context.Connection :?> 'TConnection
                                   Payload = context.Payload }
                execute(newContext)

        Pull({ StepName = name; ConnectionPool = newPool; Execute = newExecute; CurrentContext = None }) :> IStep
    
    let createPush (name: string, pool: IConnectionPool<'TConnection>, handler: PushContext<'TConnection> -> Task) =
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
        let newHandler = 
            fun (context: PushContext<obj>) ->                 
                let newContext = { CorrelationId = context.CorrelationId
                                   Connection = context.Connection :?> 'TConnection
                                   UpdatesChannel = context.UpdatesChannel }
                handler(newContext)

        Push({ StepName = name; ConnectionPool = newPool; Handler = newHandler; CurrentContext = None }) :> IStep

    let createPause (duration) = Pause(duration) :> IStep    

module Assertion =
    open NBomber.Domain.DomainTypes

    let forStep (stepName, assertion: AssertStats -> bool) = 
        Step({ StepName = stepName; ScenarioName = ""; AssertFunc = assertion }) :> IAssertion

module Scenario =        
    
    let create (name: string, steps: IStep list): Contracts.Scenario =
        { ScenarioName = name
          TestInit = None
          Steps = Seq.toArray(steps)
          ConcurrentCopies = Constants.DefaultConcurrentCopies
          Duration = TimeSpan.FromSeconds(Constants.DefaultScenarioDurationInSec)
          Assertions = Array.empty }

    let withTestInit (initFunc: unit -> unit) (scenario: Contracts.Scenario) =                
        { scenario with TestInit = Some initFunc }

    let withAssertions (assertions: IAssertion list) (scenario: Contracts.Scenario) =        
        let asrts = assertions
                    |> Seq.cast<Assertion>
                    |> Seq.map(function | Step x -> Step({ x with ScenarioName = scenario.ScenarioName}))
                    |> Seq.map(fun x -> x :> IAssertion)
                    |> Seq.toArray

        { scenario with Assertions = asrts }

    let withConcurrentCopies (concurrentCopies: int) (scenario: Contracts.Scenario) =
        { scenario with ConcurrentCopies = concurrentCopies }

    let withDuration (duration: TimeSpan) (scenario: Contracts.Scenario) =        
        { scenario with Duration = duration }

module NBomberRunner = 
    open System.IO
    open Serilog
    open NBomber.Configuration    
    open NBomber.Infra        

    let registerScenario (scenario: Contracts.Scenario) = 
        { Scenarios = [|scenario|]; NBomberConfig = None }

    let registerScenarios (scenarios: Contracts.Scenario list) = 
        { Scenarios = Seq.toArray(scenarios); NBomberConfig = None }

    let loadConfig (path: string) (context: NBomberRunnerContext) =
        let config = path |> File.ReadAllText |> NBomberConfig.parse
        { context with NBomberConfig = config }

    let run (context: NBomberRunnerContext) =
        let dep = Dependency.create(Process)
        NBomberRunner.run(dep, context)

    let runInConsole (context: NBomberRunnerContext) =
        let mutable run = true
        while run do
            let dep = Dependency.create(Console)
            NBomberRunner.run(dep, context)
            Log.Information("Repeat the same test one more time? (y/n)")
        
            let userInput = Console.ReadLine()
            run <- List.contains userInput ["y"; "Y"; "yes"; "Yes"]

    let runTest (context: NBomberRunnerContext) =
        let dep = Dependency.create(Test)
        NBomberRunner.run(dep, context)