
using MQTTnet;
using MQTTnet.Client;
using NBomber.CSharp;
using NBomber.Data;
using MqttClient = NBomber.MQTT.MqttClient;


namespace Demo.MQTT;

public class PingPongMqttTest
{
    public void Run()
    {
        var payload = Data.GenerateRandomBytes(200);

        var scenario = Scenario.Create("ping_pong_mqtt_scenario", async ctx =>
        {
            using var mqttClient = new MqttClient(new MqttFactory().CreateMqttClient());
            var topic = $"/clients/{ctx.ScenarioInfo.ThreadId}";

            var connect = await Step.Run("connect", ctx, async () =>
            {
                var clientOptions = new MqttClientOptionsBuilder()
                    .WithWebSocketServer(optionsBuilder => optionsBuilder.WithUri("ws://localhost:8083/mqtt"))
                    .WithCleanSession()
                    .WithClientId($"client_{ctx.ScenarioInfo.ThreadId}")
                    .Build();

                return await mqttClient.Connect(clientOptions);
            });

            var subscribe = await Step.Run("subscribe", ctx, async () =>
                await mqttClient.Subscribe(topic));

            var publish = await Step.Run("publish", ctx, async () =>
            {
                var msg = new MqttApplicationMessageBuilder().WithTopic(topic)
                    .WithPayload(payload).Build();

                return await mqttClient.Publish(msg);
            });

            var receive = await Step.Run("receive", ctx, async () =>
                await mqttClient.Receive());

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
