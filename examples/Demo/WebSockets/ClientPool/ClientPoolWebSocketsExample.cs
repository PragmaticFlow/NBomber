using Microsoft.Extensions.Configuration;
using NBomber;
using NBomber.CSharp;
using NBomber.Data;
using NBomber.WebSockets;

namespace Demo.WebSockets.ClientPool;

public class ClientPoolWebSocketsExample
{
    // To run this example you need to spin up local server examples/simulators/WebSocketsSimulator
    // The server should run on localhost:60528

    public class CustomScenarioSettings
    {
        public string WebSocketsServerUrl { get; set; }
        public int ClientCount { get; set; }
        public int MsgSizeBytes { get; set; }
    }

    public void Run()
    {
        var clientPool = new ClientPool<WebSocket>();
        byte[] payload = null;

        var scenario = Scenario.Create("websockets_client_pool", async ctx =>
        {
            var websocket = clientPool.GetClient(ctx.ScenarioInfo);

            var ping = await Step.Run("ping", ctx, async () =>
            {
                await websocket.Send(payload);
                return Response.Ok(sizeBytes: payload.Length);
            });

            var pong = await Step.Run("pong", ctx, async () =>
            {
                using var response = await websocket.Receive();
                // var str = Encoding.UTF8.GetString(response.Data.Span);
                // var user = JsonSerializer.Deserialize<T>(response.Data.Span);
                return Response.Ok(sizeBytes: response.Data.Length);
            });

            return Response.Ok();
        })
        .WithWarmUpDuration(TimeSpan.FromSeconds(5))
        .WithLoadSimulations(
            Simulation.KeepConstant(50, TimeSpan.FromSeconds(30))
        )
        .WithInit(async ctx =>
        {
            var config = ctx.CustomSettings.Get<CustomScenarioSettings>();
            payload = Data.GenerateRandomBytes(sizeInBytes: config.MsgSizeBytes);

            for (var i = 0; i < config.ClientCount; i++)
            {
                var websocket = new WebSocket(new WebSocketConfig());
                await websocket.Connect(config.WebSocketsServerUrl);

                clientPool.AddClient(websocket);
            }
        })
        .WithClean(ctx =>
        {
            clientPool.DisposeClients(client => client.Close().Wait());
            return Task.CompletedTask;
        });

        NBomberRunner
            .RegisterScenarios(scenario)
            .LoadConfig("./WebSockets/ClientPool/config.json")
            .Run();
    }
}
