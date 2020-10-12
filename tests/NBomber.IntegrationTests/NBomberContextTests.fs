module Tests.NBomberContext

open System
open System.IO

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
    ReportFolder = None
    ReportFormats = None
    SendStatsInterval = None
}

let baseScenarioSetting = {
    ScenarioName = "test_scenario"
    WarmUpDuration = ""
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
    CreateLoggerConfig = None
    ReportFileName = None
    ReportFolder = None
    ReportFormats = List.empty
    ReportingSinks = List.empty
    SendStatsInterval = Constants.MinSendStatsInterval
    WorkerPlugins = List.empty
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
        let result = NBomberContext.getConnectionPoolSettings(ctx)

        test <@ result.Head.ConnectionCount = poolSettings.ConnectionCount @>
        test <@ result.Head.PoolName = Domain.Scenario.createConnectionPoolName(scnSettings.ScenarioName, poolSettings.PoolName) @>
        test <@ result.Head.PoolName <> poolSettings.PoolName @>

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
let ``checkReportName should return fail if ReportFileName is empty`` () =
    match NBomberContext.Validation.checkReportName(" ") with
    | Error (EmptyReportName _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``checkReportName should return fail if ReportFileName contains invalid chars`` () =
    Path.GetInvalidFileNameChars()
    |> Seq.map(Array.singleton >> String)
    |> Seq.iter(fun x ->
        match NBomberContext.Validation.checkReportName(x) with
        | Error (InvalidReportName _) -> ()
        | _ -> failwith ""
    )

[<Fact>]
let ``checkReportFolder should return fail if ReportFolderPath is empty`` () =
    match NBomberContext.Validation.checkReportFolder(" ") with
    | Error (EmptyReportFolderPath _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``checkReportFolder should return fail if ReportFolderPath contains invalid chars`` () =
    try

        Path.GetInvalidPathChars()
        |> Seq.map(Array.singleton >> String)
        |> Seq.iter(fun x ->
            match NBomberContext.Validation.checkReportFolder(x) with
            | Error (InvalidReportFolderPath _) -> ()
            | _ -> failwith ""
        )
    with
    | ex -> failwith (ex.ToString())

[<Fact>]
let ``checkSendStatsInterval should return fail if SendStatsInterval is smaller than min value`` () =
    match NBomberContext.Validation.checkSendStatsInterval(TimeSpan.FromSeconds 9.0) with
    | Error (SendStatsValueSmallerThanMin _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``checkSendStatsSetting should return fail if SendStatsInterval has invalid format`` () =
    match NBomberContext.Validation.checkSendStatsSettings("1232") with
    | Error (SendStatsConfigValueHasInvalidFormat _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``checkSendStatsSetting should return ok if SendStatsInterval has valid format and correct value`` () =
    match NBomberContext.Validation.checkSendStatsSettings("00:00:20") with
    | Error _ -> failwith ""
    | _       -> ()

[<Fact>]
let ``checkWarmUpSettings should return fail if WarmUp has invalid format`` () =
    let setting = { baseScenarioSetting with WarmUpDuration = "::"}
    match NBomberContext.Validation.checkWarmUpSettings [setting] with
    | Error (WarmUpConfigValueHasInvalidFormat _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``checkWarmUpSettings should return ok if WarmUp has correct format`` () =
    let setting = { baseScenarioSetting with WarmUpDuration = "00:00:10"}
    match NBomberContext.Validation.checkWarmUpSettings [setting] with
    | Error _ -> failwith ""
    | _       -> ()

[<Fact>]
let ``checkLoadSimulationsSettings should return fail if duration time has invalid format`` () =
    let setting = { baseScenarioSetting with LoadSimulationsSettings = [LoadSimulationSettings.KeepConstant(1, "asd:123")]}
    match NBomberContext.Validation.checkLoadSimulationsSettings [setting] with
    | Error (LoadSimulationConfigValueHasInvalidFormat _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``checkLoadSimulationsSettings should return ok if duration time has correct format`` () =
    let setting = { baseScenarioSetting with LoadSimulationsSettings = [LoadSimulationSettings.KeepConstant(1, "00:00:25")]}
    match NBomberContext.Validation.checkLoadSimulationsSettings [setting] with
    | Error _ -> failwith ""
    | _       -> ()
