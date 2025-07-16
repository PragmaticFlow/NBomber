using NBomber.AMQP;
using NBomber.CSharp;
using NBomber.Data;
using RabbitMQ.Client;

namespace Demo.AMQP;

public class PingPongAmqpTest
{
    // For this example, please spin up local RabbitMQ via docker-compose.yml located in the AMQP folder.

    public void Run()
    {
        var payload = Data.GenerateRandomBytes(200);
        var factory = new ConnectionFactory { HostName = "localhost" };

        var scenario = Scenario.Create("ping_pong_scenario", async ctx =>
        {
            var connect = await Step.Run("connect", ctx, async () =>
            {
                var connection = await factory.CreateConnectionAsync();
                var channel = await connection.CreateChannelAsync();

                var amqpClient = new AmqpClient(channel);
                return Response.Ok(payload: amqpClient);
            });

            using var amqpClient = connect.Payload.Value;

            var subscribe = await Step.Run("subscribe", ctx, async () =>
            {
                var queueName = ctx.ScenarioInfo.InstanceId;

                await amqpClient.DeclareQueue(exchange: "myExchange", exchangeType: ExchangeType.Direct, queue: queueName,
                    routingKey: queueName);

                return await amqpClient.Subscribe(queue: queueName, autoAck: true);
            });

            var publish = await Step.Run("publish", ctx, async () =>
            {
                var queueName = ctx.ScenarioInfo.InstanceId;
                return await amqpClient.Publish(exchange: "myExchange", routingKey: queueName, body: payload);
            });

            var receive = await Step.Run("receive", ctx, async () =>
            {
                // Here, we pass the ScenarioCancellationToken to stop waiting for a response if the scenario finish event is triggered
                var response = await amqpClient.Receive(ctx.ScenarioCancellationToken);
                return response;
            });

            var disconnect = await Step.Run("disconnect", ctx, async () =>
            {
                await amqpClient.Disconnect();
                return Response.Ok();
            });

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
