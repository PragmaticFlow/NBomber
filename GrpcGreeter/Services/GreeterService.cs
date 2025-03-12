using Google.Protobuf;
using Grpc.Core;
using System.Collections.Concurrent;

namespace GrpcGreeter.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    private ConcurrentDictionary<int, ByteString> DataStorage { get; set; }

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
        DataStorage = new ConcurrentDictionary<int, ByteString>();
    }

    public override Task<SendDataReply> SendData(SendDataRequest request, ServerCallContext context)
    {
        var result = DataStorage.TryAdd(request.RecordId, request.Data);

        return Task.FromResult(new SendDataReply
        {
            SendDataStatus = result == true ? "Success" : "Fail"
        });
    }

    public override Task<ReadDataReply> ReadData(ReadDataRequest request, ServerCallContext context)
    {
        return Task.FromResult(new ReadDataReply
        {
            Data = DataStorage[request.RecordId]
        });
    }
}
