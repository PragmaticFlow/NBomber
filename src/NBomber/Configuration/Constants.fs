module internal NBomber.Constants

open System
open Microsoft.Extensions.Configuration
open NBomber.Configuration

[<Literal>]
let Logo = "NBomber"

[<Literal>]
let NBomberWelcomeText = "NBomber '{0}' Started a new session: '{1}'"

[<Literal>]
let DefaultCopiesCount = 50

[<Literal>]
let DefaultConnectionCount = 50

[<Literal>]
let TryCount = 5

[<Literal>]
let DefaultDoNotTrack = false

let AllReportFormats = [ReportFormat.Txt; ReportFormat.Html; ReportFormat.Csv; ReportFormat.Md]

[<Literal>]
let StepResponseKey = "nbomber_step_response"

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
let SchedulerNotificationTickInterval = TimeSpan.FromSeconds 2.0
let OperationTimeOut = TimeSpan.FromSeconds 3.0

let EmptyInfraConfig = ConfigurationBuilder().Build() :> IConfiguration
