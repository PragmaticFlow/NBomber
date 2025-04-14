namespace Demo.WebSockets.ClientPool;

using NBomber;
using NBomber.CSharp;
using NBomber.Data;
using NBomber.WebSockets;

public class ClientPoolWebSocketsExample
{
    // To run this example you need to spin up local server examples/simulators/WebSocketsSimulator
    // The server should run on localhost:60528

    public void Run()
    {
        var clientPool = new ClientPool<WebSocket>();
        var payload = Data.GenerateRandomBytes(sizeInBytes: 500);

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
            for (var i = 0; i < 50; i++)
            {
                var websocket = new WebSocket(new WebSocketConfig());
                await websocket.Connect("ws://localhost:60528/ws");

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
            .Run();
    }
}
