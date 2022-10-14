[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Scenario

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Text
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Configuration
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.Domain.ScenarioContext
open NBomber.Extensions.Internal
open NBomber.Extensions.Operator.Result
open NBomber.Errors
open NBomber.Domain.DomainTypes

module Validation =

    let checkEmptyScenarioName (scenario: Contracts.ScenarioProps) =
        if String.IsNullOrWhiteSpace scenario.ScenarioName then AppError.createResult EmptyScenarioName
        else Ok scenario

    let checkDuplicateScenarioName (scenarios: Contracts.ScenarioProps list) =
        let duplicates = scenarios |> Seq.map(fun x -> x.ScenarioName) |> String.filterDuplicates |> Seq.toList
        if duplicates.Length > 0 then AppError.createResult(DuplicateScenarioName duplicates)
        else Ok scenarios

    // let checkInitOnlyScenario (scenario: Contracts.ScenarioArgs) =
    //     if scenario.Run.IsNone then
    //         // for init only scenario we can have scenario with 0 steps
    //         if scenario.Init.IsSome || scenario.Clean.IsSome then Ok scenario
    //         else AppError.createResult(EmptySteps scenario.ScenarioName)
    //     else Ok scenario

    let validate =
        checkEmptyScenarioName
        // >=> checkInitOnlyScenario

// module Feed =
//
//     let filterDistinctFeeds (scenarios: Scenario list) =
//         scenarios
//         |> List.collect(fun x -> x.Steps)
//         |> List.choose(fun x -> x.Feed)
//         |> List.distinctBy id

module ScenarioInitContext =

    let create (context: IBaseContext) = {
        new IScenarioInitContext with
            member _.TestInfo = context.TestInfo
            member _.NodeInfo = context.GetNodeInfo()
            member _.CustomSettings = ConfigurationBuilder().Build() :> IConfiguration
            member _.CancellationToken = context.CancellationToken
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
            member _.CancellationToken = context.CancellationToken
            member _.Logger = context.Logger }

let createScenarioInfo (scenarioName: string, duration: TimeSpan, threadNumber: int, operation: ScenarioOperation) =
    { ThreadId = $"{scenarioName}_{threadNumber}"
      ThreadNumber = threadNumber
      ScenarioName = scenarioName
      ScenarioDuration = duration
      ScenarioOperation = operation }

let createScenario (scn: ScenarioProps) = result {
    let! timeline = scn.LoadSimulations |> LoadTimeLine.createWithDuration
    let! scenario = Validation.validate scn

    return { ScenarioName = scenario.ScenarioName
             Init = scenario.Init
             Clean = scenario.Clean
             Run = scenario.Run
             LoadTimeLine = timeline.LoadTimeLine
             WarmUpDuration = scenario.WarmUpDuration
             PlanedDuration = timeline.ScenarioDuration
             ExecutedDuration = None
             CustomSettings = String.Empty
             IsEnabled = true
             IsInitialized = false }
}

let createScenarios (scenarios: ScenarioProps list) = result {

    let validate =
        Validation.checkDuplicateScenarioName

    let! vScns = validate scenarios

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

        let timeLine =
            match settings.LoadSimulationsSettings with
            | Some simulation ->
                simulation
                |> LoadTimeLine.createWithDuration
                |> Result.getOk

            | None -> {| LoadTimeLine = scenario.LoadTimeLine; ScenarioDuration = scenario.PlanedDuration |}

        { scenario with LoadTimeLine = timeLine.LoadTimeLine
                        WarmUpDuration = settings.WarmUpDuration
                        PlanedDuration = timeLine.ScenarioDuration
                        CustomSettings = settings.CustomSettings |> Option.defaultValue "" }

    // let updateStepTimeout (defaultStepTimeout: TimeSpan) (step: Step) =
    //     if step.Timeout = TimeSpan.Zero then
    //         { step with Timeout = defaultStepTimeout }
    //     else
    //         step

    scenarios
    |> List.map(fun scn ->
        settings
        |> List.tryPick(fun x ->
            if x.ScenarioName = scn.ScenarioName then Some(scn, x)
            else None
        )
        |> Option.map updateScenario
        |> Option.defaultValue scn
    )
    //|> List.map(fun scn -> { scn with Steps = scn.Steps |> List.map(updateStepTimeout defaultStepTimeout) })

let setExecutedDuration (scenario: Scenario, executedDuration: TimeSpan) =
    if executedDuration < scenario.PlanedDuration then
        { scenario with ExecutedDuration = Some executedDuration }
    else
        { scenario with ExecutedDuration = Some scenario.PlanedDuration }

let getExecutedDuration (scenario: Scenario) =
    scenario.ExecutedDuration |> Option.defaultValue scenario.PlanedDuration

let defaultClusterCount = fun _ -> 1

let getScenariosForWarmUp (scenarios: Scenario list) =
    scenarios |> List.filter(fun x -> x.WarmUpDuration.IsSome)

let getMaxDuration (scenarios: Scenario list) =
    scenarios |> List.map(fun x -> x.PlanedDuration) |> List.max

let getMaxWarmUpDuration (scenarios: Scenario list) =
    scenarios |> List.choose(fun x -> x.WarmUpDuration) |> List.max

let measure (name: string) (ctx: ScenarioContext) (run: IScenarioContext -> Task<Response<obj>>) = backgroundTask {
    let startTime = ctx.Timer.Elapsed.TotalMilliseconds
    try
        let! response = run ctx
        let endTime = ctx.Timer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        let result = { StepName = name; ClientResponse = response; EndTimeMs = endTime; LatencyMs = latency }
        ctx.StatsActor.Publish(AddStepResult result)
    with
    | ex ->
        let endTime = ctx.Timer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        let context = ctx :> IScenarioContext
        context.Logger.Fatal(ex, $"Unhandled exception for Scenario: {0}", context.ScenarioInfo.ScenarioName)

        let error = ResponseInternal.fail(ex)
        let result = { StepName = name; ClientResponse = error; EndTimeMs = endTime; LatencyMs = latency }
        ctx.StatsActor.Publish(AddStepResult result)
}
