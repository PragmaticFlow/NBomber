using System;
using System.Net.Http;
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

            var step = Step.Create("fetch_html_page", async context =>
            {
                var response = await httpClient.GetAsync("https://nbomber.com", context.CancellationToken);

                return response.IsSuccessStatusCode
                    ? Response.Ok(statusCode: (int) response.StatusCode)
                    : Response.Fail(statusCode: (int) response.StatusCode);
            });

            var scenario = ScenarioBuilder
                .CreateScenario("simple_http", step)
                .WithWarmUpDuration(Seconds(5))
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 1, during: TimeSpan.FromSeconds(30))
                )
                .WithThresholds(
                    Threshold.RequestAllCount(x => x > 200, "request all count > 200"),
                    Threshold.RequestOkCount(x => x > 190),
                    Threshold.RequestFailedCount(x => x <= 10),
                    Threshold.RequestFailedRate(x => x < 0.1),
                    Threshold.RPS(GetRpsConfig),
                    Threshold.LatencyMin(x => x < 100),
                    Threshold.LatencyMean(x => x < 400),
                    Threshold.LatencyMax(x => x < 500),
                    Threshold.LatencyStdDev(x => x is > 100 and < 200, "latency standard deviation > 50 and < 100"),
                    Threshold.LatencyPercent50(x => x < 300),
                    Threshold.LatencyPercent75(x => x < 320),
                    Threshold.LatencyPercent95(x => x < 400),
                    Threshold.LatencyPercent99(x => x < 500),
                    Threshold.DataTransferAllBytes(x => x < 10000)
                );

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }

        private static bool GetRpsConfig(double x)
        {
            return x > 20.0;
        }
    }
}
