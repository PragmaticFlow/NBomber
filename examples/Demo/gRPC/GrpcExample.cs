using NBomber.CSharp;

namespace Demo.gRPC;

public class GrpcExample
{
    public void Run()
    {
        // For this example, you'll need to start the GrpcServerSimulator, which is located in the examples/simulators solution folder.
        // Make sure it’s running before executing the client tests to ensure proper communication.

        NBomberRunner.RegisterScenarios(
            new GrpcInitScenario().Create(),
            new GrpcReadScenario().Create(),
            new GrpcWriteScenario().Create()
        )
        .LoadConfig("./gRPC/config.json")
        .Run();
    }
}
