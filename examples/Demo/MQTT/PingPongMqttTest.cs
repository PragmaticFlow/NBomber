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
            using var client = new MqttClient(new MqttFactory().CreateMqttClient());
            var topic = $"/clients/{ctx.ScenarioInfo.InstanceId}";

            var connect = await Step.Run("connect", ctx, async () =>
            {
                var clientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("localhost")
                    //.WithWebSocketServer(options => options.WithUri("ws://localhost:8083/mqtt"))
                    .WithCleanSession()
                    .WithClientId($"client_{ctx.ScenarioInfo.InstanceId}")
                    .Build();

                var response = await client.Connect(clientOptions);
                return response;
            });

            var subscribe = await Step.Run("subscribe", ctx, () => client.Subscribe(topic));

            var publish = await Step.Run("publish", ctx, async () =>
            {
                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .Build();

                var response = await client.Publish(msg);
                return response;
            });

            var receive = await Step.Run("receive", ctx, async () =>
            {
                var response = await client.Receive();
                return response;
            });

            var disconnect = await Step.Run("disconnect", ctx, () => client.Disconnect());

            return Response.Ok();
        })
        .WithWarmUpDuration(TimeSpan.FromSeconds(3))
        .WithLoadSimulations(
            Simulation.KeepConstant(1, TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
