using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using NBomber;
using NBomber.CSharp;
using NBomber.Data;
// ReSharper disable CheckNamespace

namespace CSharpProd.Features;

public class CustomScenarioSettings
{
    public string MqttServerUrl { get; set; }
    public int ClientCount { get; set; }
    public int MsgSizeBytes { get; set; }
}

public class ClientPoolExample
{
    public void Run()
    {
        var clientPool = new ClientPool<IMqttClient>();
        var responsePromises = new ConcurrentDictionary<IMqttClient, TaskCompletionSource<MqttApplicationMessage>>();
        var message = Array.Empty<byte>();

        var scenario = Scenario.Create("scenario", async ctx =>
        {
            var client = clientPool.GetClient(ctx.ScenarioInfo);
            var promise = responsePromises[client];

            var publish = await Step.Run("publish", ctx, async () =>
            {
                await client.PublishAsync(client.Options.ClientId, message);
                return Response.Ok(sizeBytes: message.Length);
            });

            var receive = await Step.Run("receive", ctx, async () =>
            {
                var response = await promise.Task;
                return Response.Ok(sizeBytes: response.Payload.Length);
            });

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30)))
        .WithInit(async context =>
        {
            var config = context.CustomSettings.Get<CustomScenarioSettings>();
            message = Data.GenerateRandomBytes(config.MsgSizeBytes);

            var mqttFactory = new MqttFactory();

            for (var i = 0; i < config.ClientCount; i++)
            {
                var client = mqttFactory.CreateMqttClient();
                var clientOptions = new MqttClientOptionsBuilder()
                    .WithWebSocketServer(config.MqttServerUrl)
                    .WithCleanSession()
                    .WithClientId($"client_{i}")
                    .Build();

                var result = await client.ConnectAsync(clientOptions);

                if (result.ResultCode == MqttClientConnectResultCode.Success)
                {
                    // register client and push response promise
                    responsePromises[client] = new TaskCompletionSource<MqttApplicationMessage>();

                    client.UseApplicationMessageReceivedHandler(msg =>
                    {
                        var promise = responsePromises[client];
                        responsePromises[client] = new TaskCompletionSource<MqttApplicationMessage>(); // set new promise
                        promise.TrySetResult(msg.ApplicationMessage);
                    });

                    await client.SubscribeAsync(client.Options.ClientId);

                    clientPool.AddClient(client);
                }
                else
                    throw new Exception("client can't connect to the MQTT broker");
            }
        })
        .WithClean(context =>
        {
            clientPool.DisposeClients(client => client.DisconnectAsync().Wait());
            return Task.CompletedTask;
        });

        NBomberRunner
            .RegisterScenarios(scenario)
            .LoadConfig("./Features/ClientPool/config.json")
            .Run();

    }
}
