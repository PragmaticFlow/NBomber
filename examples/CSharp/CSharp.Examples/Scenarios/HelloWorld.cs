using System;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Examples.Scenarios
{
    class HelloWorldScenario
    {
        public static void Run()
        {
            var step1 = Step.Create("step_1", async context =>
            {
                // you can do any logic here: go to http, websocket etc

                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return Response.Ok(42); // this value will be passed as response for the next step
            });

            var step2 = Step.Create("step_1", context =>
            {
                var value = context.GetPreviousStepResponse<int>(); // 42
                return Task.FromResult(Response.Ok());
            });

            var scenario = ScenarioBuilder.CreateScenario("Hello World!", new[] { step1, step2 });

            NBomberRunner.RegisterScenarios(scenario)
                         .RunInConsole();
        }
    }
}
