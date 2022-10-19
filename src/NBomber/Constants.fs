module internal NBomber.Constants

open System
open Microsoft.Extensions.Configuration
open NBomber.Contracts.Stats

[<Literal>]
let Logo = "NBomber"
[<Literal>]
let NBomberWelcomeText = "NBomber {0} Started a new session: {1}"

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
let DefaultReportFolder = "./reports"
[<Literal>]
let ScenarioGlobalInfo = "global information"

/// Default timeouts

let DefaultSimulationDuration = TimeSpan.FromMinutes 1.0
let MinSimulationDuration = TimeSpan.FromSeconds 1.0
let MaxSimulationDuration = TimeSpan.FromDays 10.0
let DefaultWarmUpDuration = TimeSpan.FromSeconds 30.0
let MinReportingInterval = TimeSpan.FromSeconds 5.0
let DefaultReportingInterval = TimeSpan.FromSeconds 10.0
let DefaultStepTimeoutMs = 5_000
let GetPluginStatsTimeout = TimeSpan.FromSeconds 5.0

let MaxTrackableStepLatency = (1000L * TimeSpan.TicksPerMillisecond) * 60L * 10L // 10 min (in ticks)
let MaxTrackableStepResponseSize = int64 Int32.MaxValue

[<Literal>]
let SchedulerTimerDriftMs = 10.0
[<Literal>]
let SchedulerTickIntervalMs = 1_000.0
[<Literal>]
let ReportingTimerCompleteMs = 3_000
[<Literal>]
let ConsoleRefreshTableCounter = 13
[<Literal>]
let MaxClientInitFailCount = 5

/// Default status codes

[<Literal>]
let TimeoutStatusCode = -100
[<Literal>]
let StepUnhandledErrorCode = -101
[<Literal>]
let StepInternalClientErrorCode = -102

let EmptyInfraConfig = ConfigurationBuilder().Build() :> IConfiguration

[<Literal>]
let StatsRounding = 2
[<Literal>]
let DefaultMaxFailCount = 5_000
