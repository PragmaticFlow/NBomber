using MQTTnet;
using NBomber.CSharp;
using NBomber.Data;
using MqttClient = NBomber.MQTT.MqttClient;

namespace Demo.MQTT;

public class PingPongMqttTest
{
    // For this example, please spin up local MQTT broker via docker-compose.yml located in the MQTT folder.

    public void Run()
    {
        var payload = Data.GenerateRandomBytes(200);

        var scenario = Scenario.Create("mqtt_scenario", async ctx =>
        {
            var topic = $"/clients/{ctx.ScenarioInfo.InstanceId}";
            using var mqttClient = new MqttClient(new MqttClientFactory().CreateMqttClient());

            var connect = await Step.Run("connect", ctx, async () =>
            {
                var options = new MqttClientOptionsBuilder()
                    .WithWebSocketServer(options => { options.WithUri("ws://localhost:8083/mqtt"); })
                    .Build();

                return await mqttClient.Connect(options);
            });

            var subscribe = await Step.Run("subscribe", ctx, async () =>
            {
                var response = await mqttClient.Subscribe(topic);
                return response;
            });

            var publish = await Step.Run("publish", ctx, async () =>
            {
                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .Build();

                return await mqttClient.Publish(msg);
            });

            var receive = await Step.Run("receive", ctx, async () =>
            {
                // pass the ScenarioCancellationToken to stop waiting for a response if the scenario finish event is triggered
                var response = await mqttClient.Receive(ctx.ScenarioCancellationToken);
                return response;
            });

            var disconnect = await Step.Run("disconnect", ctx, async () =>
            {
                var response = await mqttClient.Disconnect();
                return response;
            });

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
