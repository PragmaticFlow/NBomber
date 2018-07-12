using NBomber.Examples.CSharp.Scenarios.Mongo;
using System;

namespace NBomber.Examples.CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var scenario = MongoScenario.Build();
            ScenarioRunner.Run(scenario);

            Console.ReadLine();
        }
    }
}
