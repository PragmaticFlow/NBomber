using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
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
        DataStorage[request.RecordId] = request.Data;

        return Task.FromResult(new SendDataReply
        {
            SendDataStatus = "Success"
        });
    }

    public override Task<GetDataReply> GetData(GetDataRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetDataReply
        {
            Data = DataStorage[request.RecordId]
        });
    }

    public override async Task<SendDataReply> SendDataStream(IAsyncStreamReader<SendDataRequest> requestStream,
        ServerCallContext context)
    {
        var response = new SendDataReply();
        while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
        {
            DataStorage[requestStream.Current.RecordId] = requestStream.Current.Data;
            response.SendDataStatus = "Success";
        }

        return await Task.FromResult<SendDataReply>(response);
    }

    public override async Task GetDataStream(Empty request,
        IServerStreamWriter<GetDataReply> responseStream, ServerCallContext context)
    {
        var dataStorageKeys = DataStorage.Keys.ToList();
        var i = 0;

        while (!context.CancellationToken.IsCancellationRequested && i < dataStorageKeys.Count)
        {
            await responseStream.WriteAsync(
                new GetDataReply { Data = DataStorage[dataStorageKeys[i]] }
            );

            i++;
        }
    }

    public override async Task SendAndGetDataStream(IAsyncStreamReader<SendDataRequest> requestStream,
        IServerStreamWriter<GetDataReply> responseStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
        {
            DataStorage[requestStream.Current.RecordId] = requestStream.Current.Data;

            await responseStream.WriteAsync(
                new GetDataReply { Data = DataStorage[requestStream.Current.RecordId] }
            );
        }
    }
}
