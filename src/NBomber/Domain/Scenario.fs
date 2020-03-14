[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Scenario

open NBomber
open NBomber.Extensions
open NBomber.Configuration
open NBomber.Domain.ConnectionPool
open NBomber.Domain.Concurrency

let createCorrelationId (scnName: ScenarioName, copyNumber): Contracts.CorrelationId =
    { Id = sprintf "%s_%i" scnName copyNumber
      ScenarioName = scnName
      CopyNumber = copyNumber }

let create (config: Contracts.Scenario) =

    let timeline = config.LoadSimulations |> Array.toList |> LoadTimeLine.unsafeCreateWithDuration

    { ScenarioName = config.ScenarioName
      TestInit = config.TestInit
      TestClean = config.TestClean
      Steps = config.Steps |> Seq.cast<Step> |> Seq.toArray
      LoadTimeLine = timeline.LoadTimeLine
      WarmUpDuration = config.WarmUpDuration
      Duration = timeline.ScenarioDuration }

let filterTargetScenarios (targetScenarios: string[]) (allScenarios: Scenario list) =
    match targetScenarios with
    | [||] -> allScenarios
    | _    ->
        allScenarios
        |> List.filter(fun x -> targetScenarios |> Seq.exists(fun target -> x.ScenarioName = target))

let applySettings (settings: ScenarioSetting[]) (scenarios: Scenario list) =

    let updateScenario (scenario: Scenario, settings: ScenarioSetting) =

        let timeLine =
            settings.LoadSimulationsSettings
            |> List.map(LoadTimeLine.createSimulationFromSettings)
            |> LoadTimeLine.unsafeCreateWithDuration

        { scenario with LoadTimeLine = timeLine.LoadTimeLine
                        WarmUpDuration = settings.WarmUpDuration.TimeOfDay
                        Duration = timeLine.ScenarioDuration }

    scenarios
    |> Seq.map(fun scn ->
        settings
        |> Seq.tryPick(fun x ->
            if x.ScenarioName = scn.ScenarioName then Some(scn, x)
            else None)
        |> Option.map updateScenario
        |> Option.defaultValue scn)
    |> Seq.toList

let applyConnectionPoolSettings (settings: ConnectionPoolSetting list) (poolArgs: Contracts.ConnectionPoolArgs<obj> list) =
    poolArgs |> List.map(fun pool ->
        let setting = settings |> List.tryFind(fun x -> x.PoolName = pool.PoolName)
        match setting with
        | Some v -> { pool with ConnectionCount = v.ConnectionCount }
        | None   -> pool
    )

let filterDistinctConnectionPoolsArgs (scenarios: Scenario list) =
    scenarios
    |> Seq.collect(fun x -> x.Steps)
    |> Seq.choose(fun x -> if x.ConnectionPoolArgs.ConnectionCount > 0 then Some x.ConnectionPoolArgs else None)
    |> Seq.distinct
    |> Seq.toList

let filterDistinctConnectionPools (scenarios: Scenario list) =
    scenarios
    |> Seq.collect(fun x -> x.Steps)
    |> Seq.choose(fun x -> if x.ConnectionPool.IsSome then Some x.ConnectionPool.Value else None)
    |> Seq.distinct
    |> Seq.toList

let insertConnectionPools (pools: ConnectionPool list) (scenarios: Scenario list) =

    scenarios |> List.map(fun scn ->

        scn.Steps |> Array.map(fun step ->
            let pool = pools |> List.tryFind(fun x -> x.PoolName = step.ConnectionPoolArgs.PoolName)
            match pool with
            | Some v -> { step with ConnectionPool = Some v }
            | None   -> step
        )
        |> fun updatedSteps -> { scn with Steps = updatedSteps }
    )
