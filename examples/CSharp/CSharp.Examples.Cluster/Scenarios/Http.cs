using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace CSharp.Examples.Cluster.Scenarios
{
    public class HttpScenario
    {
        public static void Run(string configPath)
        {
            var step = HttpStep.Create("simple step", async (context) =>
                Http.CreateRequest("GET", "https://nbomber.com")
                    .WithHeader("Accept", "text/html")
                    //.WithHeader("Cookie", "cookie1=value1; cookie2=value2")
                    //.WithBody(new StringContent("{ some JSON }", Encoding.UTF8, "application/json"))
                    //.WithCheck(response => Task.FromResult(response.IsSuccessStatusCode))
            );

            var scenario = ScenarioBuilder.CreateScenario("test_gitter", step);
            
            NBomberRunner.RegisterScenarios(scenario)
                         .LoadConfig(configPath)
                         //.LoadConfig("agent_config.json")
                         //.LoadConfig("coordinator_config.json")
                         .RunInConsole();
        }
    }
}