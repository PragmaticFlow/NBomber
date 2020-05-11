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
                .WithoutWarmUp()
                .WithLoadSimulations(new[]
                {
                    Simulation.KeepConcurrentScenarios(copiesCount: 1, during: TimeSpan.FromSeconds(2))
                });

            var nodeStats = NBomberRunner.RegisterScenarios(new[] {scenario}).RunTest();
            var stepStats = nodeStats.ScenarioStats.First().StepStats.First();

            Assert.IsTrue(stepStats.OkCount > 2, "OkCount > 2");
            Assert.IsTrue(stepStats.RPS > 8, "RPS > 8");
            Assert.IsTrue(stepStats.Percent75 >= 100, "Percent75 >= 100");
            Assert.IsTrue(stepStats.MinDataKb == 1.0, "DataMinKb == 1.0");
            Assert.IsTrue(stepStats.AllDataMB >= 0.01, "AllDataMB >= 0.01");
        }
    }
}
