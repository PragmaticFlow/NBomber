using System;
using System.Threading.Tasks;

namespace CSharpDev.ClientFactory;

using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;

public class CustomDistributionExample
{
    public static void Run()
    {
        var factory = ClientFactory.Create(
            name: "test_pool",
            initClient: (number, context) => Task.FromResult(5),
            clientCount: 5
        );

        var step = Step.Create(
            name: "step",
            clientFactory: factory,
            clientDistribution: (context) => 5, // always returns client_index = 5 from client factory
            execute: async (context) =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                return context.Client == 5
                    ? Response.Ok()
                    : Response.Fail();
            });

        var scenario = ScenarioBuilder
            .CreateScenario("custom_distribution_example", step)
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.KeepConstant(1, TimeSpan.FromSeconds(10)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();

    }
}
