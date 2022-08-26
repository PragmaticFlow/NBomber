namespace CSharpDev.Features;

using System;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;

public class CustomStepExecControlExample
{
    public static void Run()
    {
        var step1 = Step.Create("step_1", async context =>
        {
            context.Logger.Information($"{context.StepName} invoked");
            await Task.Delay(TimeSpan.FromSeconds(0.1));
            return Response.Ok();
        });

        var step2 = Step.Create("step_2", async context =>
        {
            context.Logger.Information($"{context.StepName} invoked");
            await Task.Delay(TimeSpan.FromSeconds(0.1));
            return Response.Ok();
        });

        var step3 = Step.Create("step_3", async context =>
        {
            context.Logger.Information($"{context.StepName} invoked");
            await Task.Delay(TimeSpan.FromSeconds(0.1));
            return Response.Ok();
        });

        var scenario = ScenarioBuilder
            .CreateScenario("my_scenario", step1, step2, step3)
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.KeepConstant(1, TimeSpan.FromSeconds(10)))
            .WithStepInterception(context =>
            {
                // step_1 will never be invoked
                if (context.IsSome)
                {
                    if (!context.Value.PrevStepResponse.IsError)
                        return ValueOption.Some("step_2");
                }
                return ValueOption.Some("step_3");
            });

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
