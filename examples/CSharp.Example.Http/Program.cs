using NBomber.CSharp;

namespace CSharp.Example.Http
{
    class Program
    {
        static void Main(string[] args)
        {            
            var scenario = HttpScenario.BuildScenario();
            scenario.Run();
        }
    }
}
