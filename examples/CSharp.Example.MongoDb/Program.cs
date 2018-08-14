using NBomber.CSharp;

namespace CSharp.Example.MongoDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var scenario = MongoScenario.BuildScenario();
            scenario.Run();
        }
    }
}
