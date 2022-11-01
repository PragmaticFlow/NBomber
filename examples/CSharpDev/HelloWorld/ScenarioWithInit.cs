using System;
using System.Threading.Tasks;
using NBomber.CSharp;

namespace CSharpDev.HelloWorld;

public class ScenarioWithInit
{
    public void Run()
    {
        var scn1 = Scenario.Create("scenario_1", async context =>
        {
            await Task.Delay(1000);
            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(10)))
        .WithInit(context =>
        {
            // You can do here any initialization logic: populate the database, etc.
            context.Logger.Information("MY INIT");
            return Task.CompletedTask;
        })
        .WithClean(context =>
        {
            // You can do here any cleaning logic: clearing the database, etc.
            context.Logger.Information("MY CLEAN");
            return Task.CompletedTask;
        });

        NBomberRunner
            .RegisterScenarios(scn1)
            .Run();
    }
}
