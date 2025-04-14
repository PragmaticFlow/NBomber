using Demo.DB.Redis;
using NBomber.CSharp;

namespace Demo.HTTP.SimpleBookstore
{
    public class ExampleSimpleBookstore
    {
        public void Run()
        {
            // For this example, you'll need to start the BookstoreSimulator, which is located in the examples/simulators solution folder.
            // Make sure it’s running before executing the client tests to ensure proper communication.
            // Also, please spin up local environment via docker-compose.yml located in BookstoreSimulator project.

            NBomberRunner.RegisterScenarios(
                new InitSimpleBookstoreScenario().Create(),
                new TestSimpleBookstoreScenario().Create()
            )
            .LoadConfig("./HTTP/SimpleBookstore/config.json")
            .Run();
        }
    }
}
