using System;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharpDev.HelloWorld
{
    public class HelloWorldExample
    {
        public static void Run()
        {
            var step1 = Step.Create("step_1", async context =>
            {
                // you can do any logic here: go to http, websocket etc

                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return Response.Ok(42); // this value will be passed as response for the next step
            });

            var pause = Step.CreatePause(TimeSpan.FromMilliseconds(100));

            var step2 = Step.Create("step_2", async context =>
            {
                var value = context.GetPreviousStepResponse<int>(); // 42
                return Response.Ok();
            });

            // here you create scenario and define (default) step order
            // you also can define them in opposite direction, like [step2; step1]
            // or even repeat [step1; step1; step1; step2]
            var scenario = ScenarioBuilder
                .CreateScenario("hello_world_scenario", step1, pause, step2)
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
                );

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithTestSuite("example")
                .WithTestName("hello_world_test")
                .Run();
        }
    }
}
