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

            ScenarioRunner.Run(scenario);
        }
    }
}
