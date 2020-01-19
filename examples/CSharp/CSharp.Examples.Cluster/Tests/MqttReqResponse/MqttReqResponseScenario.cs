using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Extensions;

namespace CSharp.Examples.Cluster.Tests.MqttReqResponse
{
    public class CustomSettings
    {
        public string TargetMqttBrokerHost { get; set; }
        public int MsgPayloadSizeInBytes { get; set; }
    }

    public class MqttReqResponseScenario
    {
        class State
        {
            public byte[] MsgPayload { get; set; }
            public string TargetMqttBrokerHost { get; set; }
            public ClientResponses ClientResponses { get; set; } = new ClientResponses();
        }

        public static Scenario Create()
        {
            var state = new State();
            var mqttConectionPool = CreateMqttConnectionPool(state);

            var requestStep = Step.Create("request step", async context =>
            {
                var clientId = context.Connection.Options.ClientId;

                var msg = new MqttApplicationMessage
                {
                    Topic = $"requests/{clientId}",
                    Payload = state.MsgPayload
                };

                await context.Connection.PublishAsync(msg);
                return Response.Ok(sizeBytes: state.MsgPayload.Length);
            },
            connectionPool: mqttConectionPool);

            var responseStep = Step.Create("response step", async context =>
            {
                var clientId = context.Connection.Options.ClientId;
                var response = await state.ClientResponses.GetResponseAsync(clientId);
                return Response.Ok(sizeBytes: response.Length);
            },
            connectionPool: mqttConectionPool);

            var scenario = ScenarioBuilder
                .CreateScenario("request response scenario", new[] {requestStep, responseStep})
                .WithTestInit(context =>
                {
                    var settings = context.CustomSettings.DeserializeJson<CustomSettings>();

                    state.MsgPayload = GeneratePayload(settings.MsgPayloadSizeInBytes);
                    state.TargetMqttBrokerHost = settings.TargetMqttBrokerHost;

                    context.Logger.Information("MsgPayloadSizeInBytes:'{MsgPayloadSizeInBytes}'", settings.MsgPayloadSizeInBytes);

                    return Task.CompletedTask;
                });

            return scenario;
        }

        static IConnectionPool<IMqttClient> CreateMqttConnectionPool(State state)
        {
            var mqttFactory = new MqttFactory();

            return ConnectionPool.Create(
                name: "mqtt.connection.pool",
                openConnection: () =>
                {
                    var clientId = Guid.NewGuid().ToString("N");
                    var client = mqttFactory.CreateMqttClient();

                    var clientOptions = new MqttClientOptionsBuilder()
                        .WithTcpServer(state.TargetMqttBrokerHost)
                        .WithCleanSession()
                        .WithClientId(clientId)
                        .Build();

                    client.UseConnectedHandler(e => client.SubscribeAsync($"requests/{clientId}"));

                    client.UseApplicationMessageReceivedHandler(msg =>
                    {
                        state.ClientResponses.SetResponse(clientId, msg.ApplicationMessage.Payload);
                    });

                    client.ConnectAsync(clientOptions, CancellationToken.None).Wait();
                    state.ClientResponses.InitClientId(clientId);

                    return client;
                });
        }

        static byte[] GeneratePayload(int payloadSizeInBytes)
        {
            var buffer = new byte[payloadSizeInBytes];
            new Random().NextBytes(buffer);
            return buffer;
        }
    }
}
