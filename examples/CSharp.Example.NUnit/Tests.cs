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
            var step1 = Step.CreatePull("simple step", async _ =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
                return Response.Ok();
            });

            return ScenarioBuilder.CreateScenario("nunit hello world", step1)
                .WithConcurrentCopies(1)
                .WithDuration(TimeSpan.FromSeconds(2));
        }

        [Test]
        public void Test()
        {
            var assertions = new[] {
               Assertion.ForScenario(stats => stats.OkCount > 2),               
               Assertion.ForStep("simple step", stats => stats.OkCount > 2)
            };

            var scenario = BuildScenario().WithAssertions(assertions);

            NBomberRunner.RegisterScenarios(scenario)
                         .RunTest();            
        }
    }
}
