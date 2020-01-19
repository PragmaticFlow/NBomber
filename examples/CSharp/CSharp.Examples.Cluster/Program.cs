using System;
using CSharp.Examples.Cluster.Tests.MqttReqResponse;
using CSharp.Examples.Cluster.Tests.SimpleHttp;
using NBomber.CSharp;
using NBomber.Sinks.InfluxDB;

namespace CSharp.Examples.Cluster
{
    class Program
    {
        static void Main(string[] args)
        {
            var configPath = args[0]; // agent_config.json or coordinator_config.json

            var influxDb = new InfluxDBSink(url: "http://localhost:8086", dbName: "default");

            NBomberRunner.RegisterScenarios(
                    SimpleHttpScenario.Create(),
                    MqttReqResponseScenario.Create()
                )
                .WithReportingSinks(new[] { influxDb }, sendStatsInterval: TimeSpan.FromSeconds(20))
                .LoadInfraConfig("infra_config.json")
                .LoadTestConfig(configPath) // agent_config.json or coordinator_config.json
                .RunInConsole();
        }
    }
}
