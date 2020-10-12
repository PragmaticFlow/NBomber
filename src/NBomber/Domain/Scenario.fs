[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Scenario

open System
open System.IO
open System.Text

open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Extensions.InternalExtensions
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

    let checkDuplicateScenarioName (scenarios: Contracts.Scenario list) =
        let duplicates = scenarios |> Seq.map(fun x -> x.ScenarioName) |> String.filterDuplicates |> Seq.toList
        if duplicates.Length > 0 then AppError.createResult(DuplicateScenarioName duplicates)
        else Ok scenarios

    let checkStepsNotEmpty (scenario: Contracts.Scenario) =
        if List.isEmpty scenario.Steps then AppError.createResult(EmptySteps scenario.ScenarioName)
        else Ok scenario

    let checkEmptyStepName (scenario: Contracts.Scenario) =
        let emptyStepExist = scenario.Steps |> List.exists(fun x -> String.IsNullOrWhiteSpace x.StepName)
        if emptyStepExist then AppError.createResult(EmptyStepName scenario.ScenarioName)
        else Ok scenario

    let checkDuplicateConnectionPoolArgs (scenario: Contracts.Scenario) =
        scenario.Steps
        |> Seq.cast<Step>
        |> Seq.choose(fun x -> x.ConnectionPoolArgs)
        |> Seq.distinct // checking on different instances with the same name
        |> Seq.groupBy(fun x -> x.PoolName)
        |> Seq.choose(fun (name,pools) -> if Seq.length(pools) > 1 then Some name else None)
        |> Seq.toList
        |> function
            | [] -> Ok scenario
            | poolName::tail  -> AppError.createResult(DuplicateConnectionPoolName(scenario.ScenarioName, poolName))

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
        checkEmptyScenarioName >=> checkStepsNotEmpty >=> checkEmptyStepName >=> checkDuplicateConnectionPoolArgs

let createCorrelationId (scnName: ScenarioName, copyNumber): CorrelationId =
    { Id = sprintf "%s_%i" scnName copyNumber
      ScenarioName = scnName
      CopyNumber = copyNumber }

let createConnectionPoolName (scenarioName, poolName) =
    sprintf "%s.%s" scenarioName poolName

let updateConnectionPoolArgsName (scenarioName: ScenarioName) (steps: IStep list) =
    steps
    |> Seq.cast<Step>
    |> Seq.map(fun step ->
        match step.ConnectionPoolArgs with
        | Some pool ->
            let poolName = createConnectionPoolName(scenarioName, pool.PoolName)
            { step with ConnectionPoolArgs = Some(pool.Clone(poolName)) }

        | None -> step
    )
    |> Seq.toList

let createScenarios (scenarios: Contracts.Scenario list) = result {

    let create (scn: Contracts.Scenario) = result {
        let! timeline = scn.LoadSimulations |> LoadTimeLine.createWithDuration
        let! scenario = Validation.validate(scn)

        return { ScenarioName = scenario.ScenarioName
                 Init = scenario.Init
                 Clean = scenario.Clean
                 Steps = scenario.Steps |> updateConnectionPoolArgsName(scenario.ScenarioName)
                 LoadTimeLine = timeline.LoadTimeLine
                 WarmUpDuration = scenario.WarmUpDuration
                 PlanedDuration = timeline.ScenarioDuration
                 ExecutedDuration = None
                 CustomSettings = "" }
    }

    let! vScns = scenarios |> Validation.checkDuplicateScenarioName
    return! vScns
            |> List.map(create)
            |> Result.sequence
            |> Result.mapError(List.head)
}

let filterTargetScenarios (targetScenarios: string list) (scenarios: Scenario list) =
    scenarios
    |> List.filter(fun x -> targetScenarios |> Seq.exists(fun target -> x.ScenarioName = target))

let applySettings (settings: ScenarioSetting list) (scenarios: Scenario list) =

    let getWarmUpDuration (settings: ScenarioSetting) =
        match settings.WarmUpDuration with
        | Some v -> TimeSpan.Parse v
        | None   -> TimeSpan.Zero

    let updateScenario (scenario: Scenario, settings: ScenarioSetting) =

        let timeLine =
            settings.LoadSimulationsSettings
            |> List.map(LoadTimeLine.createSimulationFromSettings)
            |> LoadTimeLine.createWithDuration
            |> Result.getOk

        { scenario with LoadTimeLine = timeLine.LoadTimeLine
                        WarmUpDuration = getWarmUpDuration(settings)
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

let applyConnectionPoolSettings (settings: ConnectionPoolSetting list) (poolArgs: ConnectionPoolArgs<obj> list) =
    poolArgs |> List.map(fun poolArg ->
        let setting = settings |> List.tryFind(fun setng -> setng.PoolName = poolArg.PoolName)
        match setting with
        | Some v -> poolArg.Clone(v.ConnectionCount)
        | None   -> poolArg
    )

let filterDistinctConnectionPoolsArgs (scenarios: Scenario list) =
    scenarios
    |> Seq.collect(fun x -> x.Steps)
    |> Seq.choose(fun x -> x.ConnectionPoolArgs)
    |> Seq.distinctBy(fun x -> x.PoolName)
    |> Seq.toList

let filterDistinctConnectionPools (scenarios: Scenario list) =
    scenarios
    |> Seq.collect(fun x -> x.Steps)
    |> Seq.choose(fun x -> x.ConnectionPool)
    |> Seq.distinctBy(fun x -> x.PoolName)
    |> Seq.toList

let setConnectionPools (pools: ConnectionPool list) (scenarios: Scenario list) =

    let setPool (scenario: Scenario) =
        seq {
            for step in scenario.Steps do
            match step.ConnectionPoolArgs with
            | Some poolArgs ->
                let pool = pools |> Seq.tryFind(fun x -> x.PoolName = poolArgs.PoolName)
                match pool with
                | Some v -> { step with ConnectionPool = Some v }
                | None   -> step

            | None -> step
        }

    scenarios
    |> List.map(fun scenario -> { scenario with Steps = scenario |> setPool |> Seq.toList })

let setExecutedDuration (scenario: Scenario, executedDuration: TimeSpan) =
    if executedDuration < scenario.PlanedDuration then
        { scenario with ExecutedDuration = Some executedDuration }
    else
        { scenario with ExecutedDuration = Some scenario.PlanedDuration }

module ScenarioContext =

    let create (nodeInfo, cancelToken, logger) = {
        new IScenarioContext with
            member _.NodeInfo = nodeInfo
            member _.CustomSettings = ConfigurationBuilder().Build() :> IConfiguration
            member _.CancellationToken = cancelToken
            member _.Logger = logger
    }

    let setCustomSettings (context: IScenarioContext, customSettings: string) =

        let parseCustomSettings (settings: string) =
            try
                let stream = new MemoryStream(settings |> Encoding.UTF8.GetBytes)
                ConfigurationBuilder().AddJsonStream(stream).Build() :> IConfiguration
            with
            | _ -> ConfigurationBuilder().Build() :> IConfiguration

        { new IScenarioContext with
            member _.NodeInfo = context.NodeInfo
            member _.CustomSettings = parseCustomSettings(customSettings)
            member _.CancellationToken = context.CancellationToken
            member _.Logger = context.Logger }
