using System;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Sinks.InfluxDB;

namespace CSharpProd.RealtimeReporting
{
    public class InfluxDbReporting
    {
        public static void Run()
        {
            var step = Step.Create("step", async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return Response.Ok(sizeBytes: 100);
            });

            var scenario = ScenarioBuilder
                .CreateScenario("simple_scenario", step)
                .WithoutWarmUp()
                .WithLoadSimulations(Simulation.KeepConstant(1, TimeSpan.FromMinutes(1)));

            var influxConfig = InfluxDbSinkConfig.Create("http://localhost:8086", dbName: "default");
            var influxDb = new InfluxDBSink(influxConfig);

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithTestSuite("reporting")
                .WithTestName("influx_test")
                .WithReportingSinks(
                    reportingSinks: new[] {influxDb},
                    sendStatsInterval: TimeSpan.FromSeconds(10)
                )
                .Run();
        }
    }
}
