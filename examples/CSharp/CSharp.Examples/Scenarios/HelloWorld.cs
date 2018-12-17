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
            var pool = ConnectionPool.None;

            var step1 = Step.CreatePull("simple step", pool, async context =>
            {
                // you can do any logic here: go to http, websocket etc
                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return Response.Ok();
            });

            return ScenarioBuilder.CreateScenario("Hello World from NBomber!", step1);
        }
    }
}
