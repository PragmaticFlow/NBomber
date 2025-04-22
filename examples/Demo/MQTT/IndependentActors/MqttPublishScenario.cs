using MQTTnet;
using MQTTnet.Formatter;
using Microsoft.Extensions.Configuration;
using NBomber.Data;
using NBomber.Contracts;
using NBomber.CSharp;
using MqttClient = NBomber.MQTT.MqttClient;

namespace Demo.MQTT.IndependentActors;

public class MqttPublishScenario
{
    public ScenarioProps Create()
    {
        byte[] payload = [];
        MqttClient mqttClient = null;

        return Scenario.Create("publish_scenario", async ctx =>
        {
            var publish = await Step.Run("publish", ctx, async () =>
            {
                // We include the current timestamp so that the consumer can calculate the final latency.
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic("/clients/independentActors")
                    .WithPayload(payload)
                    .WithUserProperty("timestamp", timestamp.ToString())
                    .Build();

                return await mqttClient.Publish(msg);
            });

            return Response.Ok();
        })
        .WithInit(async ctx =>
        {
            var config = ctx.GlobalCustomSettings.Get<MqttCustomSettings>();
            payload = Data.GenerateRandomBytes(config.MsgSizeBytes);

            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(options => { options.WithUri(config.MqttServerUrl); })
                .WithClientId("mqtt_publisher")
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
