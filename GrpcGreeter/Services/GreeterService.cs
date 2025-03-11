using Google.Protobuf;
using Grpc.Core;
using GrpcGreeter;

namespace GrpcGreeter.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    private ByteString Data { get; set; }

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
        Data = ByteString.Empty;
    }

    public override Task<SendDataReply> SendData(SendDataRequest request, ServerCallContext context)
    {
        Data = request.Data;

        return Task.FromResult(new SendDataReply
        {
            SendDataStatus = "Success"
        });
    }

    public override Task<ReadDataReply> ReadData(ReadDataRequest request, ServerCallContext context)
    {
        return Task.FromResult(new ReadDataReply
        {
            Data = this.Data
        });
    }
}
