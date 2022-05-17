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
                    Threshold.RequestCount(
                        Threshold.AllCount(x => x > 200),
                        Threshold.OkCount(x => x > 190),
                        Threshold.FailedCount(x => x <= 10),
                        Threshold.FailedRate(x => x < 0.1),
                        Threshold.RPS(GetRpsConfig)
                    ),
                    Threshold.Latency(
                        Threshold.Min(x => x < 100),
                        Threshold.Mean(x => x < 400),
                        Threshold.Max(x => x < 500),
                        Threshold.StdDev(x => x is > 100 and < 200)
                    ),
                    Threshold.LatencyPercentile(
                        Threshold.P50(x => x < 300),
                        Threshold.P75(x => x < 320),
                        Threshold.P95(x => x < 400),
                        Threshold.P99(x => x < 500)
                    )
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
