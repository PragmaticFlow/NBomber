using System;
using System.Threading.Tasks;
using NBomber.CSharp;

var scenario = Scenario.Create("hello_world_scenario", async context =>
{
    await Task.Delay(1000);

    return Response.Ok();
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.RampConstant(copies: 50, during: TimeSpan.FromMinutes(1)),
    Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();
