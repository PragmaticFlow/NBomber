using System;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace CSharp.Examples.Scenarios
{
    class HttpScenario
    {   
        public static void Run()
        {
            var step = HttpStep.Create("simple step", (context) =>
                    Http.CreateRequest("GET", "https://gitter.im")
                        .WithHeader("Accept", "text/html")
                //.WithHeader("Cookie", "cookie1=value1; cookie2=value2")
                //.WithBody(new StringContent("{ some JSON }", Encoding.UTF8, "application/json"))
                //.WithCheck(response => Task.FromResult(response.IsSuccessStatusCode))
            );

            var scenario = ScenarioBuilder.CreateScenario("test_gitter", step)
                .WithConcurrentCopies(100)
                .WithWarmUpDuration(TimeSpan.FromSeconds(10))
                .WithDuration(TimeSpan.FromSeconds(20));
            
            NBomberRunner.RegisterScenarios(scenario)
                //.LoadConfig("config.json")
                .RunInConsole();
        }
    }
}
