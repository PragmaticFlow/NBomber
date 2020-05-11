using System;
using System.Net.WebSockets;
using System.Threading;

using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Extensions;

using TestServer.Utils;
using TestServer.WebSockets;

namespace CSharp.Examples.Scenarios
{
    class WebSocketsScenario
    {
        public static void Run(string[] args)
        {
            var url = "ws://localhost:5000";
            var concurrentCopies = 50;

            var webSocketsPool = ConnectionPoolArgs.Create(
                name: "webSocketsPool",
                getConnectionCount: () => concurrentCopies,
                openConnection: async (number,token) =>
                {
                    var ws = new ClientWebSocket();
                    await ws.ConnectAsync(new Uri(url), token);
                    return ws;
                },
                closeConnection: (connection, token) => connection.CloseAsync(WebSocketCloseStatus.NormalClosure, "", token));

            var pingStep = Step.Create("ping", webSocketsPool, async context =>
            {
                var msg = new WebSocketRequest
                {
                    CorrelationId = context.CorrelationId.Id,
                    RequestType = RequestType.Ping
                };
                var bytes = MsgConverter.ToJsonByteArray(msg);
                await context.Connection.SendAsync(bytes, WebSocketMessageType.Text, true, context.CancellationToken);
                return Response.Ok();
            });

            var pongStep = Step.Create("pong", webSocketsPool, async context =>
            {
                while (true)
                {
                    var (response, message) = await WebSocketsMiddleware.ReadFullMessage(context.Connection, context.CancellationToken);
                    var msg = MsgConverter.FromJsonByteArray<WebSocketResponse>(message);

                    if (msg.CorrelationId == context.CorrelationId.Id)
                    {
                        return Response.Ok(msg);
                    }
                }
            });

            var scenario = ScenarioBuilder
                .CreateScenario("web_socket test", new[] {pingStep, pongStep})
                .WithoutWarmUp()
                .WithLoadSimulations(new[]
                {
                    Simulation.KeepConcurrentScenarios(concurrentCopies, during: TimeSpan.FromSeconds(10))
                });

            NBomberRunner
                .RegisterScenarios(new[] {scenario})
                .RunInConsole();

        }
    }
}
