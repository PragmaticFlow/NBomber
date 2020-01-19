module internal NBomber.Infra.Mqtt

open System
open System.IO
open System.Runtime.Serialization.Formatters.Binary
open System.Threading

open FSharp.Control.Tasks.V2.ContextInsensitive
open MQTTnet
open MQTTnet.Client
open MQTTnet.Client.Options
open MQTTnet.Client.Publishing

open NBomber.Errors

let serialize (data: obj) =
    use ms = new MemoryStream()
    let formatter = BinaryFormatter()
    formatter.Serialize(ms, data)
    ms.ToArray()

let deserialize<'T> (data: byte[]) =
    use ms = new MemoryStream(data)
    let formatter = BinaryFormatter()
    formatter.Deserialize(ms) :?> 'T

let toMqttMsg (topic: string) (data: obj) =
    let bytes = serialize data
    MqttApplicationMessageBuilder()
        .WithTopic(topic)
        .WithPayload(bytes)
        .Build()

let reconnect (client: IMqttClient, options: IMqttClientOptions option, logger: Serilog.ILogger) = task {
    while not client.IsConnected do
        try
            if options.IsSome then
                let! result = client.ConnectAsync(options.Value, CancellationToken.None)
                ignore result
            else
                do! client.ReconnectAsync()

            logger.Information("connection with mqtt broker is established")
        with
        | ex -> logger.Error(ex, "can't connect to the mqtt broker")

    return client
}

let initClient (clientId: string, mqttServer: string,
                mqttPort: int option, logger: Serilog.ILogger) =

    let createMqttClient () =
        let factory = MqttFactory()
        factory.CreateMqttClient()

    let client = createMqttClient()
    let options =
        MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(mqttServer, Option.toNullable(mqttPort))
            .WithCleanSession()
            .WithCommunicationTimeout(TimeSpan.FromSeconds(5.0))
            .Build()
            |> Some

    reconnect(client, options, logger) |> Async.AwaitTask

let publishToBroker (mqttClient: IMqttClient) (msg: MqttApplicationMessage) = async {
    try
        let! result = mqttClient.PublishAsync(msg) |> Async.AwaitTask
        match result.ReasonCode with
        | MqttClientPublishReasonCode.Success -> return Ok()
        | _ -> return AppError.createResult SendMqttMsgFailed
    with
    | e -> return AppError.createResult SendMqttMsgFailed
}
