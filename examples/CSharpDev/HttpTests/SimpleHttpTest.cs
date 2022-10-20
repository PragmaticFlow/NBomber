using System;
using System.Net.Http;
using System.Threading.Tasks;
using HdrHistogram;
using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;
using static NBomber.Time;

namespace CSharpDev.HttpTests
{
    public class SimpleHttpTest
    {
        public static void Run()
        {
            using var httpClient = new HttpClient();

            var scenario = Scenario.Create("http_scenario", async context =>
            {
                var getJwt = await Step.Run("get_jwt", context, async () =>
                {
                    var jwt = await httpClient.GetStringAsync("https://nbomber.com");
                    return Response.Ok(payload: jwt);
                });

                var jwt = getJwt.Payload.Value;

                var login = await Step.Run("login", context, async () =>
                {
                    var response = await httpClient.GetAsync($"https://authenticate.com/login/{getJwt.Payload.Value}");
                    return response.IsSuccessStatusCode
                        ? Response.Ok()
                        : Response.Fail();
                });

                return Response.Ok();
            })
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.KeepConstant(1, TimeSpan.FromSeconds(20)));

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            //scenario

            // var step = Step.Create("fetch_html_page", async context =>
            // {
            //     var response = await httpClient.GetAsync("https://nbomber.com", context.CancellationToken);
            //
            //     return response.IsSuccessStatusCode
            //         ? Response.Ok(statusCode: (int) response.StatusCode)
            //         : Response.Fail(statusCode: (int) response.StatusCode);
            // });

            // var scenario = ScenarioBuilder
            //     .CreateScenario("simple_http", step)
            //     .WithWarmUpDuration(Seconds(5))
            //     .WithLoadSimulations(
            //         Simulation.InjectPerSec(rate: 1, during: TimeSpan.FromSeconds(30))
            //     );
            //
            // NBomberRunner
            //     .RegisterScenarios(scenario)
            //     .Run();
        }
    }
}
