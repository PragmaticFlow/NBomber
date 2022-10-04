[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Scenario

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Text

open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Contracts
open NBomber.Configuration
open NBomber.Extensions.Internal
open NBomber.Extensions.Operator.Result
open NBomber.Errors
open NBomber.Domain.DomainTypes

module Validation =

    let checkEmptyScenarioName (scenario: Contracts.Scenario) =
        if String.IsNullOrWhiteSpace scenario.ScenarioName then AppError.createResult EmptyScenarioName
        else Ok scenario

    let checkDuplicateScenarioName (scenarios: Contracts.Scenario list) =
        let duplicates = scenarios |> Seq.map(fun x -> x.ScenarioName) |> String.filterDuplicates |> Seq.toList
        if duplicates.Length > 0 then AppError.createResult(DuplicateScenarioName duplicates)
        else Ok scenarios

    let checkMultipleClientFactoryAssign (regScenarios: Contracts.Scenario list) =
        let multipleAssignFactories =
            regScenarios
            |> Seq.map(fun scn ->
                scn.Steps
                |> Seq.map id
                |> Seq.cast<Step>
                |> Seq.choose(fun x -> x.ClientFactory)
                |> Seq.distinct
                |> Seq.map(fun x -> x.FactoryName)
                |> Seq.toList
            )
            |> Seq.collect id
            |> String.filterDuplicates
            |> Seq.toList

        if multipleAssignFactories.Length > 0 then AppError.createResult(MultipleClientFactoryAssign multipleAssignFactories)
        else Ok regScenarios

    let checkDuplicateClientFactories (regScenarios: Contracts.Scenario list) =
        regScenarios
        |> Seq.collect(fun x -> x.Steps)
        |> Seq.cast<Step>
        |> Seq.choose(fun x -> x.ClientFactory)
        |> Seq.distinct
        |> Seq.groupBy(fun x -> x.FactoryName)
        |> Seq.choose(fun (name,factories) -> if Seq.length factories > 1 then Some name else None)
        |> Seq.toList
        |> function
            | [] -> Ok regScenarios
            | factoryName::tail -> AppError.createResult(DuplicateClientFactoryName factoryName)

    let checkInitOnlyScenario (scenario: Contracts.Scenario) =
        if List.isEmpty scenario.Steps then
            // for init only scenario we can have scenario with 0 steps
            if scenario.Init.IsSome || scenario.Clean.IsSome then Ok scenario
            else AppError.createResult(EmptySteps scenario.ScenarioName)
        else Ok scenario

    let checkEmptyStepName (scenario: Contracts.Scenario) =
        let emptyStepExist = scenario.Steps |> List.exists(fun x -> String.IsNullOrWhiteSpace x.StepName)
        if emptyStepExist then AppError.createResult(EmptyStepName scenario.ScenarioName)
        else Ok scenario

    let checkDuplicateStepNameButDiffImpl (scenario: Contracts.Scenario) =
        scenario.Steps
        |> List.distinct
        |> List.groupBy(fun x -> x.StepName)
        |> List.choose(fun (stName,steps) -> if List.length steps > 1 then Some stName else None)
        |> function
            | [] -> Ok scenario
            | stName::tail -> AppError.createResult(DuplicateStepNameButDiffImpl(scenario.ScenarioName, stName))

    let checkClientFactoryName (scenario: Contracts.Scenario) =
        scenario.Steps
        |> Seq.cast<Step>
        |> Seq.choose(fun x -> x.ClientFactory)
        |> Seq.distinct
        |> Seq.map(fun x -> ClientFactory.checkName x.FactoryName)
        |> Result.sequence
        |> function
            | Ok _ -> Ok scenario
            | Error errors -> errors |> List.head |> AppError.createResult

    let validate =
        checkEmptyScenarioName
        >=> checkInitOnlyScenario
        >=> checkEmptyStepName
        >=> checkDuplicateStepNameButDiffImpl
        >=> checkClientFactoryName

module ClientFactory =

    let updateName (scenarioName: string) (steps: IStep list) =
        steps
        |> Seq.cast<Step>
        |> Seq.map(fun step ->
            match step.ClientFactory with
            | Some factory ->
                let factoryName = ClientFactory.createFullName factory.FactoryName scenarioName
                factory.SetName factoryName
                step

            | None -> step
        )
        |> Seq.toList

    let filterDistinct (scenarios: Scenario list) =
        scenarios
        |> List.collect(fun x -> x.Steps)
        |> List.choose(fun x -> x.ClientFactory)
        |> List.distinctBy(fun x -> x.FactoryName)

    let applySettings (settings: ClientFactorySetting list) (factories: ClientFactory<obj> list) =
        factories
        |> List.map(fun factory ->
            let setting = settings |> List.tryFind(fun stn -> stn.FactoryName = factory.FactoryName)
            match setting with
            | Some v ->
                factory.SetClientCount v.ClientCount
                factory

            | None -> factory
        )

module Feed =

    let filterDistinctFeeds (scenarios: Scenario list) =
        scenarios
        |> List.collect(fun x -> x.Steps)
        |> List.choose(fun x -> x.Feed)
        |> List.distinctBy id

module ScenarioContext =

    let create (context: IBaseContext) = {
        new IScenarioContext with
            member _.TestInfo = context.TestInfo
            member _.NodeInfo = context.GetNodeInfo()
            member _.CustomSettings = ConfigurationBuilder().Build() :> IConfiguration
            member _.CancellationToken = context.CancellationToken
            member _.Logger = context.Logger
    }

    let setCustomSettings (context: IScenarioContext) (customSettings: string) =

        let parseCustomSettings (settings: string) =
            try
                let stream = new MemoryStream(settings |> Encoding.UTF8.GetBytes)
                ConfigurationBuilder().AddJsonStream(stream).Build() :> IConfiguration
            with
            | _ -> ConfigurationBuilder().Build() :> IConfiguration

        { new IScenarioContext with
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

let createStepOrderIndex (scenario: Contracts.Scenario) =
    if List.isEmpty scenario.Steps then Dictionary<_,_>() // it's needed for case when scenario only init
    else scenario.Steps |> List.distinct |> List.mapi(fun i x -> x.StepName, i) |> Dict.ofSeq

let createDefaultStepOrder (stepOrderIndex: Dictionary<string,int>) (scenario: Contracts.Scenario) =
    seq {
        for s in scenario.Steps do
            yield stepOrderIndex[s.StepName] }
    |> Seq.toArray

let getStepOrder (scenario: Scenario) =
    scenario.DefaultStepOrder

let createScenario (scn: Contracts.Scenario) = result {
    let! timeline = scn.LoadSimulations |> LoadTimeLine.createWithDuration
    let! scenario = Validation.validate scn
    let stepOrderIndex = createStepOrderIndex scenario
    let defaultStepOrder = scenario |> createDefaultStepOrder stepOrderIndex

    let steps =
        scenario.Steps
        |> List.distinct
        |> ClientFactory.updateName scenario.ScenarioName

    return { ScenarioName = scenario.ScenarioName
             Init = scenario.Init
             Clean = scenario.Clean
             Steps = steps
             LoadTimeLine = timeline.LoadTimeLine
             WarmUpDuration = scenario.WarmUpDuration
             PlanedDuration = timeline.ScenarioDuration
             ExecutedDuration = None
             CustomSettings = String.Empty
             DefaultStepOrder = defaultStepOrder
             StepOrderIndex = stepOrderIndex
             CustomStepOrder = scenario.CustomStepOrder
             StepInterception = scenario.StepInterception
             IsEnabled = true
             IsInitialized = false }
}

let createScenarios (scenarios: Contracts.Scenario list) = result {

    let validate =
        Validation.checkDuplicateScenarioName
        >=> Validation.checkDuplicateClientFactories
        >=> Validation.checkMultipleClientFactoryAssign

    let! vScns = validate scenarios

    return! vScns
            |> List.map createScenario
            |> Result.sequence
            |> Result.mapError List.head
}

let filterTargetScenarios (targetScenarios: string list) (scenarios: Scenario list) =
    scenarios
    |> List.filter(fun x -> targetScenarios |> Seq.exists(fun target -> x.ScenarioName = target))

let applySettings (settings: ScenarioSetting list) (defaultStepTimeout: TimeSpan) (scenarios: Scenario list) =

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
                        CustomSettings = settings.CustomSettings |> Option.defaultValue ""
                        CustomStepOrder = settings.CustomStepOrder |> Option.map(fun x -> fun () -> x) }

    let updateStepTimeout (defaultStepTimeout: TimeSpan) (step: Step) =
        if step.Timeout = TimeSpan.Zero then
            { step with Timeout = defaultStepTimeout }
        else
            step

    scenarios
    |> List.map(fun scn ->
        settings
        |> List.tryPick(fun x ->
            if x.ScenarioName = scn.ScenarioName then Some(scn, x)
            else None
        )
        |> Option.map updateScenario
        |> Option.defaultValue scn)
    |> List.map(fun scn -> { scn with Steps = scn.Steps |> List.map(updateStepTimeout defaultStepTimeout) })

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
