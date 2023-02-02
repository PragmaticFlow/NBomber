using InfluxDB.Client.Writes;
using NBomber.CSharp;
using NBomber.Sinks.InfluxDB;

namespace CSharpProd.Features.RealtimeReporting.InfluxDB;

public class InfluxDBReportingExample
{
    // this reporting sink will save stats data into InfluxDB. Also you can use it to write your custom data (some counters, etc)
    private readonly InfluxDBSink _influxDbSink = new();
    
    public void Run()
    {
        var scenario = Scenario.Create("user_flow_scenario", async context =>
        {
            // here is an example how you can use InfluxDBSink to write your custom data.
            // var writeApi = _influxDbSink.InfluxClient.GetWriteApiAsync();
            //
            // var point = PointData
            //     .Measurement("nbomber")
            //     .Field("my_custom_counter", 1);
            //
            // writeApi.WritePointAsync(point);
            
            var step1 = await Step.Run("login", context, async () =>
            {
                await Task.Delay(500);
                return Response.Ok(sizeBytes: 10, statusCode: "200");
            });

            var step2 = await Step.Run("get_product", context, async () =>
            {
                await Task.Delay(1000);
                return Response.Ok(sizeBytes: 20, statusCode: "200");
            });

            var step3 = await Step.Run("buy_product", context, async () =>
            {
                await Task.Delay(2000);
                return Response.Ok(sizeBytes: 30, statusCode: "200");
            });

            return Response.Ok(statusCode: "201");
        })
        .WithWarmUpDuration(TimeSpan.FromSeconds(10))
        .WithLoadSimulations(
            Simulation.RampingInject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)), // rump-up to rate 200
            Simulation.Inject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)),       // keep injecting with rate 200
            Simulation.RampingInject(rate: 0, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1))    // rump-down to rate 0
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .LoadInfraConfig("./Features/RealtimeReporting/InfluxDB/infra-config.json")
            .WithReportingInterval(TimeSpan.FromSeconds(5))
            .WithReportingSinks(_influxDbSink)
            .WithTestSuite("reporting")
            .WithTestName("influx_db_demo")
            .Run();
    }
}