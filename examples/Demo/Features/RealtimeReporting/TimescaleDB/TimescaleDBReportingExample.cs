using NBomber.CSharp;
using NBomber.Sinks.Timescale;

public class TimescaleDBReportingExample
{
    public void Run()
    {
        var scenario = Scenario.Create("user_scenario", async context =>
        {
            var step1 = await Step.Run("login", context, async () =>
            {
                await Task.Delay(Random.Shared.Next(100, 500));
                return Response.Ok(sizeBytes: 10, statusCode: "200");
            });

            var step2 = await Step.Run("get_product", context, async () =>
            {
                await Task.Delay(Random.Shared.Next(500, 1000));
                return Response.Ok(sizeBytes: 20, statusCode: "200");
            });

            var step3 = await Step.Run("buy_product", context, async () =>
            {
                await Task.Delay(Random.Shared.Next(1000, 2000));
                var value = context.Random.Next(0, 4);

                if (value == 3)
                    return Response.Fail(sizeBytes: 30, statusCode: "400");

                return Response.Ok(statusCode: "200", sizeBytes: 30);
            });

            return Response.Ok(statusCode: "200");
        })
        .WithMaxFailCount(Int32.MaxValue)
        .WithWarmUpDuration(TimeSpan.FromSeconds(3))
        .WithLoadSimulations(
            Simulation.RampingInject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)), // rump-up to rate 200
            Simulation.Inject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)),       // keep injecting with rate 200
            Simulation.RampingInject(rate: 0, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1))    // rump-down to rate 0
        );

        var scenario2 = Scenario.Create("user_scenario_2", async context =>
        {
            var step1 = await Step.Run("login", context, async () =>
            {
                await Task.Delay(Random.Shared.Next(100, 500));
                return Response.Ok(sizeBytes: 10, statusCode: "200");
            });

            var step2 = await Step.Run("get_product", context, async () =>
            {
                await Task.Delay(Random.Shared.Next(500, 1000));
                return Response.Ok(sizeBytes: 20, statusCode: "200");
            });

            var step3 = await Step.Run("buy_product", context, async () =>
            {
                await Task.Delay(Random.Shared.Next(1000, 2000));
                var value = context.Random.Next(0, 4);

                if (value == 3)
                    return Response.Fail(sizeBytes: 30, statusCode: "400");

                return Response.Ok(statusCode: "200", sizeBytes: 30);
            });

            return Response.Ok(statusCode: "201");
        })
        .WithMaxFailCount(Int32.MaxValue)
        .WithWarmUpDuration(TimeSpan.FromSeconds(3))
        .WithLoadSimulations(
            Simulation.RampingInject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)), // rump-up to rate 200
            Simulation.Inject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)),       // keep injecting with rate 200
            Simulation.RampingInject(rate: 0, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1))    // rump-down to rate 0
        );

        NBomberRunner
            .RegisterScenarios(scenario, scenario2)
            .LoadInfraConfig("Features/RealtimeReporting/TimescaleDB/infra-config.json")
            .WithReportingInterval(TimeSpan.FromSeconds(5))
            .WithReportingSinks(new TimescaleDbSink())
            .WithTestSuite("reporting")
            .WithTestName("timescale_db_demo")
            .Run();
    }
}
