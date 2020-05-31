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
let DefaultCopiesCount = 50

[<Literal>]
let DefaultConnectionCount = 50

[<Literal>]
let TryCount = 5

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
let DefaultReportName = "nbomber_report"

//todo: opaque types instead of ms
let DefaultSimulationDuration = TimeSpan.FromMinutes 1.0
let MinSimulationDuration = TimeSpan.FromSeconds 1.0
let MaxSimulationDuration = TimeSpan.FromDays 10.0
let DefaultWarmUpDuration = TimeSpan.FromSeconds 30.0
let MinSendStatsInterval = TimeSpan.FromSeconds 10.0
let SchedulerNotificationTickInterval = TimeSpan.FromSeconds 2.0
let OperationTimeOut = TimeSpan.FromSeconds 3.0
