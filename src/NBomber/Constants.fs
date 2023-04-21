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
let DefaultReportingInterval = TimeSpan.FromSeconds 5
let GetPluginStatsTimeout = TimeSpan.FromSeconds 5
let ONE_SECOND = TimeSpan.FromSeconds 1

let MaxTrackableStepLatency = (1000L * TimeSpan.TicksPerMillisecond) * 60L * 10L // 10 min (in ticks)
let MaxTrackableStepResponseSize = Int64.MaxValue

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

module Metrics =

    [<Literal>]
    let DEFAULT_SCALING_FRACTION = 100.0
    [<Literal>]
    let NO_SCALING_FRACTION = 1.0
    [<Literal>]
    let BYTES_TO_MB_SCALING_FRACTION = 1048576.0 // 1024 * 1024
    [<Literal>]
    let CPU_USAGE = "cpu-usage"
    [<Literal>]
    let WORKING_SET = "working-set"
    [<Literal>]
    let GC_HEAP_SIZE = "gc-heap-size"
    [<Literal>]
    let THREAD_POOL_QUEUE_LENGTH = "threadpool-queue-length"
    [<Literal>]
    let THREAD_POOL_QUEUE_COUNT = "threadpool-thread-count"
    [<Literal>]
    let INCOMING_CONNECTIONS_ESTABLISHED = "incoming-connections-established"
    [<Literal>]
    let OUTGOING_CONNECTIONS_ESTABLISHED = "outgoing-connections-established"
    [<Literal>]
    let CURRENT_OUTGOING_CONNECT_ATTEMPTS = "current-outgoing-connect-attempts"
    [<Literal>]
    let DATA_RECEIVED = "data-received"
    [<Literal>]
    let DATA_SENT = "data-sent"
    [<Literal>]
    let BYTES_RECEIVED = "bytes-received"
    [<Literal>]
    let BYTES_SENT = "bytes-sent"

module Reporting =

    [<Literal>]
    let TIMER_START_DELAY = 2_000
    [<Literal>]
    let TIMER_STOP_DELAY = 4_000
