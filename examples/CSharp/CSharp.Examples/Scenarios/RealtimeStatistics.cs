using System;
using NBomber.CSharp;
using NBomber.Http.CSharp;
//using NBomber.Sinks.InfluxDB;

namespace CSharp.Examples.Scenarios
{
    public class RealtimeStatistics
    {
        public static void Run()
        {
            // var influxDb = new InfluxDBSink(url: "http://localhost:8086", dbName: "default");
            //
            // var step = HttpStep.Create("simple step", context =>
            //         Http.CreateRequest("GET", "https://nbomber.com")
            //             .WithHeader("Accept", "text/html")
            // );
            //
            // var scenario = ScenarioBuilder.CreateScenario("test_nbomber", new[] { step })
            //     .WithConcurrentCopies(50)
            //     .WithWarmUpDuration(TimeSpan.FromSeconds(10))
            //     .WithDuration(TimeSpan.FromSeconds(180));
            //
            // NBomberRunner.RegisterScenarios(scenario)
            //     .WithReportingSinks(new[] {influxDb }, sendStatsInterval: TimeSpan.FromSeconds(20))
            //     .RunInConsole();
        }
    }
}
