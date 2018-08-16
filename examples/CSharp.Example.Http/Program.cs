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
            HttpRequestMessage CreateRequest()
            {
                var msg = new HttpRequestMessage();
                msg.RequestUri = new Uri("https://github.com/VIP-Logic/NBomber");
                msg.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                msg.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
                return msg;
            }

            var httpClient = new HttpClient();

            var step1 = Step.CreateRequest("GET github.com/VIP-Logic/NBomber html", async _ =>
            {
                var request = CreateRequest();
                var response = await httpClient.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail(response.StatusCode.ToString());
            });

            return new ScenarioBuilder(scenarioName: "Test HTTP (https://github.com) with 100 concurrent users")
                .AddTestFlow("GET flow", steps: new[] { step1 }, concurrentCopies: 100)
                .Build(duration: TimeSpan.FromSeconds(10));
        }

        static void Main(string[] args)
        {            
            var scenario = BuildScenario();
            scenario.Run();
        }
    }
}
