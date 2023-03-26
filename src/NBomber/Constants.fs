module internal NBomber.Constants

open System
open Microsoft.Extensions.Configuration
open NBomber.Contracts.Stats

[<Literal>]
let Logo = "NBomber 4"
[<Literal>]
let NBomberWelcomeText = "NBomber {0} started a new session: {1}"

[<Literal>]
let DefaultCopiesCount = 1
[<Literal>]
let DefaultDoNotTrack = false

let AllReportFormats = [ReportFormat.Txt; ReportFormat.Html; ReportFormat.Csv; ReportFormat.Md]

[<Literal>]
let StepResponseKey = "nbomber_step_response"
[<Literal>]
let StepPauseName = "nbomber_step_pause"
[<Literal>]
let DefaultTestSuite = "nbomber_default_test_suite_name"
[<Literal>]
let DefaultTestName = "nbomber_default_test_name"
[<Literal>]
let DefaultReportName = "nbomber_report"
[<Literal>]
let DefaultReportFolder = "reports"
[<Literal>]
let ScenarioGlobalInfo = "global information"

/// Default timeouts

let DefaultSimulationDuration = TimeSpan.FromMinutes 1
let MinSimulationDuration = TimeSpan.FromSeconds 1
let DefaultWarmUpDuration = TimeSpan.FromSeconds 30
let MinReportingInterval = TimeSpan.FromSeconds 5
let DefaultReportingInterval = TimeSpan.FromSeconds 10
let GetPluginStatsTimeout = TimeSpan.FromSeconds 5
let OneSecond = TimeSpan.FromSeconds 1
let ReportingManagerStartDelay = TimeSpan.FromSeconds 5

let MaxTrackableStepLatency = (1000L * TimeSpan.TicksPerMillisecond) * 60L * 10L // 10 min (in ticks)
let MaxTrackableStepResponseSize = int64 Int32.MaxValue

[<Literal>]
let ConsoleRefreshTableCounter = 13
[<Literal>]
let MaxClientInitFailCount = 5
[<Literal>]
let MaxWaitWorkingActorsSec = 60

/// Default status codes

[<Literal>]
let OperationTimeoutMessage = "operation timeout"

[<Literal>]
let TimeoutStatusCode = "-100"
[<Literal>]
let UnhandledExceptionCode = "-101"

let EmptyInfraConfig = ConfigurationBuilder().Build() :> IConfiguration

[<Literal>]
let StatsRounding = 2
[<Literal>]
let ScenarioMaxFailCount = 5_000
