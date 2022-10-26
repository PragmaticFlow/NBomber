namespace CSharpDev.Mqtt;

using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using NBomber.Contracts;
using NBomber.CSharp;

public class SimpleMqttTest
{
    public void Run()
    {
        var scenario = Scenario.Create("mqtt_scenario", async ctx =>
        {
            using var mqttClient = new MqttFactory().CreateMqttClient();
            var topic = $"/clients/{ctx.ScenarioInfo.ThreadId}";
            var promise = new TaskCompletionSource<MqttApplicationMessage>();

            await Step.Run("connect", ctx, async () =>
            {
                var clientOptions = new MqttClientOptionsBuilder()
                    .WithWebSocketServer("ws://localhost:8083/mqtt")
                    .WithCleanSession()
                    .WithClientId($"client_{ctx.ScenarioInfo.ThreadId}")
                    .Build();

                var result = await mqttClient.ConnectAsync(clientOptions, ctx.CancellationToken);
                return result.ResultCode == MqttClientConnectResultCode.Success
                    ? Response.Ok()
                    : Response.Fail(
                        $"MQTT connection code is: {result.ResultCode}, reason: {result.ReasonString}");
            });

            await Step.Run("subscribe", ctx, async () =>
            {
                mqttClient.UseApplicationMessageReceivedHandler(msg =>
                {
                    promise.TrySetResult(msg.ApplicationMessage);
                });

                await mqttClient.SubscribeAsync(topic);

                return Response.Ok();
            });

            await Step.Run("publish", ctx, async () =>
            {
                await mqttClient.PublishAsync(topic, "hello world msg");
                return Response.Ok();
            });

            await Step.Run("receive", ctx, async () =>
            {
                await promise.Task.WaitAsync(ctx.CancellationToken);
                return Response.Ok();
            });

            await Step.Run("disconnect", ctx, async () =>
            {
                await mqttClient.DisconnectAsync();
                return Response.Ok();
            });

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(1, TimeSpan.FromMinutes(3))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
