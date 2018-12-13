using System;
using System.Net.WebSockets;
using System.Threading;

using NBomber.Contracts;
using NBomber.CSharp;

using TestServer.Utils;
using TestServer.WebSockets;

namespace CSharp.Examples.Scenarios
{
    class WebSocketsScenario
    {
        public static Scenario BuildScenario()
        {
            var url = "ws://localhost:5000";

            var webSocketsPool = ConnectionPool.Create("webSocketsPool", () =>
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

            var pingStep = Step.CreatePull("ping", webSocketsPool, async context =>
            {
                var msg = new WebSocketRequest
                {
                    CorrelationId = context.CorrelationId,
                    RequestType = RequestType.Ping
                };
                var bytes = MsgConverter.ToJsonByteArray(msg);
                await context.Connection.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
                return Response.Ok();
            });

            var pongStep = Step.CreatePush("pong", webSocketsPool, async context =>
            {
                var (response, message) = await WebSocketsMiddleware.ReadFullMessage(context.Connection);
                var msg = MsgConverter.FromJsonByteArray<WebSocketResponse>(message);

                if (msg.CorrelationId == context.CorrelationId)
                {
                    context.UpdatesChannel.ReceivedUpdate(Response.Ok(msg));
                }
            });

            return ScenarioBuilder.CreateScenario("web_socket test", pingStep, pongStep);                          
        }
    }
}
