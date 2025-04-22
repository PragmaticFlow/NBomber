using Microsoft.Extensions.Configuration;
using NBomber;
using NBomber.AMQP;
using NBomber.CSharp;
using NBomber.Data;
using RabbitMQ.Client;

namespace Demo.AMQP.ClientPool;

public class CustomScenarioSettings
{
    public string AmqpServerUrl { get; set; }
    public int ClientCount { get; set; }
    public int MsgSizeBytes { get; set; }
    public bool UsePersistence { get; set; }
}

public class ClientPoolAmqpExample
{
    // For this example, please spin up local RabbitMQ via docker-compose.yml located in the AMQP folder.

    public void Run()
    {
        var clientPool = new ClientPool<AmqpClient>();
        byte[] message = [];
        var usePersistence = false;

        var scenario = Scenario.Create("amqp_scenario", async ctx =>
        {
            var client = clientPool.GetClient(ctx.ScenarioInfo);

            var publish = await Step.Run("publish", ctx, async () =>
            {
                var scenarioInstanceId = ctx.ScenarioInfo.InstanceId;
                var prop = new BasicProperties { Persistent = usePersistence };

                var response = await client.Publish(exchange: "myExchange", routingKey: scenarioInstanceId, basicProperties: prop, body: message);
                return response;
            });

            var receive = await Step.Run("receive", ctx, async () =>
            {
                var response = await client.Receive(ctx.ScenarioCancellationToken);
                return response;
            });

            return Response.Ok();
        })
        .WithInit(async context =>
        {
            var config = context.CustomSettings.Get<CustomScenarioSettings>();
            message = Data.GenerateRandomBytes(config.MsgSizeBytes);
            usePersistence = config.UsePersistence;

            var factory = new ConnectionFactory { HostName = config.AmqpServerUrl };

            for (var i = 0; i < config.ClientCount; i++)
            {
                var connection = await factory.CreateConnectionAsync();
                var channel = await connection.CreateChannelAsync();
                var amqpClient = new AmqpClient(channel);

                var scenarioInstanceId = $"amqp_scenario_{i}";
                var result = await amqpClient.Connect(exchange: "myExchange", exchangeType: ExchangeType.Direct, queue: scenarioInstanceId,
                        routingKey: scenarioInstanceId, durable: usePersistence);

                if (!result.IsError)
                {
                    await amqpClient.Subscribe(queue: scenarioInstanceId);
                    clientPool.AddClient(amqpClient);
                }
                else
                    throw new Exception("client can't connect to the AMQP broker");

                await Task.Delay(10);
            }
        })
        .WithClean(ctx =>
        {
            clientPool.DisposeClients(client => client.Dispose());
            return Task.CompletedTask;
        });

        NBomberRunner
            .RegisterScenarios(scenario)
            .LoadConfig("./AMQP/ClientPool/config.json")
            .Run();
    }
}
