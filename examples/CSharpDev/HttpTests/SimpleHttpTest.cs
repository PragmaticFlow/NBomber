using System;
using System.Net.Http;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharpDev.HttpTests
{
    public class SimpleHttpTest
    {
        public static void Run()
        {
            using var httpClient = new HttpClient();

            var step = Step.CreateAsync("fetch_html_page", async context =>
            {
                var response = await httpClient.GetAsync("https://nbomber.com");

                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail();
            });

            var scenario = ScenarioBuilder
                .CreateScenario("simple_http", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 1, during: TimeSpan.FromSeconds(30))
                );

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}
