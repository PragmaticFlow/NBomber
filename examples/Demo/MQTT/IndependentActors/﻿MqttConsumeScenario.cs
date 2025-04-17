using MQTTnet;
using MQTTnet.Formatter;
using NBomber.Data;
using NBomber.Contracts;
using NBomber.CSharp;
using MqttClient = NBomber.MQTT.MqttClient;

namespace Demo.MQTT.IndependentActors;

public class MqttConsumeScenario
{
    public ScenarioProps Create()
    {
        byte[] payload = Data.GenerateRandomBytes(200);
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
            var clientId = $"mqtt_consumer";
            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(options => { options.WithUri("ws://localhost:8083/mqtt"); })
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
