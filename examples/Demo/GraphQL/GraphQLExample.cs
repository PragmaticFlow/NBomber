using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Text;
using System.Text.Json;

namespace Demo.GraphQL;

class GraphQLExample
{
    public void Run()
    {
        // For this example, you'll need to start the GraphQLServerSimulator, which is located in the examples/simulators solution folder.
        // Make sure it’s running before executing the client tests to ensure proper communication.

        var httpClient = new HttpClient();

        var scenario = Scenario.Create("graphql_scenario", async context =>
        {
            var queryObject = new
            {
                query = @"
                  query {
                    users(where: { age: { gt: 20 } }) {
                      name
                      age
                      role {
                        name
                      }
                    }
                  }
                "
            };
            string jsonQuery = JsonSerializer.Serialize(queryObject);

            var request = Http.CreateRequest("POST", "http://localhost:5088/graphql")
                .WithBody(new StringContent(jsonQuery, Encoding.UTF8, "application/json"));

            return await Http.Send(httpClient, request);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(1, TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
