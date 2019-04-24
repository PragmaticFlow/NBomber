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
            var step1 = Step.Create("simple step", ConnectionPool.None, async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return Response.Ok(sizeBytes: 1024);
            });

            return ScenarioBuilder.CreateScenario("nunit hello world", step1);                
        }

        [Test]
        public void Test()
        {
            var assertions = new[] {               
               Assertion.ForStep("simple step", stats => stats.OkCount > 2, "OkCount > 2"),
               Assertion.ForStep("simple step", stats => stats.RPS > 8, "RPS > 8"),
               Assertion.ForStep("simple step", stats => stats.Percent75 >= 102, "Percent75 >= 1000"),
               Assertion.ForStep("simple step", stats => stats.DataMinKb == 1.0, "DataMinKb == 1.0"),
               Assertion.ForStep("simple step", stats => stats.AllDataMB >= 0.01, "AllDataMB >= 0.01")
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
