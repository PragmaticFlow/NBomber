using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharpProd.Features.CustomSettings;

public class CustomScenarioSettings
{
    public int TestField { get; set; }
    public int PauseMs { get; set; }
}

public class CustomSettingsExample
{
    CustomScenarioSettings _customSettings = new();

    Task Init(IScenarioInitContext initContext)
    {
        _customSettings = initContext.CustomSettings.Get<CustomScenarioSettings>();

        initContext.Logger.Information(
            "test init received CustomSettings.TestField '{TestField}'",
            _customSettings.TestField
        );

        return Task.CompletedTask;
    }

    public void Run()
    {
        var scenario = Scenario.Create("my_scenario", async context =>
        {
            await Task.Delay(_customSettings.PauseMs);

            var step = await Step.Run("step", context, async () =>
            {
                context.Logger.Debug("step received CustomSettings.TestField '{0}'", _customSettings.TestField);
                return Response.Ok();
            });

            return Response.Ok();
        })
        .WithInit(Init)
        .WithoutWarmUp();

        NBomberRunner
            .RegisterScenarios(scenario)
            .LoadConfig("./Features/CustomSettings/config.json")
            .Run();
    }
}

