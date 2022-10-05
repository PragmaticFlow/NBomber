namespace NBomber.FSharp

open System
open System.IO
open System.Runtime
open System.Threading.Tasks

open Serilog
open CommandLine
open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Configuration
open NBomber.Extensions.Internal
open NBomber.Errors
open NBomber.Domain.DomainTypes
open NBomber.DomainServices

/// DataFeed helps inject test data into your load test. It represents a data source.
[<RequireQualifiedAccess>]
module Feed =

    /// Creates Feed that picks constant value per Step copy.
    /// Every Step copy will have unique constant value.
    let createConstant (name) (data: 'T seq) =
        Domain.Feed.constant(name, fun _ -> data)

    /// Creates Feed (in lazy mode) that picks constant value per Step copy.
    /// Every Step copy will have unique constant value.
    let createConstantLazy (name) (getData: IBaseContext -> 'T seq) =
        Domain.Feed.constant(name, getData)

    /// Creates Feed that goes back to the top of the sequence once the end is reached.
    let createCircular (name) (data: 'T seq) =
        Domain.Feed.circular(name, fun _ -> data)

    /// Creates Feed (in lazy mode) that goes back to the top of the sequence once the end is reached.
    let createCircularLazy (name) (getData: IBaseContext -> 'T seq) =
        Domain.Feed.circular(name, getData)

    /// Creates Feed that randomly picks an item per Step invocation.
    let createRandom (name) (data: 'T seq) =
        Domain.Feed.random(name, fun _ -> data)

    /// Creates Feed (in lazy mode) that randomly picks an item per Step invocation.
    let createRandomLazy (name) (getData: IBaseContext -> 'T seq) =
        Domain.Feed.random(name, getData)

/// Step represents a single user action like login, logout, etc.
[<RequireQualifiedAccess>]
type Step =

    static member private create
        (name: string,
         execute: IStepContext<'TClient,'TFeedItem> -> Task<Response>,
         ?clientFactory: IClientFactory<'TClient>,
         ?feed: IFeed<'TFeedItem>,
         ?timeout: TimeSpan,
         ?doNotTrack: bool,
         ?isPause: bool) =

        match clientFactory with
        | Some v -> if isNull(v :> obj) then raise(ArgumentNullException (nameof clientFactory))
        | None   -> ()

        match feed with
        | Some v -> if isNull(v :> obj) then raise(ArgumentNullException (nameof feed))
        | None   -> ()

        let factory =
            clientFactory
            |> Option.map(fun x -> x :?> IUntypedClientFactory)

        { StepName = name
          ClientFactory = factory
          Execute = execute |> Domain.Step.StepContext.toUntypedExecute
          Feed = feed |> Option.map Domain.Feed.toUntypedFeed
          Timeout = timeout |> Option.defaultValue TimeSpan.Zero
          DoNotTrack = defaultArg doNotTrack Constants.DefaultDoNotTrack
          IsPause = defaultArg isPause false }
          :> IStep

    /// Creates Step.
    /// Step represents a single user action like login, logout, etc.
    static member create
        (name: string,
         execute: IStepContext<'TClient,'TFeedItem> -> Task<Response>,
         ?clientFactory: IClientFactory<'TClient>,
         ?feed: IFeed<'TFeedItem>,
         ?timeout: TimeSpan,
         ?doNotTrack: bool) =

        Step.create(name, execute, ?clientFactory = clientFactory, ?feed = feed, ?timeout = timeout, ?doNotTrack = doNotTrack, isPause = false)

    /// Creates pause step with specified duration in lazy mode.
    /// It's useful when you want to fetch value from some configuration.
    static member createPause (getDuration: unit -> TimeSpan) =
        Step.create(
            name = $"{Constants.StepPauseName}_{Guid.NewGuid()}",
            execute = (fun _ -> backgroundTask {
                do! Task.Delay(getDuration())
                return Response.ok()
            }),
            doNotTrack = true,
            isPause = true
        )

    /// Creates pause step in milliseconds in lazy mode.
    /// It's useful when you want to fetch value from some configuration.
    static member createPause (getDuration: unit -> int) =
        let func = getDuration >> float >> TimeSpan.FromMilliseconds
        Step.createPause(func)

    /// Creates pause step with specified duration.
    static member createPause (duration: TimeSpan) =
        Step.createPause(fun () -> duration)

    /// Creates pause step with specified duration in milliseconds.
    static member createPause (milliseconds: int) =
        Step.createPause(fun () -> milliseconds)

/// Scenario is basically a workflow that virtual users will follow. It helps you organize steps into user actions.
/// Scenarios are always running in parallel (it's opposite to steps that run sequentially).
/// You should think about Scenario as a system thread.
[<RequireQualifiedAccess>]
module Scenario =

    /// Creates scenario with steps which will be executed sequentially.
    /// Scenario is basically a workflow that virtual users will follow. It helps you organize steps into user actions.
    /// Scenarios are always running in parallel (it's opposite to steps that run sequentially).
    /// You should think about Scenario as a system thread.
    let create (name: string) (steps: IStep list): Contracts.Scenario =
        { ScenarioName = name
          Init = None
          Clean = None
          Steps = steps
          WarmUpDuration = Some Constants.DefaultWarmUpDuration
          LoadSimulations = [LoadSimulation.KeepConstant(copies = Constants.DefaultCopiesCount, during = Constants.DefaultSimulationDuration)]
          CustomStepOrder = None
          StepInterception = None }

    /// Initializes scenario.
    /// You can use it to for example to prepare your target system or to parse and apply configuration.
    let withInit (initFunc: IScenarioContext -> Task<unit>) (scenario: Contracts.Scenario) =
        { scenario with Init = Some(fun token -> initFunc(token) :> Task) }

    /// Cleans scenario's resources.
    let withClean (cleanFunc: IScenarioContext -> Task<unit>) (scenario: Contracts.Scenario) =
        { scenario with Clean = Some(fun token -> cleanFunc(token) :> Task) }

    /// Sets warm-up duration
    /// Warm-up will just simply start a scenario with a specified duration.
    let withWarmUpDuration (duration: TimeSpan) (scenario: Contracts.Scenario) =
        { scenario with WarmUpDuration = Some duration }

    let withoutWarmUp (scenario: Contracts.Scenario) =
        { scenario with WarmUpDuration = None }

    /// Sets load simulations.
    /// Default value is: KeepConstant(copies = 1, during = minutes 1)
    /// NBomber is always running simulations in sequential order that you defined them.
    /// All defined simulations are represent the whole Scenario duration.
    let withLoadSimulations (loadSimulations: LoadSimulation list) (scenario: Contracts.Scenario) =
        { scenario with LoadSimulations = loadSimulations }

    /// Sets custom steps order that will be used by NBomber Scenario executor.
    /// By default, all steps are executing sequentially but you can inject your custom order.
    /// getStepsOrder function will be invoked on every turn before steps list execution.
    let withCustomStepOrder (getStepsOrder: unit -> string[]) (scenario: Contracts.Scenario) =
        { scenario with CustomStepOrder = Some getStepsOrder }

    /// Sets step interception handler.
    /// It introduces more granular execution control of your steps than you can achieve with CustomStepOrder.
    /// By default, all steps are executing sequentially but you can inject your custom step interception to change default order per step iteration.
    /// handler function will be invoked before each step.
    /// You can think about interception handler like a callback before step invocation where you can specify what step should be invoked.
    let withStepInterception (handler: IStepInterceptionContext voption -> string voption) (scenario: Contracts.Scenario) =
        { scenario with StepInterception = Some handler }

/// NBomberRunner is responsible for registering and running scenarios.
/// Also it provides configuration points related to infrastructure, reporting, loading plugins.
[<RequireQualifiedAccess>]
module NBomberRunner =

    /// Registers scenario in NBomber environment.
    let registerScenario (scenario: Contracts.Scenario) =
        { NBomberContext.empty with RegisteredScenarios = [scenario] }

    /// Registers scenarios in NBomber environment.
    /// Scenarios will be run in parallel.
    let registerScenarios (scenarios: Contracts.Scenario list) =
        { NBomberContext.empty with RegisteredScenarios = scenarios }

    /// Sets target scenarios among all registered that will execute
    let withTargetScenarios (scenarioNames: string list) (context: NBomberContext) =
        context |> NBomberContext.setTargetScenarios scenarioNames

    /// Sets test suite name
    /// Default value is: nbomber_default_test_suite_name.
    let withTestSuite (testSuite: string) (context: NBomberContext) =
        { context with TestSuite = testSuite }

    /// Sets test name
    /// Default value is: nbomber_default_test_name.
    let withTestName (testName: string) (context: NBomberContext) =
        { context with TestName = testName }

    /// Sets output report name.
    /// Default name: nbomber_report.
    let withReportFileName (reportFileName: string) (context: NBomberContext) =
        let report = { context.Reporting with FileName = Some reportFileName }
        { context with Reporting = report }

    /// Sets output report folder path.
    /// Default folder path: "./reports".
    let withReportFolder (reportFolderPath: string) (context: NBomberContext) =
        let report = { context.Reporting with FolderName = Some reportFolderPath }
        { context with Reporting = report }

    let withReportFormats (reportFormats: ReportFormat list) (context: NBomberContext) =
        let report = { context.Reporting with Formats = reportFormats }
        { context with Reporting  = report }

    /// Sets to run without reports
    let withoutReports (context: NBomberContext) =
        let report = { context.Reporting with Formats = [] }
        { context with Reporting = report }

    /// Sets real-time reporting interval.
    /// Default value: 10 seconds, min value: 5 sec
    let withReportingInterval (interval: TimeSpan) (context: NBomberContext) =
        let report = { context.Reporting with ReportingInterval = interval }
        { context with Reporting = report }

    /// Sets worker plugins.
    /// Worker plugin is a plugin that starts at the test start and works as a background worker.
    let withWorkerPlugins (plugins: IWorkerPlugin list) (context: NBomberContext) =
        { context with WorkerPlugins = plugins }

    /// Loads configuration.
    /// The following formats are supported:
    /// - json (.json)
    let loadConfig (path: string) (context: NBomberContext) =
        let config =
            match Path.GetExtension(path) with
            | ".json" -> path |> File.ReadAllText |> JsonExt.deserialize<NBomberConfig>
            | _       -> failwith "unsupported config format"

        { context with NBomberConfig = Some config }

    /// Loads infrastructure configuration.
    /// The following formats are supported:
    /// - json (.json)
    let loadInfraConfig (path: string) (context: NBomberContext) =
        let config =
            match Path.GetExtension(path) with
            | ".json" -> ConfigurationBuilder().AddJsonFile(path).Build() :> IConfiguration
            | _       -> failwith "unsupported config format"

        { context with InfraConfig = Some config }

    /// Sets logger configuration.
    /// Make sure that you always return a new instance of LoggerConfiguration.
    /// You can also configure logger via configuration file.
    /// For this use NBomberRunner.loadInfraConfig
    let withLoggerConfig (createLoggerConfig: unit -> LoggerConfiguration) (context: NBomberContext) =
        try
            // this is limitation of Serilog
            // to invoke CreateLogger() twice on the same instance of LoggerConfiguration
            // it's why we can't just accept LoggerConfiguration
            createLoggerConfig().CreateLogger() |> ignore
            createLoggerConfig().CreateLogger() |> ignore
        with
        | :? InvalidOperationException ->
            failwith "createLoggerConfig should always return a new instance of LoggerConfiguration"

        { context with CreateLoggerConfig = Some createLoggerConfig }

    /// Enables or disables hints analyzer.
    /// Hints analyzer - analyze node stats to provide some hints in case of finding wrong usage or some other issue.
    /// The default value is true.
    let enableHintsAnalyzer (enable: bool) (context: NBomberContext) =
        { context with EnableHintsAnalyzer = enable }

    /// Sets and overrides the global default step timeout.
    /// This value will be applied for steps that have not set timeout.
    /// By default DefaultStepTimeout = 1 second
    let withDefaultStepTimeout (timeout: TimeSpan) (context: NBomberContext) =
        { context with DefaultStepTimeoutMs = int timeout.TotalMilliseconds }

    /// Sets and overrides the default max fail count.
    /// In case of any scenario is reaching max fail count, then NBomber will stop the whole load test.
    /// By default MaxFailCount = 500
    let withMaxFailCount (failCount: int) (context: NBomberContext) =
        { context with MaxFailCount = failCount }

    let internal executeCliArgs (args) (context: NBomberContext) =

        let loadConfigFn (loadConfig) (configPath) (context) =
            if String.IsNullOrWhiteSpace configPath then context
            else loadConfig configPath context

        let setTargetScenarios (targetScenarios: string seq) (context: NBomberContext) =
            if Seq.isEmpty targetScenarios then context
            else NBomberContext.setTargetScenarios (List.ofSeq targetScenarios) context

        match CommandLine.Parser.Default.ParseArguments<CommandLineArgs>(args) with
        | :? Parsed<CommandLineArgs> as parsed ->
            let cliArgs = parsed.Value
            let loadCnf = loadConfigFn loadConfig cliArgs.Config
            let loadInfra = loadConfigFn loadInfraConfig cliArgs.InfraConfig

            let run =
                loadCnf
                >> loadInfra
                >> setTargetScenarios cliArgs.TargetScenarios

            run context

        | _ -> context

    let internal runWithResult (args) (context: NBomberContext) =
        GCSettings.LatencyMode <- GCLatencyMode.SustainedLowLatency

        if Seq.isEmpty args then
            NBomberRunner.run context
        else
            context |> executeCliArgs args |> NBomberRunner.run

    let run (context: NBomberContext) =
        context
        |> runWithResult List.empty
        |> Result.map(fun x -> x.FinalStats)
        |> Result.mapError AppError.toString

    /// Runs scenarios with arguments.
    /// The following CLI commands are supported:
    /// -c or --config: loads configuration,
    /// -i or --infra: loads infrastructure configuration.
    /// Examples of possible args:
    /// -c config.json -i infra_config.json
    /// --config=config.json --infra=infra_config.json
    let runWithArgs (args) (context: NBomberContext) =
        context
        |> runWithResult args
        |> Result.map(fun x -> x.FinalStats)
        |> Result.mapError AppError.toString
