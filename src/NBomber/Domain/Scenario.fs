[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Scenario

open NBomber
open NBomber.Extensions
open NBomber.Configuration
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

let filterTargetScenarios (targetScenarios: string[]) (allScenarios: Scenario[]) =
    match targetScenarios with
    | [||] -> allScenarios
    | _    ->
        allScenarios
        |> Array.filter(fun x -> targetScenarios |> Array.exists(fun target -> x.ScenarioName = target))

let applySettings (settings: ScenarioSetting[]) (scenarios: Scenario[]) =

    let updateScenario (scenario: Scenario, settings: ScenarioSetting) =

        let timeLine =
            settings.LoadSimulationsSettings
            |> List.map(LoadTimeLine.createSimulationFromSettings)
            |> LoadTimeLine.unsafeCreateWithDuration

        { scenario with LoadTimeLine = timeLine.LoadTimeLine
                        WarmUpDuration = settings.WarmUpDuration.TimeOfDay
                        Duration = timeLine.ScenarioDuration }

    scenarios
    |> Array.map(fun scn ->
        settings
        |> Array.tryPick(fun x ->
            if x.ScenarioName = scn.ScenarioName then Some(scn, x)
            else None)
        |> Option.map updateScenario
        |> Option.defaultValue scn)

let filterDistinctConnectionPools (scenarios: Scenario seq) =
    scenarios
    |> Seq.collect(fun x -> x.Steps)
    |> Seq.choose(fun x -> if x.ConnectionPool.ConnectionCount > 0 then Some x.ConnectionPool else None)
    |> Seq.distinct
