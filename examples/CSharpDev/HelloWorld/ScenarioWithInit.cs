using System;
using System.Threading.Tasks;
using NBomber.Contracts;
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
            context.Logger.Information("MY INIT");
            return Task.CompletedTask;
        })
        .WithClean(context =>
        {
            context.Logger.Information("MY CLEAN");
            return Task.CompletedTask;
        });

        NBomberRunner
            .RegisterScenarios(scn1)
            .Run();
    }
}
