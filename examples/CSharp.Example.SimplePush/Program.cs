using NBomber.CSharp;

namespace CSharp.Example.SimplePush
{
    class Program
    {
        static void Main(string[] args)
        {
            var scenario = PushScenario.BuildScenario();
            scenario.Run();
        }
    }
}
