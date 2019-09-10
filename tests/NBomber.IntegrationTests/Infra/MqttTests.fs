module Tests.MqttTests

open Xunit
open Swensen.Unquote
open MQTTnet
open MQTTnet.Server
open FsToolkit.ErrorHandling

open NBomber.Infra

let startMqttServer () =
    let factory = MqttFactory()
    let server = factory.CreateMqttServer()    
    let serverOptions = MqttServerOptionsBuilder().Build()
    server.StartAsync(serverOptions).Wait()
    server
    
let stopMqttServer (server: IMqttServer) =
    server.StopAsync().Wait()    

[<Fact>]
let ``Mqtt.publishToBroker should return error in case of server is down`` () = async {
        
    let server = startMqttServer()
    let! client = Mqtt.initClient("clientId", "localhost")
    
    // stopping server to disconnect client and prevent to send a message
    server.StopAsync().Wait()    
    
    // waiting on client disconnect
    while client.IsConnected do
        do! Async.Sleep(100)
    
    let msg = Mqtt.toMqttMsg "testTopic" 777
    let! response = Mqtt.publishToBroker client msg
    
    test <@ Result.isError response @> 
}
