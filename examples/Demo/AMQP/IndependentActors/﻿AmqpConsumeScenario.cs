using RabbitMQ.Client;
using NBomber.AMQP;
using NBomber.Contracts;
using NBomber.CSharp;

namespace Demo.AMQP.IndependentActors;

public class AmqpConsumeScenario
{
    public ScenarioProps Create()
    {
        AmqpClient amqpClient = null;

        return Scenario.Create("consume_scenario", async ctx =>
        {
            var message = await amqpClient.Receive(ctx.ScenarioCancellationToken);

            // Final latency is computed by subtracting the current time from the timestamp in the header.
            var timestampMs = (long)message.Payload.Value.BasicProperties.Headers["timestamp"];
            var latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - timestampMs;

            return Response.Ok(customLatencyMs: latency);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(1, TimeSpan.FromSeconds(30))
        )
        .WithInit(async ctx =>
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            amqpClient = new AmqpClient(channel);

            await amqpClient.Connect(exchange: "myExchange", exchangeType: ExchangeType.Direct, queue: "IndependentActors",
                routingKey: "IndependentActors");

            await amqpClient.Subscribe(queue: "IndependentActors", autoAck: true);
        })
        .WithClean(async ctx =>
        {
            await amqpClient.Disconnect();
        });
    }
}
