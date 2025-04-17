using MQTTnet;
using MQTTnet.Formatter;
using NBomber.Data;
using NBomber.Contracts;
using NBomber.CSharp;
using MqttClient = NBomber.MQTT.MqttClient;

namespace Demo.MQTT.IndependentActors;

public class MqttPublishScenario
{
    public ScenarioProps Create()
    {
        byte[] payload = Data.GenerateRandomBytes(200);
        MqttClient mqttClient = null;

        return Scenario.Create("publish_scenario", async ctx =>
        {
            var publish = await Step.Run("publish", ctx, async () =>
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var topic = $"/clients/independentActors";
                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    // We include the current timestamp so the consumer can calculate the final latency.
                    .WithUserProperty("timestamp", timestamp.ToString())
                    .Build();

                return await mqttClient.Publish(msg);
            });

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Inject(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30))
        )
        .WithInit(async ctx =>
        {
            var clientId = $"mqtt_publisher_{ctx.ScenarioInfo.InstanceId}";
            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(options => { options.WithUri("ws://localhost:8083/mqtt"); })
                .WithClientId(clientId)
                .WithProtocolVersion(MqttProtocolVersion.V500)
                .Build();

            mqttClient = new MqttClient(new MqttClientFactory().CreateMqttClient());
            await mqttClient.Connect(options);
        })
        .WithClean(async ctx =>
        {
            await mqttClient.Disconnect();
        });
    }
}
