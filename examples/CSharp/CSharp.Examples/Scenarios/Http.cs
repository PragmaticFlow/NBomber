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
            // it's a very basic HTTP example, don't use it for production testing
            // for production purposes use NBomber.Http which use performance optimizations
            // you can find more here: https://github.com/PragmaticFlow/NBomber.Http

            var httpClient = new HttpClient();            

            var step = Step.Create("GET html", async context =>
            {   
                var response = await httpClient.GetAsync(
                    "https://gitter.im", 
                    context.CancellationToken);

                return response.IsSuccessStatusCode
                    ? Response.Ok(sizeBytes: (int)response.Content.Headers.ContentLength.Value)
                    : Response.Fail();
            });

            return ScenarioBuilder.CreateScenario("test_gitter", step);                           
        }
    }
}
