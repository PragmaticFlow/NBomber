using NBomber.CSharp;

namespace Demo.HTTP.WebAppSimulator
{
    public class WebAppSimulatorExample
    {
        public void RunHttpUserExample()
        {
            // For this example, you'll need to start the HttpApiSimulator, which is located in the examples/simulators solution folder.
            // Make sure it’s running before executing the client tests to ensure proper communication.
            // Also, please spin up local environment via docker-compose.yml located in HttpApiSimulator project.

            NBomberRunner.RegisterScenarios(
                new InitHttpScenario().Create(),
                new ReadHttpScenario().Create(),
                new WriteHttpScenario().Create()
            )
            .LoadConfig("./HTTP/WebAppSimulator/config.json")
            .Run();
        }
    }
}
