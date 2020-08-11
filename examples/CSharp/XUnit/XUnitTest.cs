using System;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.XUnit
{
    // in this example we use:
    // - XUnit (https://xunit.net/)
    // - Fluent Assertions (https://fluentassertions.com/)
    // to get more info about test automation, please visit: (https://nbomber.com/docs/test-automation)

    public class XUnitTest
    {
        Scenario BuildScenario()
        {
            var step = Step.Create("simple step", async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return Response.Ok(sizeBytes: 1024);
            });

            return ScenarioBuilder.CreateScenario("xunit_hello_world", step);
        }

        [Fact]
        public void Test()
        {
            var scenario = BuildScenario()
                .WithoutWarmUp()
                .WithLoadSimulations(new[]
                {
                    Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(2))
                });

            var nodeStats = NBomberRunner.RegisterScenarios(scenario).Run();
            var stepStats = nodeStats.ScenarioStats[0].StepStats[0];

            stepStats.OkCount.Should().BeGreaterThan(2);
            stepStats.RPS.Should().BeGreaterThan(8);
            stepStats.Percent75.Should().BeGreaterOrEqualTo(100);
            stepStats.MinDataKb.Should().Be(1.0);
            stepStats.AllDataMB.Should().BeGreaterOrEqualTo(0.01);
        }
    }
}
