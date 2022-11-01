using System;
using System.Threading.Tasks;
using NBomber.CSharp;

namespace CSharpDev.HelloWorld;

public class ScenarioWithStepRetry
{
    public void Run()
    {
        // In this example, we will execute a step, and when the response finished with an error,
        // we should retry this step until it succeeds.

        // For this we should set Scenario.WithResetIterationOnFail(false) to specify that iteration
        // should continue even when step returns Response.Fail().
        // It's necessary to check the response on an error and retry it until it succeeds.

        var scenario = Scenario.Create("hello_world_scenario", async context =>
        {
            var counter = 0;

            var step1Response = Response.Fail<string>();

            while (step1Response.IsError)
            {
                step1Response = await Step.Run("step_1", context, async () =>
                {
                    counter += 1;
                    await Task.Delay(1000);

                    return counter == 3
                        ? Response.Ok(payload: "ok response")
                        : Response.Fail<string>();
                });
            }

            return Response.Ok();
        })
        .WithResetIterationOnFail(false) // the iteration should continue even when step returns fail
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
