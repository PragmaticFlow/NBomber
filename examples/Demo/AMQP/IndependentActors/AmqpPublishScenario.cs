using RabbitMQ.Client;
using NBomber.AMQP;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;

namespace Demo.AMQP.IndependentActors;

public class AmqpPublishScenario
{
    public ScenarioProps Create()
    {
        byte[] payload = Data.GenerateRandomBytes(200);
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
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Inject(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30))
        )
        .WithInit(async ctx =>
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
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
