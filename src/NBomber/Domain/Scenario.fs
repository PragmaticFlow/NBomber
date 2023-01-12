[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Scenario

open System
open System.IO
open System.Text
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Configuration
open NBomber.Extensions.Internal
open NBomber.Extensions.Operator.Result
open NBomber.Errors
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.Domain.ScenarioContext

module Validation =

    let checkEmptyScenarioName (scenario: ScenarioProps) =
        if String.IsNullOrWhiteSpace scenario.ScenarioName then Error EmptyScenarioName
        else Ok scenario

    let checkDuplicateScenarioName (scenarios: ScenarioProps list) =
        let duplicates = scenarios |> Seq.map(fun x -> x.ScenarioName) |> String.filterDuplicates |> Seq.toList
        if duplicates.Length > 0 then Error (DuplicateScenarioName duplicates)
        else Ok scenarios

    let checkInitOnlyScenario (scenario: ScenarioProps) =
        if scenario.Run.IsNone then
            if scenario.Init.IsSome || scenario.Clean.IsSome then Ok scenario
            else Error (EmptyScenarioWithEmptyInitAndClean scenario.ScenarioName)

        else Ok scenario

    let checkWarmupDuration (scnDuration: TimeSpan) (scenario: ScenarioProps) =
        match scenario.WarmUpDuration with
        | Some warmUpDuration ->
            if scnDuration < warmUpDuration then
                Error (WarmUpDurationIsBiggerScnDuration(scenario.ScenarioName, warmUpDuration, scnDuration))
            else
                Ok scenario

        | None -> Ok scenario

    let validate (scenario: ScenarioProps) (scnDuration: TimeSpan) =
        scenario
        |> checkEmptyScenarioName
        |> Result.bind checkInitOnlyScenario
        |> Result.bind (checkWarmupDuration scnDuration)
        |> Result.mapError AppError.create

module ScenarioInitContext =

    let create (context: IBaseContext) = {
        new IScenarioInitContext with
            member _.TestInfo = context.TestInfo
            member _.NodeInfo = context.GetNodeInfo()
            member _.CustomSettings = ConfigurationBuilder().Build() :> IConfiguration
            member _.Logger = context.Logger
    }

    let setCustomSettings (context: IScenarioInitContext) (customSettings: string) =

        let parseCustomSettings (settings: string) =
            try
                let stream = new MemoryStream(settings |> Encoding.UTF8.GetBytes)
                ConfigurationBuilder().AddJsonStream(stream).Build() :> IConfiguration
            with
            | _ -> ConfigurationBuilder().Build() :> IConfiguration

        { new IScenarioInitContext with
            member _.TestInfo = context.TestInfo
            member _.NodeInfo = context.NodeInfo
            member _.CustomSettings = parseCustomSettings(customSettings)
            member _.Logger = context.Logger }

let createScenarioInfo (scenarioName: string, duration: TimeSpan, threadNumber: int, operation: ScenarioOperation) =
    { ThreadId = $"{scenarioName}_{threadNumber}"
      ThreadNumber = threadNumber
      ScenarioName = scenarioName
      ScenarioDuration = duration
      ScenarioOperation = operation }

let createScenario (scn: ScenarioProps) = result {
    let! simulations   = LoadSimulation.create scn.LoadSimulations
    let planedDuration = LoadSimulation.getPlanedDuration simulations
    let! scnProps      = Validation.validate scn planedDuration

    return { ScenarioName = scnProps.ScenarioName
             Init = scnProps.Init
             Clean = scnProps.Clean
             Run = scnProps.Run
             LoadSimulations = simulations
             WarmUpDuration = scnProps.WarmUpDuration
             PlanedDuration = planedDuration
             ExecutedDuration = None
             CustomSettings = String.Empty
             IsEnabled = true
             IsInitialized = false
             RestartIterationOnFail = scnProps.RestartIterationOnFail
             MaxFailCount = scnProps.MaxFailCount }
}

let createScenarios (scenarios: ScenarioProps list) = result {
    let! vScns =
        Validation.checkDuplicateScenarioName scenarios
        |> Result.mapError AppError.create

    return! vScns
            |> List.map createScenario
            |> Result.sequence
            |> Result.mapError List.head
}

let filterTargetScenarios (targetScenarios: string list) (scenarios: Scenario list) =
    scenarios
    |> List.filter(fun x -> targetScenarios |> Seq.exists(fun target -> x.ScenarioName = target))

let applySettings (settings: ScenarioSetting list) (scenarios: Scenario list) =

    let updateScenario (scenario: Scenario, settings: ScenarioSetting) =

        let simulations =
            match settings.LoadSimulationsSettings with
            | Some simulation ->
                simulation
                |> LoadSimulation.create
                |> Result.getOk

            | None -> scenario.LoadSimulations

        let planedDuration = LoadSimulation.getPlanedDuration simulations

        { scenario with LoadSimulations = simulations
                        WarmUpDuration = settings.WarmUpDuration
                        PlanedDuration = planedDuration
                        CustomSettings = settings.CustomSettings |> Option.defaultValue ""
                        MaxFailCount = settings.MaxFailCount |> Option.defaultValue Constants.ScenarioMaxFailCount }

    scenarios
    |> List.map(fun scn ->
        settings
        |> List.tryPick(fun setting ->
            if setting.ScenarioName = scn.ScenarioName then Some(scn, setting)
            else None
        )
        |> Option.map updateScenario
        |> Option.defaultValue scn
    )

let setExecutedDuration (scenario: Scenario) (executedDuration: TimeSpan) =
    if executedDuration < scenario.PlanedDuration then
        { scenario with ExecutedDuration = Some executedDuration }
    else
        { scenario with ExecutedDuration = Some scenario.PlanedDuration }

let getExecutedDuration (scenario: Scenario) =
    scenario.ExecutedDuration |> Option.defaultValue scenario.PlanedDuration

let defaultClusterCount = fun _ -> 1

let getScenariosForWarmUp (scenarios: Scenario list) =
    scenarios |> List.filter(fun x -> x.Run.IsSome && x.WarmUpDuration.IsSome)

let getScenariosForBombing (scenarios: Scenario list) =
    scenarios |> List.filter(fun x -> x.Run.IsSome)

let getMaxDuration (scenarios: Scenario list) =
    scenarios |> List.map(fun x -> x.PlanedDuration) |> List.max

let getMaxWarmUpDuration (scenarios: Scenario list) =
    scenarios |> List.choose(fun x -> x.WarmUpDuration) |> List.max

let inline measure (name: string) (ctx: ScenarioContext) (run: IScenarioContext -> Task<IResponse>) = backgroundTask {
    let startTime = ctx.Timer.Elapsed.TotalMilliseconds
    try
        let! response = run ctx
        let endTime = ctx.Timer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        let result = { Name = name; ClientResponse = response; EndTimeMs = endTime; LatencyMs = latency }
        ctx.StatsActor.Publish(AddMeasurement result)
    with
    | :? RestartScenarioIteration ->
        let endTime = ctx.Timer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        let error = ResponseInternal.failEmpty
        let result = { Name = name; ClientResponse = error; EndTimeMs = endTime; LatencyMs = latency }
        ctx.StatsActor.Publish(AddMeasurement result)

    | :? OperationCanceledException as ex ->
        let endTime = ctx.Timer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        let response = ResponseInternal.failTimeout
        let result = { Name = name; ClientResponse = response; EndTimeMs = endTime; LatencyMs = latency }
        ctx.StatsActor.Publish(AddMeasurement result)

    | ex ->
        let endTime = ctx.Timer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        let context = ctx :> IScenarioContext
        context.Logger.Error(ex, $"Unhandled exception for Scenario: {0}", context.ScenarioInfo.ScenarioName)

        let response = ResponseInternal.failUnhandled ex
        let result = { Name = name; ClientResponse = response; EndTimeMs = endTime; LatencyMs = latency }
        ctx.StatsActor.Publish(AddMeasurement result)
}
