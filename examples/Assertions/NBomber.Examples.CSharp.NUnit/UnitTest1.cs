using NUnit.Framework;
using NBomber.Examples.CSharp.Scenarios.PubSub;
using NBomber.CSharp;
using System;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var scenario = SimplePushScenario.BuildScenario();

            var assertions = new [] {
               N.Assert.ForScenario(stats => stats.OkCount > 95),
               N.Assert.ForScenario(stats => stats.FailCount > 95),
               N.Assert.ForScenario(stats => stats.ExceptionCount > 95),
               N.Assert.ForScenario(stats => stats.OkCount >= 95),
               N.Assert.ForTestFlow("Flow name 23", stats => stats.FailCount < 10),
               N.Assert.ForTestFlow("test flow1", stats => stats.FailCount < 10),
               N.Assert.ForStep("Flow name 1", "step name 10", stats => stats.FailCount == 95),
               N.Assert.ForStep("Flow name 1", "step name 19", stats => Array.IndexOf(new [] { 80, 95}, stats.FailCount) > -1),
               N.Assert.ForStep("Flow name 2", "step name 29", stats => stats.OkCount > 80 && stats.OkCount > 95)
            };

            ScenarioRunner.ApplyAssertions(scenario, assertions);
        }
    }
}