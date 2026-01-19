using NBomber.CSharp;
using NBomber.Http;

namespace Demo.HTTP.HttpApiSimulator
{
    public class HttpApiSimulatorExample
    {
        public void Run()
        {
            // For this example, you'll need to start the HttpApiSimulator, which is located in the examples/simulators solution folder.
            // Make sure it’s running before executing the client tests to ensure proper communication.
            // Also, please spin up local environment via docker-compose.yml located in HttpApiSimulator project.

            NBomberRunner.RegisterScenarios(
                new InitHttpScenario().Create(),
                new ReadHttpScenario().Create(),
                new WriteHttpScenario().Create()
            )
            .WithWorkerPlugins(new HttpMetricsPlugin([HttpVersion.Version1]))
            .LoadConfig("./HTTP/HttpApiSimulator/config.json")
            .Run();
        }
    }
}
