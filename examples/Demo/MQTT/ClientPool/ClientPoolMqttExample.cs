using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Protocol;
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
    // For this example, please spin up local MQTT broker via docker-compose.yml located in the MQTT folder.

    public void Run()
    {
        var clientPool = new ClientPool<MqttClient>();
        byte[] payload = [];

        var scenario = Scenario.Create("mqtt_scenario", async ctx =>
        {
            // get a client from the pool by Scenario InstanceID
            var mqttClient = clientPool.GetClient(ctx.ScenarioInfo.InstanceNumber);

            var publish = await Step.Run("publish", ctx, async () =>
            {
                var topic = $"/clients/{ctx.ScenarioInfo.InstanceId}";
                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                    .Build();

                return await mqttClient.Publish(msg);
            });

            var receive = await Step.Run("receive", ctx, async () =>
            {
                var response = await mqttClient.Receive(ctx.ScenarioCancellationToken);
                return response;
            });

            return Response.Ok();
        })
        .WithInit(async context =>
        {
            var config = context.CustomSettings.Get<CustomScenarioSettings>();
            payload = Data.GenerateRandomBytes(config.MsgSizeBytes);

            for (var i = 0; i < config.ClientCount; i++)
            {
                var topic = $"/clients/mqtt_scenario_{i}";
                var clientId = $"mqtt_client_{i}";
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer(config.MqttServerUrl)
                    .WithClientId(clientId)
                    .Build();

                var mqttClient = new MqttClient(new MqttClientFactory().CreateMqttClient());
                var connectResult = await mqttClient.Connect(options);

                if (!connectResult.IsError)
                {
                    await mqttClient.Subscribe(topic, MqttQualityOfServiceLevel.AtMostOnce);
                    clientPool.AddClient(mqttClient);
                }
                else
                    throw new Exception("client can't connect to the MQTT broker");

                await Task.Delay(10);
            }
        })
        .WithClean(ctx =>
        {
            clientPool.DisposeClients(client => client.Dispose());
            return Task.CompletedTask;
        });

        NBomberRunner
            .RegisterScenarios(scenario)
            .LoadConfig("./MQTT/ClientPool/config.json")
            .Run();
    }
}
