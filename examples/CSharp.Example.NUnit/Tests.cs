using System;
using System.Threading.Tasks;

using NUnit.Framework;

using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Example.NUnit
{
    public class Tests
    {
        Scenario BuildScenario()
        {
            var step1 = Step.CreateRequest("Step", async _ =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                return Response.Ok();
            });

            return new ScenarioBuilder("Scenario")
                .AddTestFlow("Flow", steps: new[] { step1 }, concurrentCopies: 1)
                .Build(duration: TimeSpan.FromSeconds(10));
        }

        [Test]
        public void Test()
        {
            var scenario = BuildScenario();

            var assertions = new[] {
               Assertion.ForScenario(stats => stats.OkCount > 3),
               Assertion.ForTestFlow("Flow", stats => stats.FailCount < 10),
               Assertion.ForStep("Step", "Flow", stats => stats.OkCount > 3)
            };

            scenario.RunTest(assertions);
        }
    }
}
