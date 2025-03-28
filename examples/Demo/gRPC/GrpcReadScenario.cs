using Grpc.Net.Client;
using GrpcSimulator;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using GrpcSimulatorClient = GrpcSimulator.GrpcSimulator.GrpcSimulatorClient;

namespace Demo.gRPC;

public class GrpcReadScenario
{
    private GrpcConfig _grpcConfig;
    private GrpcSimulatorClient _grpcClient;
    private readonly Random _random = new();

    public ScenarioProps Create()
    {
        return Scenario
            .Create("grpc_read", async context =>
            {
                var randomId = _random.Next(_grpcConfig.RecordsCount);

                var getDataResponse = await _grpcClient.GetDataAsync(
                    new GetDataRequest { RecordId = randomId }
                );

                return Response.Ok(sizeBytes: getDataResponse.Data.Length);
            })
            .WithInit(context =>
            {
                _grpcConfig = context.GlobalCustomSettings.Get<GrpcConfig>();

                var channel = GrpcChannel.ForAddress(_grpcConfig.ConnectionString);
                _grpcClient = new GrpcSimulatorClient(channel);

                return Task.CompletedTask;
            });
    }
}
