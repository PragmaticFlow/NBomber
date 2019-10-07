[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Scenario

open System.Threading

open NBomber
open NBomber.Configuration
open NBomber.Errors            

let createCorrelationId (scnName: ScenarioName, concurrentCopies: int) =
    [|0 .. concurrentCopies - 1|] 
    |> Array.map(fun i -> sprintf "%s_%i" scnName i)    

let create (config: Contracts.Scenario) =
    let steps = config.Steps |> Step.cast
    let assertions = config.Assertions |> Assertion.cast

    { ScenarioName = config.ScenarioName
      TestInit = config.TestInit
      TestClean = config.TestClean
      Steps = steps
      Assertions = assertions
      ConcurrentCopies = config.ConcurrentCopies      
      CorrelationIds = createCorrelationId(config.ScenarioName, config.ConcurrentCopies)
      WarmUpDuration = config.WarmUpDuration
      Duration = config.Duration }

let init (scenario: Scenario, initAllConnectionPools: Scenario -> ConnectionPool<obj>[],
          customSettings: string, nodeType: Contracts.NodeType) =
    try     
        // todo: refactor, pass token
        if scenario.TestInit.IsSome then
            let cancelToken = new CancellationTokenSource()
            let context = {
                Contracts.NodeType = nodeType
                Contracts.ScenarioContext.CustomSettings = customSettings
                Contracts.ScenarioContext.CancellationToken = cancelToken.Token
            }
            scenario.TestInit.Value(context).Wait()        

        let allPools = initAllConnectionPools(scenario)
        let steps = scenario.Steps |> Array.map(ConnectionPool.setConnectionPool(allPools))
        Ok { scenario with Steps = steps }
    with 
    | ex -> Error <| InitScenarioError ex

let clean (scenario: Scenario, nodeType: Contracts.NodeType, logger: Serilog.ILogger) =

    try
        if scenario.TestClean.IsSome then
            // todo: refacto, pass token
            let cancelToken = new CancellationTokenSource()
            let context = {
                Contracts.NodeType = nodeType
                Contracts.ScenarioContext.CustomSettings = ""
                Contracts.ScenarioContext.CancellationToken = cancelToken.Token
            }
            scenario.TestClean.Value(context).Wait()
        
        ConnectionPool.clean(scenario, logger)
    with
    | ex -> Serilog.Log.Error(ex, "TestClean")

let filterTargetScenarios (targetScenarios: string[]) (allScenarios: Scenario[]) =
    match targetScenarios with
    | [||] -> allScenarios
    | _    ->
        allScenarios 
        |> Array.filter(fun x -> targetScenarios |> Array.exists(fun target -> x.ScenarioName = target))
        
let applySettings (settings: ScenarioSetting[]) (scenarios: Scenario[]) =        
    
    let updateScenario (scenario: Scenario, settings: ScenarioSetting) =        
        { scenario with ConcurrentCopies = settings.ConcurrentCopies                        
                        CorrelationIds = createCorrelationId(scenario.ScenarioName, settings.ConcurrentCopies)
                        WarmUpDuration = settings.WarmUpDuration.TimeOfDay
                        Duration = settings.Duration.TimeOfDay }

    scenarios
    |> Array.map(fun scn -> 
        settings
        |> Array.tryPick(fun x -> 
            if x.ScenarioName = scn.ScenarioName then Some(scn, x)
            else None)
        |> Option.map(updateScenario)
        |> Option.defaultValue(scn))