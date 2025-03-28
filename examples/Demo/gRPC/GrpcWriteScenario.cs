using Google.Protobuf;
using Grpc.Net.Client;
using GrpcSimulator;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;

namespace Demo.gRPC;

public class GrpcWriteScenario
{
    private GrpcConfig _grpcConfig;
    private GrpcSimulator.GrpcSimulator.GrpcSimulatorClient _grpcClient;
    private ByteString _payload;
    private readonly Random _random = new();

    public ScenarioProps Create()
    {
        return Scenario
            .Create("grpc_write", async context =>
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

                var channel = GrpcChannel.ForAddress(_grpcConfig.ConnectionString);
                _grpcClient = new GrpcSimulator.GrpcSimulator.GrpcSimulatorClient(channel);

                var randomBytes = Data.GenerateRandomBytes(_grpcConfig.RecordSize);
                _payload = ByteString.CopyFrom(randomBytes);

                return Task.CompletedTask;
            });
    }
}
