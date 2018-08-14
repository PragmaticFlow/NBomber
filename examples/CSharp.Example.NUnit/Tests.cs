using System;
using NUnit.Framework;
using NBomber.CSharp;

namespace CSharp.Example.NUnit
{
    public class Tests
    {
        [Test]
        public void Test()
        {
            var scenario = PushScenario.BuildScenario();

            var assertions = new[] {
               Assertion.ForScenario(stats => stats.OkCount > 95),               
               Assertion.ForTestFlow("test flow1", stats => stats.FailCount < 10),               
               Assertion.ForStep("Flow name 2", "step name 29", stats => stats.OkCount > 80)
            };

            scenario.RunTest(assertions);
        }
    }
}
