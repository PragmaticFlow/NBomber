using System;
using System.Threading.Tasks;
using NBomber.CSharp;

var scenario = Scenario.Create("hello_world_scenario", async context =>
{
    var step1 = await Step.Run("step_1", context, async () =>
    {
        await Task.Delay(1000);
        return Response.Ok(payload: "step_1 response", sizeBytes: 10);
    });

    var step2 = await Step.Run("step_2", context, async () =>
    {
        await Task.Delay(1000);
        return Response.Ok(payload: "step_2 response", sizeBytes: 10);
    });

    return step1.Payload.Value == "step_1 response" && step2.Payload.Value == "step_2 response"
        ? Response.Ok(statusCode: "200")
        : Response.Fail(statusCode: "500");
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(10))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();
