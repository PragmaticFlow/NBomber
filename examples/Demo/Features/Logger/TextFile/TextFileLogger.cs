using NBomber.CSharp;
using Serilog;
using Serilog.Events;

namespace Demo.Features.Logger.TextFile;

public class TextFileLogger
{
    public void Run()
    {
        // Docs: https://nbomber.com/docs/nbomber/logger

        // By default, the logger writes logs only to a text file using Serilog.Sinks.File.
        // The default output folder for the logs is NBomber's report folder.
        // After finishing the load test the report folder path will be printed in the console.

        var scenario = Scenario.Create("hello_world_scenario", async context =>
        {
            await Task.Delay(1000);

            context.Logger.Debug("my log message: {0}", context.InvocationNumber);

            return Response.Ok();
        })
        .WithInit(context =>
        {
            context.Logger.Information("MY INIT");
            return Task.CompletedTask;
        })
        .WithClean(context =>
        {
            context.Logger.Information("MY CLEAN");
            return Task.CompletedTask;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            // .WithMinimumLogLevel(LogEventLevel.Warning)

            // or you can use JSON config
            // .LoadInfraConfig("infra-config.json")
            .Run();
    }
}
