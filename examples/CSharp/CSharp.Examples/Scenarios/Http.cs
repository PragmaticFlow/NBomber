using System;
using System.Net.Http;

using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Examples.Scenarios
{
    class HttpScenario
    {
        public static Scenario BuildScenario()
        {
            HttpRequestMessage CreateHttpRequest()
            {
                var msg = new HttpRequestMessage();
                msg.RequestUri = new Uri("https://github.com/PragmaticFlow/NBomber");
                msg.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                msg.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
                return msg;
            }

            var httpPool = ConnectionPool.Create("http pool", () => new HttpClient());

            var step1 = Step.CreatePull("GET html", httpPool, async context =>
            {
                var request = CreateHttpRequest();
                var response = await context.Connection.SendAsync(request);
                var responseSizeKB = response.Content.Headers.ContentLength.HasValue
                    ? response.Content.Headers.ContentLength.Value / 1024
                    : 0;

                return response.IsSuccessStatusCode
                    ? Response.Ok(sizeKB: Convert.ToInt32(responseSizeKB))
                    : Response.Fail(response.StatusCode.ToString());
            });

            return ScenarioBuilder.CreateScenario("test_github", step1);                           
        }
    }
}
