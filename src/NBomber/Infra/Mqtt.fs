module internal NBomber.Infra.Mqtt

open System
open System.IO
open System.Runtime.Serialization.Formatters.Binary
open System.Threading
open System.Threading.Tasks

open Serilog
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

let connect (clientId: string, mqttServer: string) =
    
    let createMqttClient () =
        let factory = MqttFactory()
        factory.CreateMqttClient()

    let tsc = TaskCompletionSource<IMqttClient>()
    let client = createMqttClient()
    let options = 
        MqttClientOptionsBuilder()
            .WithClientId(clientId)                            
            .WithTcpServer(mqttServer)
            .WithCleanSession()
            .Build()                          

    let reconnect () =
        while not client.IsConnected do
            try
                client.ConnectAsync(options, CancellationToken.None).Wait()
            with
            | _ -> Log.Error("can't connect to the mqtt broker")
            Task.Delay(5_000).Wait()

    client.UseConnectedHandler(fun _ -> 
        Log.Information("connection with mqtt broker is established")
        tsc.TrySetResult(client) |> ignore
    )
    |> ignore

    client.UseDisconnectedHandler(fun args -> 
        match args.Exception with
        | :? OperationCanceledException -> ()
        | _ ->
            if args.ClientWasConnected then
                reconnect()
    )
    |> ignore

    reconnect()

    tsc.Task |> Async.AwaitTask

let publishToBroker (mqttClient: IMqttClient) (msg: MqttApplicationMessage) = async {
    try
        let! result = mqttClient.PublishAsync(msg) |> Async.AwaitTask
        match result.ReasonCode with
        | MqttClientPublishReasonCode.Success -> return Ok()
        | _ -> return AppError.createResult SendMqttMsgFailed
    with
    | e -> return AppError.createResult SendMqttMsgFailed
}