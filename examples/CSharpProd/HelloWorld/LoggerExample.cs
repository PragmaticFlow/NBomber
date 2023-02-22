using NBomber.CSharp;
using Serilog;
using Serilog.Events;

namespace CSharpProd.HelloWorld;

public class LoggerExample
{
    public void Run()
    {
        var scenario = Scenario.Create("hello_world_scenario", async context =>
        {
            await Task.Delay(1000);
            
            context.Logger.Verbose("my log message: {0}", context.InvocationNumber);

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
        );

    NBomberRunner
        .RegisterScenarios(scenario)
        
        // option 1
        .WithMinimumLogLevel(LogEventLevel.Verbose)
        
        // option 2
        
        // .WithLoggerConfig(() => 
        //     new LoggerConfiguration()
        //         .MinimumLevel.Verbose()
        //         .WriteTo.File(
        //             path: "my-log.txt",
        //             outputTemplate:
        //             "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [ThreadId:{ThreadId}] {Message:lj}{NewLine}{Exception}",
        //             rollingInterval: RollingInterval.Day)
        //             // buffered: true
        // )
        .Run();
    }
}