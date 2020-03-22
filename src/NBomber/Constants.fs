module internal NBomber.Constants

open NBomber.Configuration

[<Literal>]
let DefaultScenarioDurationInSec = 60.0

[<Literal>]
let DefaultConcurrentCopiesCount = 50

[<Literal>]
let DefaultWarmUpDurationInSec = 10.0

[<Literal>]
let DefaultRepeatCount = 0

[<Literal>]
let DefaultDoNotTrack = false

let AllReportFormats = [ReportFormat.Txt; ReportFormat.Html; ReportFormat.Csv; ReportFormat.Md]

[<Literal>]
let StepResponseKey = "nbomber_step_response"

[<Literal>]
let EmptyPoolName = "nbomber_empty_pool"

[<Literal>]
let EmptyFeedName = "nbomber_empty_feed"

[<Literal>]
let DefaultTestSuite = "nbomber_default_test_suite_name"

[<Literal>]
let DefaultTestName = "nbomber_default_test_name"

[<Literal>]
let MinSendStatsIntervalSec = 10.0

//todo: opaque types instead of ms

[<Literal>]
let SchedulerTickIntervalMs = 250

[<Literal>]
let NotificationTickIntervalMs = 3_000

[<Literal>]
let ReTryCount = 5

[<Literal>]
let OperationTimeOut = 3_000
