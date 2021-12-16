module internal NBomber.Constants

open System
open Microsoft.Extensions.Configuration
open NBomber.Contracts.Stats

[<Literal>]
let Logo = "NBomber"

[<Literal>]
let NBomberWelcomeText = "NBomber '{0}' Started a new session: '{1}'."

[<Literal>]
let DefaultCopiesCount = 1

[<Literal>]
let DefaultClientCount = 1

[<Literal>]
let TryCount = 5

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

//todo: opaque types instead of ms
let DefaultSimulationDuration = TimeSpan.FromMinutes 1.0
let MinSimulationDuration = TimeSpan.FromSeconds 1.0
let MaxSimulationDuration = TimeSpan.FromDays 10.0
let DefaultWarmUpDuration = TimeSpan.FromSeconds 30.0
let MinSendStatsInterval = TimeSpan.FromSeconds 5.0
let DefaultSendStatsInterval = TimeSpan.FromSeconds 10.0
let StepTimeout = TimeSpan.FromSeconds 1.0
let GetPluginStatsTimeout = TimeSpan.FromSeconds 5.0

[<Literal>]
let SchedulerTickIntervalMs = 1_000.0

[<Literal>]
let TimeoutStatusCode = -100

[<Literal>]
let StepExceptionStatusCode = -101

let EmptyInfraConfig = ConfigurationBuilder().Build() :> IConfiguration

let MaxTrackableStepLatency = (1000L * TimeSpan.TicksPerMillisecond) * 60L * 10L // 10 min (in ticks)
let MaxTrackableStepResponseSize = int64 Int32.MaxValue

[<Literal>]
let StatsRounding = 2

[<Literal>]
let ResponseBufferLength = 50

[<Literal>]
let ResponseBufferFlushDelaySec = 1
