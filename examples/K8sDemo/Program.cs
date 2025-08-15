using NBomber.CSharp;

var scenario = Scenario.Create("my_scenario", async context =>
{
    await Task.Delay(500);

    return Response.Ok();
});

NBomberRunner
    .RegisterScenarios(scenario)
    .Run(args);
