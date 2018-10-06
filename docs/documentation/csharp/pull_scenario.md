# Testing HTTP via Pull Scenario

```csharp
using System;
using System.Net.Http;

using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Example.Http
{
    class Program
    {
        static Scenario BuildScenario()
        {
            HttpRequestMessage CreateHttpRequest()
            {
                var msg = new HttpRequestMessage();
                msg.RequestUri = new Uri("https://github.com/PragmaticFlow/NBomber");
                msg.Headers.TryAddWithoutValidation("Accept", "text/html");                
                return msg;
            }

            var httpClient = new HttpClient();

            var step1 = Step.CreateRequest("GET html", async _ =>
            {
                var request = CreateHttpRequest();
                var response = await httpClient.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail(response.StatusCode.ToString());
            });

            return new ScenarioBuilder(scenarioName: "Test HTTP https://github.com")
                .AddTestFlow("GET flow", steps: new[] { step1 }, concurrentCopies: 100)
                .Build(duration: TimeSpan.FromSeconds(10));
        }

        static void Main(string[] args)
        {            
            var scenario = BuildScenario();
            scenario.RunInConsole();
        }
    }
}

```