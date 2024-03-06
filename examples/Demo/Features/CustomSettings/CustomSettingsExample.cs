using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;

namespace Demo.Features.CustomSettings;

public class CustomScenarioSettings
{
    public int MyTestField { get; set; }
    public int MyPauseMs { get; set; }
}

public class GlobalScenarioSettings
{
    public string ConnectionString { get; set; }
}

public class CustomSettingsExample
{
    CustomScenarioSettings _customSettings = new();

    Task Init(IScenarioInitContext initContext)
    {
        _customSettings = initContext.CustomSettings.Get<CustomScenarioSettings>();

        // if you want some settings to be shared globally among all scenarios
        // you can use GlobalCustomSettings for this
        var globalSettings = initContext.GlobalCustomSettings.Get<GlobalScenarioSettings>();
        initContext.Logger.Information(
            "test init received GlobalSettings.ConnectionString '{0}'",
            globalSettings.ConnectionString
        );

        initContext.Logger.Information(
            "test init received CustomSettings.MyTestField '{0}'",
            _customSettings.MyTestField
        );

        return Task.CompletedTask;
    }

    public void Run()
    {
        var scenario = Scenario.Create("my_scenario", async context =>
        {
            await Task.Delay(_customSettings.MyPauseMs);

            var step = await Step.Run("step", context, async () =>
            {
                await Task.Delay(1_000);
                context.Logger.Debug("step received CustomSettings.MyTestField '{0}'", _customSettings.MyTestField);
                return Response.Ok();
            });

            return Response.Ok();
        })
        .WithInit(Init)
        .WithLoadSimulations(Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)))
        .WithWarmUpDuration(TimeSpan.FromSeconds(10))
        .WithMaxFailCount(1_000);

        NBomberRunner
            .RegisterScenarios(scenario)
            .LoadConfig("./Features/CustomSettings/config.json")
            .WithTestSuite("my test suite")
            .WithTestName("my test name")
            .WithTargetScenarios("my_scenario")
            .WithReportFileName("my_report")
            .WithReportFolder("report_folder")
            .WithReportFormats(ReportFormat.Txt, ReportFormat.Html)
            .WithReportingInterval(TimeSpan.FromSeconds(10))
            .Run();
    }
}

