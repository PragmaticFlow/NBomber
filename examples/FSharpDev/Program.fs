// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open FSharpDev.ClientFactory
open FSharpDev.HelloWorld
open FSharpDev.DataFeed
open FSharpDev.HttpTests
open FSharpDev.InfluxDbSink
open FSharpDev.MqttTests

[<EntryPoint>]
let main argv =

    //HelloWorldExample.run()
    //CustomSettingsExample.run()
    //DataFeedTest.run()
    //SimpleHttpTest.run()
    HttpClientFactoryExample.run()
    //MqttScenario.run()
    //InfluxDbReportingScenario.run()

    0 // return an integer exit code
