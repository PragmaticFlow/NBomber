using System;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharpDev.HelloWorld;

public class EmptyScenario
{
    public void Run()
    {
        var scn1 = Scenario.Create("scenario_1", async context =>
        {
            await Task.Delay(1000);
            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(10)));

        var scn2 = Scenario.Create("scenario_2", async context =>
        {
            await Task.Delay(1000);
            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(10)));

        /// An empty scenario is useful when you want to create the scenario to do only initialization or cleaning and execute it separately.
        /// The need for this can be when you have a few scenarios with the same init logic, and you want to run this init logic only once.
        /// Instead of using workarounds, you can separate the init logic into the dedicated scenario.

        var initDbScn = Scenario.Empty("populate_database_scenario")
            .WithInit(async context =>
            {
                await Task.Delay(5000);
            })
            .WithClean(async context =>
            {
                await Task.Delay(5000);
            });

        NBomberRunner
            .RegisterScenarios(scn1, scn2, initDbScn)
            .Run();
    }
}
