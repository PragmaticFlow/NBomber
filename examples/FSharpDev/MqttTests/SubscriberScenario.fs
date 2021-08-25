module FSharpDev.MqttTests.SubscriberScenario

open System
open System.Text
open System.Threading
open System.Threading.Tasks

open MQTTnet
open MQTTnet.Client
open MQTTnet.Client.Connecting
open MQTTnet.Client.Options
open FSharp.Control.Tasks.NonAffine
open Newtonsoft.Json

open NBomber
open NBomber.Contracts
open NBomber.Extensions.PushExtensions
open NBomber.FSharp

[<CLIMutable>]
type Message = {
    Number: int
    PublishTime: DateTime
}

let private _responseBuffer = new PushResponseBuffer()

let private createMqttFactory (clientCount) =
    ClientFactory.create(
        name = "mqtt_subscriber_factory",
        initClient = fun (i, ctx) -> task {
            let clientId = $"subscriber_{i}"
            let mqttFactory = MqttFactory()

            let clientOptions =
                MqttClientOptionsBuilder()
                    .WithTcpServer("localhost")
                    .WithCleanSession()
                    .WithClientId(clientId)
                    .Build();

            let client = mqttFactory.CreateMqttClient()

            client.UseApplicationMessageReceivedHandler(fun msg ->
                _responseBuffer.AddResponse(clientId, msg.ApplicationMessage.Payload)
                Task.CompletedTask
            )
            |> ignore

            let! result = client.ConnectAsync(clientOptions, CancellationToken.None)
            let! subscribeResult = client.SubscribeAsync("test_topic")

            if result.ResultCode = MqttClientConnectResultCode.Success then
                _responseBuffer.InitBufferForClient(clientId)
            else
                failwith $"MQTT connection code is: {result.ResultCode}, reason: {result.ReasonString}"

            return client
        },
        clientCount = clientCount
    )

let create () =

    let clientCount = 1
    let mqttClientFactory = createMqttFactory(clientCount)

    let step = Step.create("receive_step",
                           clientFactory = mqttClientFactory,
                           execute = fun context -> task {

        let clientId = context.Client.Options.ClientId
        let! response = _responseBuffer.ReceiveResponse(clientId)

        let msg =
            response.Payload :?> byte[]
            |> Encoding.ASCII.GetString
            |> JsonConvert.DeserializeObject<Message>

        let latency = response.ReceivedTime - msg.PublishTime

        return Response.ok(latencyMs = latency.TotalMilliseconds)
    })

    Scenario.create "mqtt_subscriber_scenario" [step]
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 10)]
    |> Scenario.withoutWarmUp
