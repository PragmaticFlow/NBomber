using MQTTnet;
using MQTTnet.Formatter;
using NBomber.Data;
using NBomber.Contracts;
using NBomber.CSharp;
using MqttClient = NBomber.MQTT.MqttClient;
using Microsoft.Extensions.Configuration;

namespace Demo.MQTT.IndependentActors;

public class CustomConsumeScenarioSettings
{
    public string MqttServerUrl { get; set; }
}

public class MqttConsumeScenario
{
    public ScenarioProps Create()
    {
        CustomConsumeScenarioSettings config = null;
        MqttClient mqttClient = null;

        return Scenario.Create("consume_scenario", async ctx =>
        {
            var message = await mqttClient.Receive(ctx.ScenarioCancellationToken);

            // Final latency is computed by subtracting the current time from the timestamp in the header.
            var timestampMs = long.Parse(message.Payload.Value.UserProperties.FirstOrDefault(prop => prop.Name == "timestamp").Value);
            var latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - timestampMs;

            return Response.Ok(customLatencyMs: latency);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(1, TimeSpan.FromSeconds(30))
        )
        .WithInit(async ctx =>
        {
            config = ctx.CustomSettings.Get<CustomConsumeScenarioSettings>();

            var clientId = $"mqtt_consumer";
            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(options => { options.WithUri(config.MqttServerUrl); })
                .WithClientId(clientId)
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
