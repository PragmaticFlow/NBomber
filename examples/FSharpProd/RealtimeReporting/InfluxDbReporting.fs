module FSharpProd.RealtimeReporting.InfluxDbReporting

open System.Threading.Tasks
open FSharp.Control.Tasks.NonAffine
open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Sinks.InfluxDB

let run () =

    let step = Step.create("step_1", fun context -> task {

        do! Task.Delay(seconds 1)

        // this message will be saved to elastic search
        context.Logger.Debug("hello from NBomber")

        return Response.ok()
    })

    let influxConfig = InfluxDbSinkConfig.create(url = "http://localhost:8086", database = "default")
    use influxDb = new InfluxDBSink(influxConfig)

    Scenario.create "hello_world_scenario" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = minutes 1)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "reporting"
    |> NBomberRunner.withTestName "influx_test"
    |> NBomberRunner.withReportingSinks [influxDb] //TODO???:  (seconds 10)
    |> NBomberRunner.run
    |> ignore
