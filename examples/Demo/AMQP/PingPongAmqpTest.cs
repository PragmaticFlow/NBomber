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

        var scenario = Scenario.Create("ping_pong_amqp_scenario", async ctx =>
        {
            AmqpClient amqpClient = null;

            var connect = await Step.Run("connect", ctx, async () =>
            {
                var connection = await factory.CreateConnectionAsync();
                var channel = await connection.CreateChannelAsync();

                amqpClient = new AmqpClient(channel);

                var scenarioInstanceId = ctx.ScenarioInfo.InstanceId;

                return await amqpClient.Connect(exchange: "myExchange", exchangeType: ExchangeType.Direct, queue: scenarioInstanceId,
                    routingKey: scenarioInstanceId);
            });

            var subscribe = await Step.Run("subscribe", ctx, async () =>
            {
                var queueName = ctx.ScenarioInfo.InstanceId;
                return await amqpClient.Subscribe(queue: queueName, autoAck: true);
            });

            var publish = await Step.Run("publish", ctx, async () =>
            {
                var queueName = ctx.ScenarioInfo.InstanceId;
                var prop = new BasicProperties();
                return await amqpClient.Publish(exchange: "myExchange", routingKey: queueName, prop, body: payload);
            });

            var receive = await Step.Run("receive", ctx, async () =>
            {
                var response = await amqpClient.Receive().AsTask();
                return response;
            });

            var disconnect = await Step.Run("disconnect", ctx, async () =>
            {
                await amqpClient.Disconnect();
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
