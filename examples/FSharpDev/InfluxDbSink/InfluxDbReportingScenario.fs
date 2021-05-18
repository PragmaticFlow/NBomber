module FSharpDev.InfluxDbSink.InfluxDbReportingScenario

open System.Threading.Tasks
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open FSharpDev.InfluxDbSink

let run () =

    let config = InfluxDbSinkConfig.create("http://localhost:8086", "default")
    use influxDb = new InfluxDBSink(config)

    let okStep = Step.create("ok_step", fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(sizeBytes = 100)
    })

    let failStep = Step.create("fail_step", fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.fail(sizeBytes = 200)
    })

    Scenario.create "simple_scenario" [okStep; failStep]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = minutes 1)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "reporting"
    |> NBomberRunner.withTestName "influx_db_test"
    |> NBomberRunner.withReportingSinks [influxDb]
    |> NBomberRunner.run
    |> ignore
