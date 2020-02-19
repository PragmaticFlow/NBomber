[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Scenario

open System
open System.Threading

open Serilog

open NBomber
open NBomber.Extensions
open NBomber.Configuration
open NBomber.Errors
open NBomber.Domain.Concurrency

let create (config: Contracts.Scenario) =

    let timeLine = LoadSimulation.createTimeLine(TimeSpan.Zero, config.LoadSimulations) |> Result.getOk
    let duration = timeLine |> List.last |> fst

    { ScenarioName = config.ScenarioName
      TestInit = config.TestInit
      TestClean = config.TestClean
      Steps = config.Steps |> Seq.cast<Step> |> Seq.toArray
      LoadTimeLine = timeLine
      WarmUpDuration = config.WarmUpDuration
      Duration = duration }

let init (scenario: Scenario,
          initAllConnectionPools: Scenario -> ConnectionPool<obj>[],
          customSettings: string,
          nodeType: Contracts.NodeType,
          logger: ILogger) =
    try
        // todo: refactor, pass token
        if scenario.TestInit.IsSome then
            let cancelToken = new CancellationTokenSource()
            let context = {
                Contracts.NodeType = nodeType
                Contracts.ScenarioContext.CustomSettings = customSettings
                Contracts.ScenarioContext.CancellationToken = cancelToken.Token
                Contracts.ScenarioContext.Logger = logger
            }
            scenario.TestInit.Value(context).Wait()

        let allPools = initAllConnectionPools(scenario)
        let steps = scenario.Steps |> Array.map(ConnectionPool.setConnectionPool(allPools))
        Ok { scenario with Steps = steps }
    with
    | ex -> Error <| InitScenarioError ex

let clean (scenario: Scenario, nodeType: Contracts.NodeType, logger: ILogger, customSettings: string) =

    try
        if scenario.TestClean.IsSome then
            // todo: refacto, pass token
            let cancelToken = new CancellationTokenSource()
            let context = {
                Contracts.NodeType = nodeType
                Contracts.ScenarioContext.CustomSettings = customSettings
                Contracts.ScenarioContext.CancellationToken = cancelToken.Token
                Contracts.ScenarioContext.Logger = logger
            }
            scenario.TestClean.Value(context).Wait()

        ConnectionPool.clean(scenario, logger)
    with
    | ex -> logger.Error(ex, "TestClean")

let filterTargetScenarios (targetScenarios: string[]) (allScenarios: Scenario[]) =
    match targetScenarios with
    | [||] -> allScenarios
    | _    ->
        allScenarios
        |> Array.filter(fun x -> targetScenarios |> Array.exists(fun target -> x.ScenarioName = target))

let applySettings (settings: ScenarioSetting[]) (scenarios: Scenario[]) =

    let updateScenario (scenario: Scenario, settings: ScenarioSetting) =
        { scenario with LoadTimeLine = List.empty
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
