namespace TestServer.WebSockets
{
    public enum RequestType
    {
        Ping
    }

    public class WebSocketRequest
    {
        public string CorrelationId { get; set; }
        public RequestType RequestType { get; set; }
    }

    public enum ResponseType
    {
        Pong
    }

    public class WebSocketResponse
    {
        public string CorrelationId { get; set; }
        public ResponseType ResponseType { get; set; }
    }
}
