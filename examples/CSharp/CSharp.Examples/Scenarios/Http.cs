using System;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace CSharp.Examples.Scenarios
{
    class HttpScenario
    {
        public static void Run()
        {
            var step = HttpStep.Create("http pull", context =>
                Http.CreateRequest("GET", "https://nbomber.com")
                    .WithHeader("Accept", "text/html")
                    //.WithHeader("Cookie", "cookie1=value1; cookie2=value2")
                    //.WithBody(new StringContent("{ some JSON }", Encoding.UTF8, "application/json"))
                    //.WithCheck(response => Task.FromResult(response.IsSuccessStatusCode))
            );

            var scenario = ScenarioBuilder.CreateScenario("test_nbomber", new[] {step})
                .WithWarmUpDuration(TimeSpan.FromSeconds(10))
                .WithLoadSimulations(new[]
                {
                    Simulation.InjectScenariosPerSec(copiesCount: 100, during: TimeSpan.FromSeconds(30))
                });

            NBomberRunner.RegisterScenarios(scenario)
                .RunInConsole();
        }
    }
}
