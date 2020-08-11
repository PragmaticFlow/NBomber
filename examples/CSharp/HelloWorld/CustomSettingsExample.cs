using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.HelloWorld
{
    public class CustomScenarioSettings
    {
        public int TestField { get; set; }
        public int PauseMs { get; set; }
    }

    public class CustomSettingsExample
    {
        static CustomScenarioSettings _customSettings = new CustomScenarioSettings();

        static Task TestInit(IScenarioContext context)
        {
            _customSettings = context.CustomSettings.Get<CustomScenarioSettings>();

            context.Logger.Information(
                "test init received CustomSettings.TestField '{TestField}'",
                _customSettings.TestField
            );

            return Task.CompletedTask;
        }

        public static void Run()
        {
            var step = Step.Create("step", async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1));

                context.Logger.Debug(
                    "step received CustomSettings.TestField '{TestField}'",
                    _customSettings.TestField
                );

                return Response.Ok(); // this value will be passed as response for the next step
            });

            var customPause = Step.CreatePause(() => _customSettings.PauseMs);

            var scenario = ScenarioBuilder
                .CreateScenario("my_scenario", step, customPause)
                .WithoutWarmUp()
                .WithInit(TestInit)
                .WithLoadSimulations(new []
                {
                    Simulation.KeepConstant(copies: 1, TimeSpan.FromSeconds(30))
                });

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithTestSuite("example")
                .WithTestName("custom_settings")
                .LoadConfig("./HelloWorld/config.json")
                .Run();
        }
    }
}
