using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;
using CorrelationId = System.String;

namespace Demo.MQTT.ConstantRate;

public class ConstantPublishToMqtt
{
    private readonly ConcurrentDictionary<CorrelationId, TaskCompletionSource<MqttApplicationMessage>> _responsePromises = new();
    private IMqttClient _mqttClient = null;

    public void Run()
    {
        var scenario = Scenario.Create("constant_publish_rate", async ctx =>
        {
            var correlationId = Guid.NewGuid().ToString();
            var promise = new TaskCompletionSource<MqttApplicationMessage>();
            _responsePromises[correlationId] = promise;

            var publish = await Step.Run("publish", ctx, async () =>
            {
                await _mqttClient.PublishAsync("/my_topic", correlationId);
                return Response.Ok();
            });

            var receive = await Step.Run("receive", ctx, async () =>
            {
                var response = await promise.Task;
                return Response.Ok();
            });

            return Response.Ok();
        })
        .WithoutWarmUp()

        .WithLoadSimulations(Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)))
        // .WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30)))

        .WithInit(Init)
        .WithClean(context =>
        {
            _mqttClient.Dispose();
            return Task.CompletedTask;
        });

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    private async Task Init(IScenarioInitContext ctx)
    {
        var mqttFactory = new MqttFactory();

        _mqttClient = mqttFactory.CreateMqttClient();
        var clientOptions = new MqttClientOptionsBuilder()
            .WithWebSocketServer("ws://localhost:8083/mqtt")
            .WithCleanSession()
            .WithClientId($"my_client")
            .Build();

        var result = await _mqttClient.ConnectAsync(clientOptions);
        if (result.ResultCode == MqttClientConnectResultCode.Success)
        {
            _mqttClient.UseApplicationMessageReceivedHandler(msg =>
            {
                var correlationId = msg.ApplicationMessage.ConvertPayloadToString();
                var promise = _responsePromises[correlationId];

                // here we signaling Step("receive") to continue work
                promise.SetResult(msg.ApplicationMessage);
            });

            await _mqttClient.SubscribeAsync("/my_topic");
        }
        else
            throw new Exception("client can't connect to the MQTT broker");
    }
}
