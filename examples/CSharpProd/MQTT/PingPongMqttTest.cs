using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using NBomber.CSharp;
using NBomber.Data;

namespace CSharpProd.MQTT;

public class PingPongMqttTest
{
    public void Run()
    {
        var payload = Data.GenerateRandomBytes(200);

        var scenario = Scenario.Create("ping_pong_mqtt_scenario", async ctx =>
        {
            using var mqttClient = new MqttFactory().CreateMqttClient();
            var topic = $"/clients/{ctx.ScenarioInfo.ThreadId}";
            var promise = new TaskCompletionSource<MqttApplicationMessage>();

            var connect = await Step.Run("connect", ctx, async () =>
            {
                var clientOptions = new MqttClientOptionsBuilder()
                    .WithWebSocketServer("ws://localhost:8083/mqtt")
                    .WithCleanSession()
                    .WithClientId($"client_{ctx.ScenarioInfo.ThreadId}")
                    .Build();

                var result = await mqttClient.ConnectAsync(clientOptions);
                return result.ResultCode == MqttClientConnectResultCode.Success
                    ? Response.Ok()
                    : Response.Fail(
                        statusCode: MqttClientConnectResultCode.Success.ToString(),
                        message: $"MQTT connection code is: {result.ResultCode}, reason: {result.ReasonString}"
                    );
            });

            var subscribe = await Step.Run("subscribe", ctx, async () =>
            {
                mqttClient.UseApplicationMessageReceivedHandler(msg =>
                {
                    promise.TrySetResult(msg.ApplicationMessage);
                });

                await mqttClient.SubscribeAsync(topic);

                return Response.Ok();
            });

            var publish = await Step.Run("publish", ctx, async () =>
            {
                await mqttClient.PublishAsync(topic, payload);
                return Response.Ok(sizeBytes: payload.Length);
            });

            var receive = await Step.Run("receive", ctx, async () =>
            {
                var msg = await promise.Task;
                return Response.Ok(sizeBytes: msg.Payload.Length);
            });

            var disconnect = await Step.Run("disconnect", ctx, async () =>
            {
                await mqttClient.DisconnectAsync();
                return Response.Ok();
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
