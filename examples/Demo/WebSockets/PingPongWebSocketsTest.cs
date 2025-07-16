using NBomber.CSharp;
using NBomber.Data;
using NBomber.WebSockets;

namespace Demo.WebSockets;

public class PingPongWebSocketsTest
{
    // To run this example you need to spin up local server examples/simulators/WebSocketsSimulator
    // The server should run on localhost:60528

    public void Run()
    {
        var payload = Data.GenerateRandomBytes(sizeInBytes: 500);

        var scenario = Scenario.Create("ping_pong_websockets", async ctx =>
        {
            using var websocket = new WebSocket(new WebSocketConfig());

            var connect = await Step.Run("connect", ctx, async () =>
            {
                await websocket.Connect("ws://localhost:60528/ws");
                return Response.Ok();
            });

            var ping = await Step.Run("ping", ctx, async () =>
            {
                await websocket.Send(payload);
                return Response.Ok(sizeBytes: payload.Length);
            });

            var pong = await Step.Run("pong", ctx, async () =>
            {
                using var response = await websocket.Receive(ctx.ScenarioCancellationToken);
                // var str = Encoding.UTF8.GetString(response.Data.Span);
                // var user = JsonSerializer.Deserialize<T>(response.Data.Span);
                return Response.Ok(sizeBytes: response.Data.Length);
            });

            var disconnect = await Step.Run("disconnect", ctx, async () =>
            {
                await websocket.Close();
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
