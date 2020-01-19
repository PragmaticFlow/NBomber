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
            var step = Step.Create("step", async context =>
            {
                // you can do any logic here: go to http, websocket etc

                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return Response.Ok();
            });

            var scenario = ScenarioBuilder.CreateScenario("Hello World!", new[] { step });

            NBomberRunner.RegisterScenarios(scenario)
                         .RunInConsole();
        }
    }
}
