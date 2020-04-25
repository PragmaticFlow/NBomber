module internal NBomber.Constants

open System
open NBomber.Configuration

[<Literal>]
let NBomberWelcomeText = """
//        __   __         __   ___  __
//  |\ | |__) /  \  |\/| |__) |__  |__)
//  | \| |__) \__/  |  | |__) |___ |  \  '{0}' started a new session: '{1}'
//
"""

[<Literal>]
let DefaultScenarioDurationInSec = 60.0

[<Literal>]
let DefaultWarmUpDurationInSec = 10.0

[<Literal>]
let DefaultConcurrentCopiesCount = 50

[<Literal>]
let DefaultRepeatCount = 0

[<Literal>]
let DefaultDoNotTrack = false

let AllReportFormats = [ReportFormat.Txt; ReportFormat.Html; ReportFormat.Csv]

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
let SchedulerNotificationTickIntervalMs = 2_000.0

[<Literal>]
let TryCount = 5

[<Literal>]
let OperationTimeOutMs = 3_000
