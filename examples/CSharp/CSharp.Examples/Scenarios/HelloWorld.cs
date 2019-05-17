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
            var step = Step.Create("step 1", async _ =>
            {
                // you can do any logic here: go to http, websocket etc

                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return Response.Ok();
            });

            return ScenarioBuilder.CreateScenario("Hello World!", step);
        }
    }
}
