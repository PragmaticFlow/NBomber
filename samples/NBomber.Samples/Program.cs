using NBomber.Samples.Scenarios.Mongo;
using System;

namespace NBomber.Samples
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
