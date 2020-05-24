module BuilderExample

open System.Threading.Tasks
open NBomber.Configuration
open NBomber.Contracts
open NBomber.FSharp
open FSharp.Control.Tasks.V2.ContextInsensitive


let seconds = float >> System.TimeSpan.FromSeconds
let minutes = float >> System.TimeSpan.FromMinutes
let connectionPool = Unchecked.defaultof<IConnectionPoolArgs<System.Net.Http.HttpClient>>
let sink = Unchecked.defaultof<IReportingSink>

let test =
    perftest {
        name "test name"
        testSuite "test suite name"

        applicationType ApplicationType.Process
        applicationType ApplicationType.Console
        runProcess
        runConsole

        config "path/to/config/file"
        infraConfig "path/to/config/file"

        reports [
            ReportFormat.Html
            ReportFormat.Txt
        ]
        noReports
        reportFileName "see_results"
        reportInterval (seconds 5.0)
        reportSink sink

        scenarios [
            scenario "pauses" {
                pause 1000
                pause (seconds 1.0)
                pause (fun _ -> 1000)
                pause (fun _ -> seconds 1.0)
            }

            scenario "simple step overloads for different action types" {
                stepSimple "Async<unit>" (fun _ -> async {
                    do! Async.Sleep 1000
                })
                stepSimple "Async<Response>" (fun _ -> async {
                    return Response.Ok()
                })

                stepSimple "Task" (fun _ -> Task.CompletedTask)
                stepSimple "Task<Response>" (fun _ -> Task.FromResult(Response.Ok()))
                stepSimple "Task<unit>" (fun _ -> Task.FromResult())
            }

            scenario "step overloads for different action types and with connection pool" {
                step "Async<unit>" connectionPool (fun _ -> async {
                    do! Async.Sleep 1000
                })
                step "Async<Response>" connectionPool (fun _ -> async {
                    return Response.Ok()
                })
                step "Task" connectionPool (fun _ -> Task.CompletedTask)
                step "Task<Response>" connectionPool (fun _ -> Task.FromResult(Response.Ok()))
                step "Task<unit>" connectionPool (fun _ -> Task.FromResult())
                step "Task<unit>" connectionPool (fun _ -> Task.FromResult())
            }

            scenario "scenario configuration" {
                noWarmUp
                warmUp (seconds 5.0)
                load [
                    KeepConcurrentScenarios(100, seconds 10.0)
                    InjectScenariosPerSec(10, minutes 3.0)
                ]

                testInit  (fun _ -> Task.FromResult())
                testInit  (fun _ -> Task.CompletedTask)
                testClean (fun _ -> Task.FromResult())
                testClean (fun _ -> Task.CompletedTask)
                step "get homepage" connectionPool (fun ctx -> task {
                    use! response = ctx.Connection.GetAsync("https://nbomber.com", ctx.CancellationToken)

                    match response.IsSuccessStatusCode with
                    | true  -> let size = int response.Content.Headers.ContentLength.Value
                               return Response.Ok(sizeBytes = size)
                    | false -> return Response.Fail()
                })
            }
        ]
    }
