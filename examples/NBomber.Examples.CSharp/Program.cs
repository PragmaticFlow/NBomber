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

            var asserts = new [] {
               Assert.ForScenario(stats => stats.OkCount > 95),
               Assert.ForScenario(stats => stats.FailCount > 95),
               Assert.ForScenario(stats => stats.ExceptionCount > 95),
               Assert.ForScenario(stats => stats.OkCount >= 95),
               Assert.ForTestFlow("Flow name 23", stats => stats.FailCount < 10),
               Assert.ForTestFlow("test flow1", stats => stats.FailCount < 10),
               Assert.ForStep("Flow name 1", "step name 10", stats => stats.FailCount == 95),
               Assert.ForStep("Flow name 1", "step name 19", stats => Array.IndexOf(new [] { 80, 95}, stats.FailCount) > -1),
               Assert.ForStep("Flow name 2", "step name 29", stats => stats.OkCount > 80 && stats.OkCount > 95)
            };

            ScenarioRunner.RunWithAssertions(scenario, asserts);
        }
    }
}
