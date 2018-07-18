using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NBomber.Examples.CSharp.Scenarios.Http
{
    class HttpScenario
    {
        public static Scenario BuildScenario()
        {
            var httpClient = new HttpClient();

            var getGithubStep = RequestStep.Create("GET github.com/VIP-Logic/NBomber html", async _ =>
            {
                var request = CreateRequest();
                var response = await httpClient.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail(response.StatusCode.ToString());
            });

            return new ScenarioBuilder(scenarioName: "Test HTTP (https://github.com) with 100 concurrent users")                
                .AddTestFlow("GET flow", steps: new[] { getGithubStep }, concurrentCopies: 100)
                .Build(duration: TimeSpan.FromSeconds(10));
        }

        static HttpRequestMessage CreateRequest()
        {
            var msg = new HttpRequestMessage();
            msg.RequestUri = new Uri("https://github.com/VIP-Logic/NBomber");
            msg.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            msg.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
            return msg;
        }
    }
}
