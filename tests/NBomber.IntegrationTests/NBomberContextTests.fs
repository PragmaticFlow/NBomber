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
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Extensions.Internal
open NBomber.Errors
open NBomber.DomainServices
open NBomber.FSharp

let baseGlobalSettings = {
    ScenariosSettings = None
    ReportFileName = None
    ReportFolder = None
    ReportFormats = None
    ReportingInterval = None
    EnableHintsAnalyzer = None
    DefaultStepTimeoutMs = None
    MaxFailCount = None
}

let baseScenarioSetting = {
    ScenarioName = "test_scenario"
    WarmUpDuration = None
    LoadSimulationsSettings = None
    ClientFactorySettings = None
    CustomSettings = None
    CustomStepOrder = None
}

let failStep = Step.create("fail step", fun _ -> Response.fail() |> Task.singleton)
let baseScenario = Scenario.create "1" [failStep] |> Scenario.withoutWarmUp

let config = {
    TestSuite = Some Constants.DefaultTestSuite
    TestName = Some Constants.DefaultTestName
    TargetScenarios = Some ["1"]
    GlobalSettings = None
}

let context =
    NBomberRunner.registerScenario baseScenario
    |> NBomberRunner.enableHintsAnalyzer false

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
    | scenarios when scenarios.Length = 1 && scenarios[0] = "10" -> ()
    | _ -> failwith ""

[<Property>]
let ``getReportFileNameOrDefault should return from GlobalSettings, if empty then from TestContext, if empty then default name``
    (configValue: string option, contextValue: string option) =

    (configValue.IsNone || configValue.IsSome && not (isNull configValue.Value)) ==> lazy
    (contextValue.IsNone || contextValue.IsSome && not (isNull contextValue.Value)) ==> lazy

    let glSettings = { baseGlobalSettings with ReportFileName = configValue }
    let config = { config with GlobalSettings = Some glSettings }

    let ctx = {
        context with NBomberConfig = Some config
                     Reporting = { context.Reporting with Formats = [ReportFormat.Txt]
                                                          FileName = contextValue }
    }

    let currentTime = DateTime.UtcNow
    let currentTimeStr = currentTime.ToString("yyyy-MM-dd--HH-mm-ss")
    let fileName = ctx |> NBomberContext.getReportFileNameOrDefault(currentTime)

    match configValue, contextValue with
    | Some v1, Some v2 -> test <@ fileName = v1 @>
    | Some v1, None    -> test <@ fileName = v1 @>
    | None, Some v2    -> test <@ fileName = v2 @>
    | None, None       -> test <@ fileName = $"{Constants.DefaultReportName}_{currentTimeStr}" @>

[<Property>]
let ``getReportFormats should return from GlobalSettings, if empty then from TestContext``
    (configValue: ReportFormat list option, contextValue: ReportFormat list) =

    let glSettings = { baseGlobalSettings with ReportFormats = configValue }
    let config = { config with GlobalSettings = Some glSettings }

    let ctx = {
        context with NBomberConfig = Some config
                     Reporting = { context.Reporting with Formats = contextValue
                                                          FileName = None }
    }

    let formats = NBomberContext.getReportFormats(ctx)
    match configValue, contextValue with
    | Some v, _ -> Assert.True((formats = v))

    | None, v when List.isEmpty v ->
        test <@ formats = List.empty @>

    | None, v -> test <@ formats = contextValue @>

[<Property>]
let ``getHintAnalyzer should be based on EnableHintsAnalyzer from GlobalSettings, if empty then from TestContext``
    (configValue: bool option, contextValue: bool) =

    let glSettings = { baseGlobalSettings with EnableHintsAnalyzer = configValue }
    let config = { config with GlobalSettings = Some glSettings }

    let ctx = {
        context with
            NBomberConfig = Some config
            EnableHintsAnalyzer = contextValue
    }

    let enable = NBomberContext.getEnableHintAnalyzer ctx
    match configValue with
    | Some value -> test <@ value = enable @>
    | None       -> test <@ contextValue = enable @>

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
let ``getClientFactorySettings should return from Config with updated poolName, if empty then empty result``
    (poolNameGenerated: string, configValue: int option) =

    match configValue with
    | Some value ->
        let poolSettings = { FactoryName = poolNameGenerated; ClientCount = value }
        let scnSettings = { baseScenarioSetting with ClientFactorySettings = Some [poolSettings] }
        let glSettings = { baseGlobalSettings with ScenariosSettings = Some [scnSettings] }
        let config = { config with GlobalSettings = Some glSettings }
        let ctx = { context with NBomberConfig = Some config }
        let result = NBomberContext.getClientFactorySettings ctx

        test <@ result.Head.ClientCount = poolSettings.ClientCount @>
        test <@ result.Head.FactoryName = Domain.ClientFactory.createFullName poolSettings.FactoryName scnSettings.ScenarioName @>
        test <@ result.Head.FactoryName <> poolSettings.FactoryName @>

    | None ->
        let result = NBomberContext.getClientFactorySettings context
        test <@ result = List.empty @>

