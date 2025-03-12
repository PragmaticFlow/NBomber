using Google.Protobuf;
using Grpc.Net.Client;
using GrpcGreeterClient;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;

namespace Demo.gRPC;

public class GrpcWriteScenario
{
    private GrpcConfig _grpcConfig;
    private Greeter.GreeterClient _grpcClient;
    private ByteString _payload;
    private readonly Random _random = new();

    public ScenarioProps Create()
    {
        return Scenario
            .Create("redis_write", async context =>
            {
                var randomId = _random.Next(_grpcConfig.RecordsCount);

                await _grpcClient.SendDataAsync(
                    new SendDataRequest { RecordId = randomId, Data = _payload }
                );

                return Response.Ok(sizeBytes: _payload.Length);
            })
            .WithInit(context =>
            {
                _grpcConfig = context.GlobalCustomSettings.Get<GrpcConfig>();

                using var channel = GrpcChannel.ForAddress(_grpcConfig.ConnectionString);
                _grpcClient = new Greeter.GreeterClient(channel);

                var randomBytes = Data.GenerateRandomBytes(_grpcConfig.RecordSize);
                _payload = ByteString.CopyFrom(randomBytes);

                return Task.CompletedTask;
            });
    }
}
