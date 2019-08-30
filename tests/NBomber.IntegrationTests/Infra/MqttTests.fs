module Tests.MqttTests

open Xunit
open Swensen.Unquote
open Serilog.Events
open Serilog.Sinks.InMemory
open MQTTnet
open MQTTnet.Server
open FsToolkit.ErrorHandling

open NBomber.Infra

let initTestMqttServer () =
    let factory = MqttFactory()
    let server = factory.CreateMqttServer()    
    let serverOptions = MqttServerOptionsBuilder().Build()
    server.StartAsync(serverOptions).Wait()
    server

[<Fact>]
let ``Mqtt.connect client should reconnect automatically`` () = async {
    
    Dependency.Logger.initLogger(Dependency.ApplicationType.Test, None)
    
    // init mqtt client which can't connect since server is down        
    let task = Mqtt.connect("clientId", "localhost")
    
    do! Async.Sleep(5_000)
    
    // now client should be reconnected automatically
    let server = initTestMqttServer()
    let! client = task

    let reconnectCount =
        InMemorySink.Instance.LogEvents
        |> Seq.filter(fun x -> x.Level = LogEventLevel.Error
                               && x.MessageTemplate.Text.Contains("can't connect to the mqtt broker"))
        |> Seq.length

    let successfulConnect =
        InMemorySink.Instance.LogEvents
        |> Seq.exists(fun x -> x.Level = LogEventLevel.Information
                               && x.MessageTemplate.Text.Contains("connection with mqtt broker is established"))
    
    let clientConnected = client.IsConnected
    do! server.StopAsync() |> Async.AwaitTask
    
    test <@ reconnectCount >= 1 @>
    test <@ successfulConnect = true @>
    test <@ clientConnected = true @>
}

[<Fact>]
let ``Mqtt.publishToBroker should return error in case of server is down`` () = async {
    
    Dependency.Logger.initLogger(Dependency.ApplicationType.Test, None)
    let server = initTestMqttServer()
    let! client = Mqtt.connect("clientId", "localhost")
    
    // stopping server to prevent client send a message
    do! server.StopAsync() |> Async.AwaitTask
    let msg = Mqtt.toMqttMsg "testTopic" 777
    let! response = Mqtt.publishToBroker client msg
    
    test <@ Result.isError response @> 
}