[<Fact>]
let ``getReportingInterval should return fail if interval is smaller than min value`` () =

    let okContext =    { context with Reporting = { context.Reporting with ReportingInterval = seconds 5 }}
    let errorContext = { context with Reporting = { context.Reporting with ReportingInterval = seconds 2 }}

    let ok = NBomberContext.getReportingInterval(okContext)
    let error = NBomberContext.getReportingInterval(errorContext)

    test <@ Result.isOk ok @>
    test <@ Result.isError error @>

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
        | Error EmptyReportName -> ()
        | Ok value -> failwithf $"received OK for char: %s{value}"
        | error -> error |> Result.getError |> AppError.toString |> failwith
    )

[<Fact>]
let ``checkReportFolder should return fail if ReportFolderPath is empty`` () =
    match NBomberContext.Validation.checkReportFolder(" ") with
    | Error (EmptyReportFolderPath _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``checkReportFolder should return fail if ReportFolderPath contains invalid chars`` () =
    Path.GetInvalidPathChars()
    |> Seq.map(Array.singleton >> String)
    |> Seq.iter(fun x ->
        match NBomberContext.Validation.checkReportFolder(x) with
        | Error (InvalidReportFolderPath _) -> ()
        | Error EmptyReportFolderPath -> ()
        | Ok value -> failwithf $"received OK for char: %s{value}"
        | error -> error |> Result.getError |> AppError.toString |> failwith
    )

[<Fact>]
let ``checkReportingInterval should return fail if ReportingInterval is smaller than min value`` () =
    match NBomberContext.Validation.checkReportingInterval(seconds 3) with
    | Error (ReportingIntervalSmallerThanMin _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``createSessionArgs should properly create args with default values`` () =

    let context = NBomberRunner.registerScenario baseScenario

    taskResult {
        let testInfo = SessionArgs.empty.TestInfo
        let! scenarios  = context |> NBomberContext.createScenarios
        let! sessionArgs = context |> NBomberContext.createSessionArgs testInfo scenarios
        return sessionArgs
    }
    |> fun t -> t.Result
    |> Result.getOk
    |> fun sessionArgs ->
        test <@ context.NBomberConfig.IsNone @>
        test <@ sessionArgs.NBomberConfig.GlobalSettings.IsSome @>
        test <@ sessionArgs.NBomberConfig.GlobalSettings.Value.DefaultStepTimeoutMs = Some Constants.DefaultStepTimeoutMs @>
        test <@ sessionArgs.NBomberConfig.GlobalSettings.Value.ReportFileName.IsSome @>
        test <@ sessionArgs.NBomberConfig.GlobalSettings.Value.ReportFolder.IsSome @>
        test <@ sessionArgs.NBomberConfig.GlobalSettings.Value.ReportFormats.IsSome @>
        test <@ sessionArgs.NBomberConfig.GlobalSettings.Value.ReportingInterval.IsSome @>
        test <@ sessionArgs.NBomberConfig.GlobalSettings.Value.ScenariosSettings.IsSome @>
        test <@ sessionArgs.NBomberConfig.GlobalSettings.Value.EnableHintsAnalyzer.IsSome @>

[<Fact>]
let ``createSessionArgs should validate empty register scenarios`` () =
    NBomberRunner.registerScenarios []
    |> NBomberRunner.run
    |> Result.getError
    |> fun error ->
        test <@ error.Contains "No scenarios were registered" @>
