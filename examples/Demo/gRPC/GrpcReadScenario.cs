using Grpc.Net.Client;
using GrpcGreeterClient;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;

namespace Demo.gRPC;

public class GrpcReadScenario
{
    private GrpcConfig _grpcConfig;
    private Greeter.GreeterClient _grpcClient;
    private readonly Random _random = new();

    public ScenarioProps Create()
    {
        return Scenario
            .Create("grpc_read", async context =>
            {
                var randomId = _random.Next(_grpcConfig.RecordsCount);

                var readDataResponse = await _grpcClient.ReadDataAsync(
                    new ReadDataRequest { RecordId = randomId }
                );

                return Response.Ok(sizeBytes: readDataResponse.Data.Length);
            })
            .WithInit(context =>
            {
                _grpcConfig = context.GlobalCustomSettings.Get<GrpcConfig>();

                var channel = GrpcChannel.ForAddress(_grpcConfig.ConnectionString);
                _grpcClient = new Greeter.GreeterClient(channel);

                return Task.CompletedTask;
            });
    }
}
