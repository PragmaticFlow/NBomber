using RabbitMQ.Client;
using NBomber.AMQP;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;
using Microsoft.Extensions.Configuration;

namespace Demo.AMQP.IndependentActors;

public class AmqpPublishScenario
{
    // For this example, please spin up local RabbitMQ via docker-compose.yml located in the AMQP folder.

    public ScenarioProps Create()
    {
        byte[] payload = [];
        AmqpClient amqpClient = null;

        return Scenario.Create("publish_scenario", async ctx =>
        {
            var publish = await Step.Run("publish", ctx, async () =>
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var prop = new BasicProperties
                {
                    // We include the current timestamp so the consumer can calculate the final latency.
                    Headers = new Dictionary<string, object>
                    {
                        { "timestamp", timestamp }
                    }
                };

                return await amqpClient.Publish(exchange: "myExchange", routingKey: "IndependentActors", prop, body: payload);
            });

            return Response.Ok();
        })
        .WithInit(async ctx =>
        {
            var config = ctx.GlobalCustomSettings.Get<AmqpCustomSettings>();
            payload = Data.GenerateRandomBytes(config.MsgSizeBytes);

            var factory = new ConnectionFactory { HostName = config.AmqpServerUrl };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            amqpClient = new AmqpClient(channel);

            await amqpClient.Connect(exchange: "myExchange", exchangeType: ExchangeType.Direct, queue: "IndependentActors",
                routingKey: "IndependentActors");
        })
        .WithClean(async ctx =>
        {
            await amqpClient.Disconnect();
        });
    }
}
