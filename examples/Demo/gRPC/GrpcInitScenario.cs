using Google.Protobuf;
using Grpc.Net.Client;
using GrpcSimulator;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;

namespace Demo.gRPC;

public class GrpcConfig
{
    public string ConnectionString { get; set; }
    public int RecordsCount { get; set; }
    public int RecordSize { get; set; }
}

public class GrpcInitScenario
{
    public ScenarioProps Create()
    {
        return Scenario.Empty("grpc_init")
            .WithInit(context =>
            {
                var grpcConfig = context.GlobalCustomSettings.Get<GrpcConfig>();

                using var channel = GrpcChannel.ForAddress(grpcConfig.ConnectionString);
                var client = new GrpcSimulator.GrpcSimulator.GrpcSimulatorClient(channel);

                foreach (var i in Enumerable.Range(0, grpcConfig.RecordsCount))
                {
                    var randomBytes = Data.GenerateRandomBytes(grpcConfig.RecordSize);
                    var randomData = ByteString.CopyFrom(randomBytes);
                    var sendDataReply = client.SendData(
                        new SendDataRequest { RecordId = i, Data = randomData }
                    );
                }

                return Task.CompletedTask;
            });
    }
}
