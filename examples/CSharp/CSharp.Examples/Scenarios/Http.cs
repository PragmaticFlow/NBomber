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
            HttpRequestMessage CreateHttpRequest(Uri requestUrl)
            {
                var msg = new HttpRequestMessage();
                msg.RequestUri = requestUrl;
                msg.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                msg.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
                return msg;
            }

            var httpClient = new HttpClient();
            var url = new Uri("https://nbomber.com");

            var step1 = Step.Create("GET html", ConnectionPool.None, async context =>
            {
                var request = CreateHttpRequest(url);

                var response = await httpClient.SendAsync(request, context.CancellationToken);

                var responseSize = response.Content.Headers.ContentLength.HasValue
                    ? Convert.ToInt32(response.Content.Headers.ContentLength.Value)
                    : 0;

                return response.IsSuccessStatusCode
                    ? Response.Ok(sizeBytes: responseSize)
                    : Response.Fail();
            });

            return ScenarioBuilder.CreateScenario("nbomber.com", step1);                           
        }
    }
}
