using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using NBomber;
using NBomber.CSharp;
using NBomber.Data;
using MqttClient = NBomber.MQTT.MqttClient;

namespace Demo.MQTT.ClientPool;

public class CustomScenarioSettings
{
    public string MqttServerUrl { get; set; }
    public int ClientCount { get; set; }
    public int MsgSizeBytes { get; set; }
}

public class ClientPoolMqttExample
{
    public void Run()
    {
        var clientPool = new ClientPool<MqttClient>();
        var message = Array.Empty<byte>();

        var scenario = Scenario.Create("mqtt_scenario", async ctx =>
        {
            var client = clientPool.GetClient(ctx.ScenarioInfo);

            var publish = await Step.Run("publish", ctx, async () =>
            {
                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic(client.Client.Options.ClientId)
                    .WithPayload(message)
                    .Build();

                var response = await client.Publish(msg);
                return response;
            });

            var receive = await Step.Run("receive", ctx, async () =>
            {
                var response = await client.Receive();
                return response;
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

            var counter = 0;
            for (var i = 0; i < config.ClientCount; i++)
            {
                counter++;

                var client = new MqttClient(mqttFactory.CreateMqttClient());
                var clientOptions = new MqttClientOptionsBuilder()
                    .WithWebSocketServer(optionsBuilder => optionsBuilder.WithUri(config.MqttServerUrl))
                    .WithCleanSession()
                    .WithClientId($"client_{i}")
                    .Build();

                var result = await client.Connect(clientOptions);

                if (!result.IsError)
                {
                    await client.Subscribe(client.Client.Options.ClientId);
                    clientPool.AddClient(client);
                }
                else
                    throw new Exception("client can't connect to the MQTT broker");

                if (counter == 10)
                {
                    counter = 0;
                    await Task.Delay(500); // pause, to do not overload MQTT broker
                }
            }
        })
        .WithClean(ctx =>
        {
            clientPool.DisposeClients(client => client.Disconnect().Wait());
            return Task.CompletedTask;
        });

        NBomberRunner
            .RegisterScenarios(scenario)
            .LoadConfig("./MQTT/ClientPool/config.json")
            .Run();

    }
}
