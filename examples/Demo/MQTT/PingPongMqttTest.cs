using MQTTnet;
using NBomber.CSharp;
using NBomber.Data;
using MqttClient = NBomber.MQTT.MqttClient;

namespace Demo.MQTT;

public class PingPongMqttTest
{
    public void Run()
    {
        var payload = Data.GenerateRandomBytes(200);

        var scenario = Scenario.Create("mqtt_scenario", async ctx =>
        {
            var topic = $"/clients/{ctx.ScenarioInfo.InstanceId}";
            var mqttClient = new MqttClient(new MqttClientFactory().CreateMqttClient());

            var connect = await Step.Run("connect", ctx, async () =>
            {
                var options = new MqttClientOptionsBuilder()
                    .WithWebSocketServer(options => { options.WithUri("ws://localhost:8083/mqtt"); })
                    .Build();

                return await mqttClient.Connect(options);
            });

            var subscribe = await Step.Run("subscribe", ctx, async () =>
                await mqttClient.Subscribe(topic));

            var publish = await Step.Run("publish", ctx, async () =>
            {
                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .Build();

                return await mqttClient.Publish(msg);
            });

            var receive = await Step.Run("receive", ctx, async () =>
                await mqttClient.Receive(ctx.ScenarioCancellationToken));

            var disconnect = await Step.Run("disconnect", ctx, async () =>
                await mqttClient.Disconnect());

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(1, TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
