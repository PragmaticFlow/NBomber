using System;
using System.Threading.Tasks;

using NUnit.Framework;

using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Examples.NUnit
{
    public class Tests
    {
        Scenario BuildScenario()
        {   
            var step1 = Step.CreateAction("simple step", ConnectionPool.None, async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
                return Response.Ok();
            });

            return ScenarioBuilder.CreateScenario("nunit hello world", step1);                
        }

        [Test]
        public void Test()
        {
            var assertions = new[] {               
               Assertion.ForStep("simple step", stats => stats.OkCount > 2, "OkCount > 2")
            };

            var scenario = BuildScenario()
                .WithConcurrentCopies(1)
                .WithWarmUpDuration(TimeSpan.FromSeconds(0))
                .WithDuration(TimeSpan.FromSeconds(2))
                .WithAssertions(assertions);

            NBomberRunner.RegisterScenarios(scenario)
                         .RunTest();            
        }
    }
}
