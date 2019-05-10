using System;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Examples.Scenarios
{
    class HelloWorldScenario
    {
        public static Scenario BuildScenario()
        {   
            var step1 = Step.Create("simple step", ConnectionPool.None, async context =>
            {
                // you can do any logic here: go to http, websocket etc
                await Task.Delay(TimeSpan.FromSeconds(0.1), context.CancellationToken);
                return Response.Ok();
            });
            return ScenarioBuilder.CreateScenario("Hello World from NBomber!", step1);
        }

        public static Scenario BuildPlainScenario()
        {

            //Just a sample how to create no-context step
            var step1 = Step.Create("plain step", () => Task.FromResult(Response.Ok()));
            return ScenarioBuilder.CreateScenario("Hello World from NBomber struggle continues!", step1);
        }
    }

    class HelloWorld2Scenario
    {
        public static Scenario BuildScenario()
        {

            //Just a sample how to create no-context step
            var step1 = Step.Create("plain step", async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return Response.Ok();
            });
            return ScenarioBuilder.CreateScenario("Hello World from NBomber struggle continues!", step1);
        }
    }
}
