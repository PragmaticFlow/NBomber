using NBomber.CSharp;

namespace Demo.gRPC;

public class GrpcExample
{
    public void Run()
    {
        NBomberRunner.RegisterScenarios(
            new GrpcInitScenario().Create(),
            new GrpcReadScenario().Create(),
            new GrpcWriteScenario().Create()
        )
        .LoadConfig("./gRPC/config.json")
        .Run();
    }
}
