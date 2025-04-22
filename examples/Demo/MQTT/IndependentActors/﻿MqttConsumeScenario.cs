using MQTTnet;
using MQTTnet.Formatter;
using NBomber.Contracts;
using NBomber.CSharp;
using MqttClient = NBomber.MQTT.MqttClient;
using Microsoft.Extensions.Configuration;

namespace Demo.MQTT.IndependentActors;

public class MqttConsumeScenario
{
    public ScenarioProps Create()
    {
        MqttClient mqttClient = null;

        return Scenario.Create("consume_scenario", async ctx =>
        {
            var message = await mqttClient.Receive(ctx.ScenarioCancellationToken);

            // Final latency is computed by subtracting the current time from the timestamp in the header.
            var timestamp = message.Payload.Value.UserProperties.First(prop => prop.Name == "timestamp").Value;
            var timestampMs = long.Parse(timestamp);
            var latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - timestampMs;

            return Response.Ok(customLatencyMs: latency, sizeBytes: message.SizeBytes);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(1, TimeSpan.FromSeconds(30))
        )
        .WithInit(async ctx =>
        {
            var config = ctx.GlobalCustomSettings.Get<MqttCustomSettings>();

            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(options => { options.WithUri(config.MqttServerUrl); })
                .WithClientId("mqtt_consumer")
                .WithProtocolVersion(MqttProtocolVersion.V500)
                .Build();

            mqttClient = new MqttClient(new MqttClientFactory().CreateMqttClient());
            await mqttClient.Connect(options);

            await mqttClient.Subscribe($"/clients/independentActors");
        })
        .WithClean(async ctx =>
        {
            await mqttClient.Disconnect();
        });
    }
}
