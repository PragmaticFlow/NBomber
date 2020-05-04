[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Scenario

open System

open FsToolkit.ErrorHandling

open NBomber
open NBomber.Extensions
open NBomber.Extensions.Operator.Result
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain.DomainTypes
open NBomber.Domain.ConnectionPool

module Validation =

    let checkEmptyScenarioName (scenario: Contracts.Scenario) =
        if String.IsNullOrWhiteSpace scenario.ScenarioName then AppError.createResult EmptyScenarioName
        else Ok scenario

    let checkDuplicateName (scenarios: Contracts.Scenario list) =
        let duplicates = scenarios |> List.map(fun x -> x.ScenarioName) |> String.filterDuplicates
        if duplicates.Length > 0 then AppError.createResult(DuplicateScenarioName duplicates)
        else Ok scenarios

    let checkStepsNotEmpty (scenario: Contracts.Scenario) =
        if List.isEmpty scenario.Steps then AppError.createResult(EmptySteps scenario.ScenarioName)
        else Ok scenario

    let checkEmptyStepName (scenario: Contracts.Scenario) =
        let emptyStepExist = scenario.Steps |> List.exists(fun x -> String.IsNullOrWhiteSpace x.StepName)
        if emptyStepExist then AppError.createResult(EmptyStepName scenario.ScenarioName)
        else Ok scenario

    let validateWarmUpStats (nodesStats: NodeStats list) =
        let folder (state) (stats: NodeStats) =
            state |> Result.bind(fun _ ->
                if stats.FailCount > stats.OkCount then
                    AppError.createResult(WarmUpErrorWithManyFailedSteps(stats.OkCount, stats.FailCount))
                else Ok()
            )

        let okState = Ok()
        nodesStats |> List.fold folder okState

    let validate =
        checkEmptyScenarioName >=> checkStepsNotEmpty >=> checkEmptyStepName

let createCorrelationId (scnName: ScenarioName, copyNumber): Contracts.CorrelationId =
    { Id = sprintf "%s_%i" scnName copyNumber
      ScenarioName = scnName
      CopyNumber = copyNumber }

let createScenarios (scenarios: Contracts.Scenario list) = result {

    let create (scn: Contracts.Scenario) = result {
        let! timeline = scn.LoadSimulations |> LoadTimeLine.createWithDuration
        let! scenario = Validation.validate scn

        return { ScenarioName = scenario.ScenarioName
                 TestInit = scenario.TestInit
                 TestClean = scenario.TestClean
                 Steps = scenario.Steps |> Seq.cast<Step> |> Seq.toList
                 LoadTimeLine = timeline.LoadTimeLine
                 WarmUpDuration = scenario.WarmUpDuration
                 PlanedDuration = timeline.ScenarioDuration
                 ExecutedDuration = None
                 CustomSettings = "" }
    }

    let! vScns = scenarios |> Validation.checkDuplicateName
    return! vScns
            |> List.map(create)
            |> Result.sequence
            |> Result.mapError(List.head)
}

let filterTargetScenarios (targetScenarios: string list) (scenarios: Scenario list) =
    scenarios
    |> List.filter(fun x -> targetScenarios |> Seq.exists(fun target -> x.ScenarioName = target))

let applySettings (settings: ScenarioSetting list) (scenarios: Scenario list) =

    let updateScenario (scenario: Scenario, settings: ScenarioSetting) =

        let timeLine =
            settings.LoadSimulationsSettings
            |> List.map(LoadTimeLine.createSimulationFromSettings)
            |> LoadTimeLine.createWithDuration
            |> Result.getOk

        { scenario with LoadTimeLine = timeLine.LoadTimeLine
                        WarmUpDuration = settings.WarmUpDuration.TimeOfDay
                        PlanedDuration = timeLine.ScenarioDuration
                        CustomSettings = settings.CustomSettings |> Option.defaultValue "" }

    scenarios
    |> List.map(fun scn ->
        settings
        |> List.tryPick(fun x ->
            if x.ScenarioName = scn.ScenarioName then Some(scn, x)
            else None)
        |> Option.map updateScenario
        |> Option.defaultValue scn)

let applyConnectionPoolSettings (settings: ConnectionPoolSetting list) (poolArgs: Contracts.IConnectionPoolArgs<obj> list) =
    poolArgs |> List.map(fun poolArg ->
        let setting = settings |> List.tryFind(fun setng -> setng.PoolName = poolArg.PoolName)
        match setting with
        | Some v -> poolArg |> ConnectionPoolArgs.cloneWith(v.ConnectionCount)
        | None   -> poolArg
    )

let filterDistinctConnectionPoolsArgs (scenarios: Scenario list) =
    scenarios
    |> Seq.collect(fun x -> x.Steps)
    |> Seq.choose(fun x -> if x.ConnectionPoolArgs.PoolName = Constants.EmptyPoolName then None else Some x.ConnectionPoolArgs)
    |> Seq.distinctBy(fun x -> x.PoolName)
    |> Seq.toList

let filterDistinctConnectionPools (scenarios: Scenario list) =
    scenarios
    |> Seq.collect(fun x -> x.Steps)
    |> Seq.choose(fun x -> if x.ConnectionPool.IsSome then Some x.ConnectionPool.Value else None)
    |> Seq.distinctBy(fun x -> x.PoolName)
    |> Seq.toList

let insertConnectionPools (pools: ConnectionPool list) (scenarios: Scenario list) =

    scenarios |> List.map(fun scn ->

        scn.Steps |> List.map(fun step ->
            let pool = pools |> List.tryFind(fun x -> x.PoolName = step.ConnectionPoolArgs.PoolName)
            match pool with
            | Some v -> { step with ConnectionPool = Some v }
            | None   -> step
        )
        |> fun updatedSteps -> { scn with Steps = updatedSteps }
    )

let setExecutedDuration (scenario: Scenario, executedDuration: TimeSpan) =
    if executedDuration < scenario.PlanedDuration then
        { scenario with ExecutedDuration = Some executedDuration }
    else
        { scenario with ExecutedDuration = Some scenario.PlanedDuration }
