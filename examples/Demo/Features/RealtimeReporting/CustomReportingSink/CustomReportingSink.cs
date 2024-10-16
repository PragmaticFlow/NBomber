using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using Serilog;

namespace Demo.Features.RealtimeReporting.CustomReportingSink;

class CustomReportingSink : IReportingSink
{
    private ILogger _log;
    public string SinkName => nameof(CustomReportingSink);

    public Task Init(IBaseContext context, IConfiguration infraConfig)
    {
        _log = context.Logger.ForContext<CustomReportingSink>();
        return Task.CompletedTask;
    }

    public Task Start(SessionStartInfo sessionInfo) => Task.CompletedTask;
    public Task SaveRealtimeStats(ScenarioStats[] stats) => Task.CompletedTask;
    public Task SaveFinalStats(NodeStats stats) => Task.CompletedTask;
    public Task Stop() => Task.CompletedTask;

    public void Dispose()
    { }
}

public class CustomReportingExample
{
    public void Run()
    {
        var scenario = Scenario.Create("simple_scenario", async context =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            return Response.Ok(sizeBytes: 100);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(1, TimeSpan.FromMinutes(1)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportingSinks(new CustomReportingSink())
            .WithReportingInterval(TimeSpan.FromSeconds(10))
            .Run();
    }
}
