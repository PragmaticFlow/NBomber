using NBomber.CSharp;

namespace CSharpProd.HelloWorld.LoadSimulation;

public class ParallelScenarios
{
    public void Run()
    {
        // In this example, we will register and execute 2 scenarios in parallel.

        var scenario1 = Scenario.Create("scenario_1", async context =>
        {
            await Task.Delay(1000);
            return Response.Ok(statusCode: "200", sizeBytes: 1000);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 10, TimeSpan.FromMinutes(1))
        );

        var scenario2 = Scenario.Create("scenario_2", async context =>
        {
            await Task.Delay(1000);
            return Response.Ok(statusCode: "203");
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingConstant(copies: 50, during: TimeSpan.FromSeconds(30)),
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario1, scenario2) // here, we register two scenarios that will run in parallel

            // by default all registered scenarios will be executed in parallel
            // but it can be controlled via NBomberRunner.WithTargetScenario(string[]) API method
            // or JSON config: TargetScenarios: string[]
            // or CLI args: --target=scenario_2

            //.WithTargetScenario("scenario_2")

            .Run();
    }
}
