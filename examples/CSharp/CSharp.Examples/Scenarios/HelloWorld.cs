using System;
using System.Threading.Tasks;

using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Extensions;

namespace CSharp.Examples.Scenarios
{
    class HelloWorldScenario
    {
        public static void Run()
        {
            var step1 = Step.Create("step_1", async context =>
            {
                // you can do any logic here: go to http, websocket etc

                await Task.Delay(TimeSpan.FromMilliseconds(200));
                return Response.Ok(42); // this value will be passed as response for the next step
            });

            var step2 = Step.Create("step_2", async context =>
            {
                // you can do any logic here: go to http, websocket etc

                await Task.Delay(TimeSpan.FromMilliseconds(200));
                var value = context.GetPreviousStepResponse<int>(); // 42
                return Response.Ok();
            });

            var scenario = ScenarioBuilder
                .CreateScenario("hello_world_scenario", new[] { step1, step2 })
                .WithWarmUpDuration(TimeSpan.FromSeconds(10))
                .WithLoadSimulations(new []
                {
                    Simulation.RampConcurrentScenarios(copiesCount: 10, during: TimeSpan.FromSeconds(20)),
                    Simulation.KeepConcurrentScenarios(copiesCount: 10, during: TimeSpan.FromMinutes(1)),
                    // Simulation.RampScenariosPerSec(copiesCount: 10, during: TimeSpan.FromSeconds(20)),
                    // Simulation.InjectScenariosPerSec(copiesCount: 10, during: TimeSpan.FromMinutes(1))
                });

            NBomberRunner
                .RegisterScenarios(new[] {scenario})
                .RunInConsole();
        }
    }
}
