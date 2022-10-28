using CSharpDev.Features.CustomSettings;
using CSharpDev.HelloWorld;
using CSharpDev.Http;
using CSharpDev.Mqtt;

namespace CSharpDev
{
    class Program
    {
        static void Main(string[] args)
        {
            new HelloWorldExample().Run();
            // new ScenarioWithSteps().Run();
            // new ParallelScenarios().Run();
            // new ScenarioWithTimeout().Run();
            // new EmptyScenario().Run();

            // new SimpleHttpTest().Run();
            // new SimpleMqttTest().Run();
            // new CustomSettingsExample().Run();
        }
    }
}
