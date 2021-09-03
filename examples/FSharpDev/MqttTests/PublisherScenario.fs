module FSharpDev.MqttTests.PublisherScenario

open System
open System.Text
open System.Threading

open MQTTnet
open MQTTnet.Client.Connecting
open MQTTnet.Client.Options
open MQTTnet.Client.Publishing
open FSharp.Control.Tasks.NonAffine
open Newtonsoft.Json

open NBomber
open NBomber.Contracts
open NBomber.FSharp

[<CLIMutable>]
type Message = {
    Number: int
    PublishTime: DateTime
}

let private createMqttFactory () =
    ClientFactory.create(
        name = "mqtt_publisher_factory",
        initClient = fun (i, context) -> task {
            let clientId = $"publisher_{i}"
            let mqttFactory = MqttFactory()

            let clientOptions =
                MqttClientOptionsBuilder()
                    .WithTcpServer("localhost")
                    .WithCleanSession()
                    .WithClientId(clientId)
                    .Build();

            let client = mqttFactory.CreateMqttClient()
            let! result = client.ConnectAsync(clientOptions, CancellationToken.None)

            if result.ResultCode <> MqttClientConnectResultCode.Success then
                failwith $"MQTT connection code is: {result.ResultCode}, reason: {result.ReasonString}"

            return client
        }
    )

let create () =

    let mqttClientFactory = createMqttFactory()

    let step = Step.create("publish_step",
                           clientFactory = mqttClientFactory,
                           execute = fun context -> task {

        let msg = { Number = context.InvocationCount; PublishTime = DateTime.UtcNow }
        let payload = msg |> JsonConvert.SerializeObject |> Encoding.ASCII.GetBytes
        let mqttMsg = MqttApplicationMessage(Topic = "test_topic", Payload = payload)
        let! result = context.Client.PublishAsync(mqttMsg, context.CancellationToken)

        return
            if result.ReasonCode = MqttClientPublishReasonCode.Success then Response.ok()
            else Response.fail(statusCode = int result.ReasonCode)
    })

    let pause = Step.createPause(seconds 0.5)

    Scenario.create "mqtt_publisher_scenario" [step; pause]
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 10)]
    |> Scenario.withoutWarmUp
