using System;
using System.Net.Http;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharpDev.Http;

public class SimpleHttpTest
{
    public void Run()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("http_scenario", async context =>
        {
            var httpResponse = await httpClient.GetAsync("https://nbomber.com");
            return httpResponse.IsSuccessStatusCode
                ? Response.Ok()
                : Response.Fail();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(1, TimeSpan.FromMinutes(0.1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
