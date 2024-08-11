using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IO;

namespace WebAppSimulator.Controllers;

public class WebSocketController : ControllerBase
{
    private static readonly RecyclableMemoryStreamManager MsStreamManager = new();
    private const int BufferSize = 1024 * 16;

    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            using var ms = MsStreamManager.GetStream();

            await Receive(webSocket, ms);
            //await Send(webSocket, ms);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task Receive(WebSocket webSocket, RecyclableMemoryStream ms)
    {
        var closeSocket = false;

        while (!closeSocket)
        {
            var endOfMessage = false;

            while (!endOfMessage)
            {
                var buffer = ms.GetMemory(BufferSize);
                var message = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                if (message.MessageType == WebSocketMessageType.Close)
                {
                    closeSocket = true;
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    break;
                }

                ms.Advance(message.Count);
                endOfMessage = message.EndOfMessage;
            }

            if (!closeSocket)
                await Send(webSocket, ms);
        }
    }

    private async Task Send(WebSocket webSocket, RecyclableMemoryStream ms)
    {
        if (ms.Length > 0)
        {
            var msg = ms.GetBuffer().AsMemory(0, (int) ms.Length);
            await webSocket.SendAsync(msg, WebSocketMessageType.Binary, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
        }
    }
}
