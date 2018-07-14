using System;
using NBomber.Examples.CSharp.Scenarios.Mongo;
using NBomber.Examples.CSharp.Scenarios.Http;

namespace NBomber.Examples.CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var scenario = MongoScenario.BuildScenario();
            //var scenario = HttpScenario.BuildScenario();

            ScenarioRunner.Run(scenario);            
        }
    }
}
