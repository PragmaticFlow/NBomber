using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Serilog;

using NBomber.Configuration;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using static NBomber.Time;

namespace CSharpDev.CustomReporting
{
    class CustomReportingSink : IReportingSink
    {
        private ILogger _logger;
        public string SinkName => nameof(CustomReportingSink);

        public Task Init(IBaseContext context, IConfiguration infraConfig)
        {
            _logger = context.Logger;
            return Task.CompletedTask;
        }

        public Task Start() => Task.CompletedTask;
        public Task SaveRealtimeStats(ScenarioStats[] stats) => Task.CompletedTask;
        public Task SaveFinalStats(NodeStats[] stats) => Task.CompletedTask;
        public Task Stop() => Task.CompletedTask;

        public void Dispose()
        { }
    }

    public class CustomReporting
    {
        public static void Run()
        {
            var step = Step.Create("step", async context =>
            {
                await Task.Delay(Seconds(0.1));
                return Response.Ok(sizeBytes: 100);
            });

            var scenario = ScenarioBuilder
                .CreateScenario("simple_scenario", step)
                .WithoutWarmUp()
                .WithLoadSimulations(Simulation.KeepConstant(1, TimeSpan.FromMinutes(1)));

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithTestSuite("reporting")
                .WithTestName("custom_reporting_test")
                .WithReportingSinks(new CustomReportingSink())
                .WithReportingInterval(Seconds(10))
                .WithReportFolder("./custom_reports")
                .WithReportFormats(ReportFormat.Html, ReportFormat.Txt, ReportFormat.Csv)
                .Run();
        }
    }
}
