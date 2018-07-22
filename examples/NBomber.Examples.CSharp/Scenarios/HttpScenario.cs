using System;
using System.Net.Http;
using NBomber.CSharp;

namespace NBomber.Examples.CSharp.Scenarios.Http
{
    class HttpScenario
    {
        public static Scenario BuildScenario()
        {
            var httpClient = new HttpClient();            

            var getGithubStep = StepFactory.Request("GET github.com/VIP-Logic/NBomber html", async _ =>
            {
                var request = CreateRequest();
                var response = await httpClient.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail(response.StatusCode.ToString());
            });

            //var asserts = new Assert[] {
            //    Assert.ForAll(stats => stats.OkCount > 95),
            //    Assert.ForAll(stats => stats.FailCount > 95),
            //    Assert.ForAll(stats => stats.ExceptionCount > 95),
            //    Assert.ForAll(stats => stats.OkCount >= 95),
            //    Assert.ForFlow("Flow name 23", stats => stats.FailCount < 10),
            //    Assert.ForStep("Flow name 1", "step name 10", stats => stats.FailCount == 95),
            //    Assert.ForStep("Flow name 1", "step name 19", stats => Array.IndexOf(new [] { 80, 95}, stats.FailCount) > -1),
            //    Assert.ForStep("Flow name 2", "step name 29", stats => stats.OkCount > 80 && stats.OkCount > 95)
            //};

            return new ScenarioBuilder(scenarioName: "Test HTTP (https://github.com) with 100 concurrent users")                
                .AddTestFlow("GET flow", steps: new[] { getGithubStep }, concurrentCopies: 100)
                //.AddAsserts(asserts)
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
