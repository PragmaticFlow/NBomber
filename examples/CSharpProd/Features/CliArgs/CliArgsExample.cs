using NBomber.CSharp;

namespace CSharpProd.Features.CliArgs;

public class CliArgsExample
{
    public void Run()
    {
        var args = new[]
        {
            "--config", "./Features/CliArgs/config.json", // path to JSON config
            "--infra", "./Features/CliArgs/infra-config.json", // path to Infra config
            "--target", "scenario_2", "scenario_1" // target scenarios
        };

        var scenario1 = Scenario.Create("scenario_1", async context =>
        {
            await Task.Delay(1_000);
            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
        );

        var scenario2 = Scenario.Create("scenario_2", async context =>
        {
            await Task.Delay(1_000);
            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(10))
        );

        var scenario3 = Scenario.Create("scenario_3", async context =>
        {
            await Task.Delay(1_000);
            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario1, scenario2, scenario3)
            .Run(args);
    }
}
