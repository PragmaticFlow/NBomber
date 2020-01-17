using System;
using System.Linq;
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
            var step = Step.Create("simple step", async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return Response.Ok(sizeBytes: 1024);
            });

            return ScenarioBuilder.CreateScenario("nunit hello world", new[] { step });                
        }

        [Test]
        public void Test()
        {
            var scenario = BuildScenario()
                .WithConcurrentCopies(1)
                .WithOutWarmUp()
                .WithDuration(TimeSpan.FromSeconds(2));

            var allStats = NBomberRunner.RegisterScenarios(scenario).RunTest();
            var stepStats = allStats.First(x => x.StepName == "simple step");
            
            Assert.IsTrue(stepStats.OkCount > 2, "OkCount > 2");
            Assert.IsTrue(stepStats.RPS > 8, "RPS > 8");
            Assert.IsTrue(stepStats.Percent75 >= 100, "Percent75 >= 100");
            Assert.IsTrue(stepStats.DataMinKb == 1.0, "DataMinKb == 1.0");
            Assert.IsTrue(stepStats.AllDataMB >= 0.01, "AllDataMB >= 0.01");
        }
    }
}
