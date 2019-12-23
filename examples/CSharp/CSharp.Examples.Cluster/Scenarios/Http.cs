using System;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using NBomber.Sinks.InfluxDB;

namespace CSharp.Examples.Cluster.Scenarios
{
    public class HttpScenario
    {
        public static void Run(string configPath)
        {
            var influxDb = new InfluxDBSink(url: "http://localhost:8086", dbName: "default");
            
            var step = HttpStep.Create("cluster simple step", (context) =>
                Http.CreateRequest("GET", "https://gitter.im")
                    .WithHeader("Accept", "text/html")
                    //.WithHeader("Cookie", "cookie1=value1; cookie2=value2")
                    //.WithBody(new StringContent("{ some JSON }", Encoding.UTF8, "application/json"))
                    //.WithCheck(response => Task.FromResult(response.IsSuccessStatusCode))
            );

            var scenario = ScenarioBuilder.CreateScenario("cluster_test_gitter", new[] { step });
            
            NBomberRunner.RegisterScenarios(scenario)
                         .LoadConfig(configPath)
                         .WithReportingSinks(new[] {influxDb }, sendStatsInterval: TimeSpan.FromSeconds(20))
                         //.LoadConfig("agent_config.json")
                         //.LoadConfig("coordinator_config.json")
                         .RunInConsole();
        }
    }
}