using System;

using NBomber.Examples.CSharp.Scenarios.Mongo;
using NBomber.Examples.CSharp.Scenarios.Http;
using NBomber.Examples.CSharp.Scenarios.PubSub;
using NBomber.CSharp;

namespace NBomber.Examples.CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //var scenario = MongoScenario.BuildScenario();
            //var scenario = HttpScenario.BuildScenario();  
            var scenario = SimplePushScenario.BuildScenario();

            var assertions = new [] {
               Assertion.ForScenario(stats => stats.OkCount > 95),
               Assertion.ForScenario(stats => stats.FailCount > 95),
               Assertion.ForScenario(stats => stats.ExceptionCount > 95),
               Assertion.ForScenario(stats => stats.OkCount >= 95),
               Assertion.ForTestFlow("Flow name 23", stats => stats.FailCount < 10),
               Assertion.ForTestFlow("test flow1", stats => stats.FailCount < 10),
               Assertion.ForStep("Flow name 1", "step name 10", stats => stats.FailCount == 95),
               Assertion.ForStep("Flow name 1", "step name 19", stats => Array.IndexOf(new [] { 80, 95}, stats.FailCount) > -1),
               Assertion.ForStep("Flow name 2", "step name 29", stats => stats.OkCount > 80 && stats.OkCount > 95)
            };

            ScenarioRunner.RunWithAssertions(scenario, assertions);
        }
    }
}
