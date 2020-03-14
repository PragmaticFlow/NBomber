module Tests.TestContextTests

open System

open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit

open NBomber
open NBomber.Configuration
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Infra

let globalSettings = {
    ScenariosSettings = None
    TargetScenarios = Some ["1"]
    ConnectionPoolSettings = None
    ReportFileName = None
    ReportFormats = None
    SendStatsInterval = None
}

let scenario = Scenario.create "1" []
               |> Scenario.withOutWarmUp

let config = {
    TestSuite = Some Constants.DefaultTestSuite
    TestName = Some Constants.DefaultTestName
    GlobalSettings = None
    CustomSettings = None
}

let context = {
    TestSuite = Constants.DefaultTestSuite
    TestName = Constants.DefaultTestName
    RegisteredScenarios = [scenario]
    TestConfig = None
    InfraConfig = None
    ReportFileName = None
    ReportFormats = List.empty
    ReportingSinks = List.empty
    SendStatsInterval = TimeSpan.FromSeconds(Constants.MinSendStatsIntervalSec)
}

[<Fact>]
let ``TestContext.getTargetScenarios should return all registered scenarios if TargetScenarios are empty`` () =
    let glSettings = { globalSettings with TargetScenarios = None }
    let config = { config with GlobalSettings = Some glSettings }
    let context = { context with TestConfig = Some config }

    match TestContext.getTargetScenarios(context) with
    | scenarios when scenarios.Length = 1 -> ()
    | _ -> failwith ""

[<Fact>]
let ``TestContext.getTargetScenarios should return only target scenarios if TargetScenarios are not empty`` () =
    let glSettings = { globalSettings with TargetScenarios = Some ["10"] }
    let config = { config with GlobalSettings = Some glSettings }

    let scn1 = { scenario with ScenarioName = "1" }
    let scn2 = { scenario with ScenarioName = "2" }

    let context = { context with TestConfig = Some config
                                 RegisteredScenarios = [scn1; scn2] }

    match TestContext.getTargetScenarios(context) with
    | scenarios when scenarios.Length = 1 && scenarios.[0] = "10" -> ()
    | _ -> failwith ""

[<Property>]
let ``TestContext.getReportFileName should return from GlobalSettings, if empty then from TestContext, if empty then default name``
    (configValue: string option, contextValue: string option) =

    (configValue.IsNone || configValue.IsSome && not (isNull configValue.Value)) ==> lazy
    (contextValue.IsNone || contextValue.IsSome && not (isNull contextValue.Value)) ==> lazy

    let glSettings = { globalSettings with ReportFileName = configValue }
    let config = { config with GlobalSettings = Some glSettings }

    let ctx = { context with TestConfig = Some config
                             ReportFormats = [ReportFormat.Txt]
                             ReportFileName = contextValue }

    let fileName = TestContext.getReportFileName("sessionId", ctx)

    match configValue, contextValue with
    | Some v1, Some v2 -> test <@ fileName = v1 @>
    | Some v1, None    -> test <@ fileName = v1 @>
    | None, Some v2    -> test <@ fileName = v2 @>
    | None, None       -> test <@ fileName = "report_sessionId" @>

[<Property>]
let ``TestContext.getReportFormats should return from GlobalSettings, if empty then from TestContext, if empty then all supported formats``
    (configValue: ReportFormat list option, contextValue: ReportFormat list) =

    let glSettings = { globalSettings with ReportFormats = configValue }
    let config = { config with GlobalSettings = Some glSettings }

    let ctx = { context with TestConfig = Some config
                             ReportFormats = contextValue
                             ReportFileName = None }

    let formats = TestContext.getReportFormats(ctx)
    match configValue, contextValue with
    | Some v, _ -> Assert.True((formats = v))

    | None, v when List.isEmpty v ->
        test <@ formats = Constants.AllReportFormats @>

    | None, v -> test <@ formats = contextValue @>

[<Property>]
let ``TestContext.getTestSuite should return from Config, if empty then from TestContext``
    (configValue: string option) =

    match configValue with
    | Some value ->
        let config = { config with TestSuite = configValue }
        let ctx = { context with TestConfig = Some config }
        let testSuite = TestContext.getTestSuite(ctx)
        test <@ testSuite = value  @>

    | None ->
        let testSuite = TestContext.getTestSuite(context)
        test <@ testSuite = context.TestSuite @>

[<Property>]
let ``TestContext.getTestName should return from Config, if empty then from TestContext``
    (configValue: string option) =

    match configValue with
    | Some value ->
        let config = { config with TestName = configValue }
        let ctx = { context with TestConfig = Some config }
        let testSuite = TestContext.getTestName(ctx)
        test <@ testSuite = value  @>

    | None ->
        let testSuite = TestContext.getTestName(context)
        test <@ testSuite = context.TestName @>

[<Property>]
let ``TestContext.getConnectionPoolSettings should return from Config, if empty then empty result``
    (configValue: int option) =

    match configValue with
    | Some value ->
        let poolSettings = Some [{PoolName = "test_pool"; ConnectionCount = value}]
        let glSettings = { globalSettings with ConnectionPoolSettings = poolSettings }
        let config = { config with GlobalSettings = Some glSettings }
        let ctx = { context with TestConfig = Some config }
        let resut = TestContext.getConnectionPoolSettings(ctx)
        test <@ resut = poolSettings.Value  @>

    | None ->
        let result = TestContext.getConnectionPoolSettings(context)
        test <@ result = List.empty @>
