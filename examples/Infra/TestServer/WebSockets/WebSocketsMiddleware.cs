using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TestServer.Utils;

namespace TestServer.WebSockets
{
    public class WebSocketsMiddleware
    {
        public static readonly int BufferSize = 4096;
        readonly RequestDelegate _next;

        public WebSocketsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            using (var socket = await context.WebSockets.AcceptWebSocketAsync())
            {
                await StartReceiveMessages(socket);
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server is stopping", CancellationToken.None);
            }
        }

        async Task StartReceiveMessages(WebSocket socket)
        {
            while (socket.State == WebSocketState.Open)
            {
                var (response, message) = await ReadFullMessage(socket, CancellationToken.None);

                if (response.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }
                else
                {
                    var msg = MsgConverter.FromJsonByteArray<WebSocketRequest>(message);
                    await Task.Delay(TimeSpan.FromSeconds(0.1));

                    var msgResponse = new WebSocketResponse
                    {
                        CorrelationId = msg.CorrelationId,
                        ResponseType = ResponseType.Pong
                    };
                    var bytes = MsgConverter.ToJsonByteArray(msgResponse);
                    await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public static async Task<(WebSocketReceiveResult,byte[])> ReadFullMessage(WebSocket socket, CancellationToken token)
        {
            var buffer = new byte[BufferSize];

            WebSocketReceiveResult response;
            var message = new List<byte>();
            do
            {
                response = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                message.AddRange(new ArraySegment<byte>(buffer, 0, response.Count));
            }
            while (!response.EndOfMessage);

            return (response, message.ToArray());
        }
    }
}
