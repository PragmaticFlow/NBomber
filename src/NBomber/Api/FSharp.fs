namespace NBomber.FSharp

open System
open System.Threading.Tasks

open NBomber
open NBomber.Contracts
open NBomber.DomainServices
open NBomber.Infra.Dependency

type GlobalUpdatesChannel =
    static member Instance = Dependency.GlobalUpdatesChannel :> IGlobalUpdatesChannel

module Step =    
    open NBomber.Domain.DomainTypes

    let createPull (name: string, execute: Request -> Task<Response>) = 
        Pull({ StepName = name; Execute = execute }) :> IStep  
    
    let createPush (name: string) =         
        Push({ StepName = name; UpdatesChannel = Dependency.GlobalUpdatesChannel }) :> IStep

    let createPause (duration) = Pause(duration) :> IStep    

module Assertion =
    open NBomber.Domain.DomainTypes

    let forStep (stepName, assertion: AssertStats -> bool) = 
        Step({ StepName = stepName; ScenarioName = ""; AssertFunc = assertion }) :> IAssertion
    
    let forScenario (assertion: AssertStats -> bool) = 
        Scenario({ ScenarioName = ""; AssertFunc = assertion }) :> IAssertion

module Scenario =    
    open NBomber.Domain.DomainTypes
    
    let create (name: string, steps: IStep list): Contracts.Scenario =
        { ScenarioName = name
          TestInit = None
          Steps = Seq.toArray(steps)
          ConcurrentCopies = Constants.DefaultConcurrentCopies
          Duration = TimeSpan.FromSeconds(Constants.DefaultDurationInSeconds)
          Assertions = Array.empty }

    let withTestInit (initFunc: Request -> Task<Response>) (scenario: Contracts.Scenario) =
        let step = Step.createPull(Domain.DomainTypes.Constants.InitId, initFunc)
        { scenario with TestInit = Some(step) }

    let withAssertions (assertions: IAssertion list) (scenario: Contracts.Scenario) =        
        let asrts = assertions
                    |> Seq.cast<Assertion>
                    |> Seq.map(function | Step x -> Step({ x with ScenarioName = scenario.ScenarioName}) 
                                        | Scenario x -> Scenario({ x with ScenarioName = scenario.ScenarioName}))
                    |> Seq.map(fun x -> x :> IAssertion)
                    |> Seq.toArray

        { scenario with Assertions = asrts }

    let withConcurrentCopies (concurrentCopies: int) (scenario: Contracts.Scenario) =
        { scenario with ConcurrentCopies = concurrentCopies }

    let withDuration (duration: TimeSpan) (scenario: Contracts.Scenario) =
        { scenario with Duration = duration }

module NBomberRunner = 
    open System.IO
    open NBomber.Configuration    
    open NBomber.Infra
    open NBomber.Infra.Dependency
    open Serilog

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