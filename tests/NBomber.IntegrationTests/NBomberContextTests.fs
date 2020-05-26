module Tests.NBomberContext

open System

open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Errors
open NBomber.FSharp
open NBomber.DomainServices

let baseGlobalSettings = {
    ScenariosSettings = None
    ReportFileName = None
    ReportFormats = None
    SendStatsInterval = None
}

let baseScenarioSetting = {
    ScenarioName = "test_scenario"
    WarmUpDuration = DateTime.MinValue
    LoadSimulationsSettings = List.empty
    ConnectionPoolSettings = None
    CustomSettings = None
}

let failStep = Step.create("fail step", fun _ -> Response.Fail() |> Task.singleton)
let baseScenario = Scenario.create "1" [failStep] |> Scenario.withoutWarmUp

let config = {
    TestSuite = Some Constants.DefaultTestSuite
    TestName = Some Constants.DefaultTestName
    TargetScenarios = Some ["1"]
    GlobalSettings = None
}

let context = {
    TestSuite = Constants.DefaultTestSuite
    TestName = Constants.DefaultTestName
    RegisteredScenarios = [baseScenario]
    NBomberConfig = None
    InfraConfig = None
    ReportFileName = None
    ReportFormats = List.empty
    ReportingSinks = List.empty
    SendStatsInterval = Constants.MinSendStatsInterval
    Plugins = List.empty
    ApplicationType = Some ApplicationType.Process
}

[<Fact>]
let ``getTargetScenarios should return all registered scenarios if TargetScenarios are empty`` () =
    let context = { context with NBomberConfig = Some config }
    match NBomberContext.getTargetScenarios(context) with
    | scenarios when scenarios.Length = 1 -> ()
    | _ -> failwith ""

[<Fact>]
let ``getTargetScenarios should return only target scenarios if TargetScenarios are not empty`` () =
    let config = { config with TargetScenarios = Some ["10"] }

    let scn1 = { baseScenario with ScenarioName = "1" }
    let scn2 = { baseScenario with ScenarioName = "2" }

    let context = { context with NBomberConfig = Some config
                                 RegisteredScenarios = [scn1; scn2] }

    match NBomberContext.getTargetScenarios(context) with
    | scenarios when scenarios.Length = 1 && scenarios.[0] = "10" -> ()
    | _ -> failwith ""

[<Property>]
let ``getReportFileName should return from GlobalSettings, if empty then from TestContext, if empty then default name``
    (configValue: string option, contextValue: string option) =

    (configValue.IsNone || configValue.IsSome && not (isNull configValue.Value)) ==> lazy
    (contextValue.IsNone || contextValue.IsSome && not (isNull contextValue.Value)) ==> lazy

    let glSettings = { baseGlobalSettings with ReportFileName = configValue }
    let config = { config with GlobalSettings = Some glSettings }

    let ctx = { context with NBomberConfig = Some config
                             ReportFormats = [ReportFormat.Txt]
                             ReportFileName = contextValue }

    let fileName = NBomberContext.getReportFileName(ctx)

    match configValue, contextValue with
    | Some v1, Some v2 -> test <@ fileName = v1 @>
    | Some v1, None    -> test <@ fileName = v1 @>
    | None, Some v2    -> test <@ fileName = v2 @>
    | None, None       -> test <@ fileName = Constants.DefaultReportName @>

[<Property>]
let ``getReportFormats should return from GlobalSettings, if empty then from TestContext``
    (configValue: ReportFormat list option, contextValue: ReportFormat list) =

    let glSettings = { baseGlobalSettings with ReportFormats = configValue }
    let config = { config with GlobalSettings = Some glSettings }

    let ctx = { context with NBomberConfig = Some config
                             ReportFormats = contextValue
                             ReportFileName = None }

    let formats = NBomberContext.getReportFormats(ctx)
    match configValue, contextValue with
    | Some v, _ -> Assert.True((formats = v))

    | None, v when List.isEmpty v ->
        test <@ formats = List.empty @>

    | None, v -> test <@ formats = contextValue @>

[<Property>]
let ``getTestSuite should return from Config, if empty then from TestContext``
    (configValue: string option) =

    match configValue with
    | Some value ->
        let config = { config with TestSuite = configValue }
        let ctx = { context with NBomberConfig = Some config }
        let testSuite = NBomberContext.getTestSuite(ctx)
        test <@ testSuite = value  @>

    | None ->
        let testSuite = NBomberContext.getTestSuite(context)
        test <@ testSuite = context.TestSuite @>

[<Property>]
let ``getTestName should return from Config, if empty then from TestContext``
    (configValue: string option) =

    match configValue with
    | Some value ->
        let config = { config with TestName = configValue }
        let ctx = { context with NBomberConfig = Some config }
        let testSuite = NBomberContext.getTestName(ctx)
        test <@ testSuite = value  @>

    | None ->
        let testSuite = NBomberContext.getTestName(context)
        test <@ testSuite = context.TestName @>

[<Property>]
let ``getConnectionPoolSettings should return from Config with updated poolName, if empty then empty result``
    (poolNameGenerated: string, configValue: int option) =

    match configValue with
    | Some value ->
        let poolSettings = {PoolName = poolNameGenerated; ConnectionCount = value}
        let scnSettings = { baseScenarioSetting with ConnectionPoolSettings = Some [poolSettings] }
        let glSettings = { baseGlobalSettings with ScenariosSettings = Some [scnSettings] }
        let config = { config with GlobalSettings = Some glSettings }
        let ctx = { context with NBomberConfig = Some config }
        let resut = NBomberContext.getConnectionPoolSettings(ctx)

        test <@ resut.Head.ConnectionCount = poolSettings.ConnectionCount @>
        test <@ resut.Head.PoolName = Domain.Scenario.createConnectionPoolName(scnSettings.ScenarioName, poolSettings.PoolName) @>
        test <@ resut.Head.PoolName <> poolSettings.PoolName @>

    | None ->
        let result = NBomberContext.getConnectionPoolSettings(context)
        test <@ result = List.empty @>

[<Fact>]
let ``checkAvailableTarget should return fail if TargetScenarios has empty value`` () =
    let scn = { baseScenario with ScenarioName = "1" }
    match NBomberContext.Validation.checkAvailableTargets [scn] [" "] with
    | Error (TargetScenariosNotFound _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``checkAvailableTarget should return fail if TargetScenarios has values which doesn't exist in registered scenarios`` () =
    let scn = { baseScenario with ScenarioName = "1" }
    match NBomberContext.Validation.checkAvailableTargets [scn] ["3"] with
    | Error (TargetScenariosNotFound _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``checkEmptyReportName should return fail if ReportFileName is empty`` () =
    match NBomberContext.Validation.checkReportName(" ") with
    | Error (EmptyReportName _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``checkSendStatsInterval should return fail if SendStatsInterval is smaller than min value`` () =
    match NBomberContext.Validation.checkSendStatsInterval(TimeSpan.FromSeconds 9.0) with
    | Error (SendStatsIntervalIsWrong _) -> ()
    | _ -> failwith ""

