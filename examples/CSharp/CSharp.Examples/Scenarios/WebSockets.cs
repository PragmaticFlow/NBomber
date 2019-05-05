using System;
using System.Net.WebSockets;
using System.Threading;

using NBomber.Contracts;
using NBomber.CSharp;
using TestServer;
using TestServer.WebSockets;

namespace CSharp.Examples.Scenarios
{
    class WebSocketsScenario
    {
        public static Scenario BuildScenario()
        {
            var url = "ws://localhost:53231";

            var webSocketsPool = ConnectionPool.Create("webSocketsPool",
            openConnection: () =>
            {
                var ws = new ClientWebSocket();
                ws.ConnectAsync(new Uri(url), CancellationToken.None).Wait();
                return ws;
            },
            closeConnection: (connection) =>
            {
                connection.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None)
                          .Wait();
            });
            //connectionsCount: 50);

            var pingStep = Step.Create("ping", webSocketsPool, async context =>
            {
                var msg = new WebSocketRequest
                {
                    CorrelationId = context.CorrelationId,
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
                    var (_, message) = await WebSocketsMiddleware.ReadFullMessage(context.Connection, context.CancellationToken);
                    var msg = MsgConverter.FromJsonByteArray<WebSocketResponse>(message);

                    if (msg.CorrelationId == context.CorrelationId)
                    {
                        return Response.Ok(msg);
                    }
                }
            });

            return ScenarioBuilder.CreateScenario("web_socket test", pingStep, pongStep);
        }
    }
}
