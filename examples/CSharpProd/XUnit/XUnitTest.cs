using System;
using System.Threading.Tasks;
using FluentAssertions;
using NBomber.Contracts;
using NBomber.CSharp;
using Xunit;

namespace CSharpProd.XUnit
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

            // todo stepStats.OkCount.Should().BeGreaterThan(2);
            stepStats.Ok.Request.RPS.Should().BeGreaterThan(8);
            stepStats.Ok.Latency.Percent75.Should().BeGreaterOrEqualTo(100);
            stepStats.Ok.DataTransfer.MinBytes.Should().Be(1024);
            stepStats.Ok.DataTransfer.AllBytes.Should().BeGreaterOrEqualTo(17408L);
        }
    }
}
