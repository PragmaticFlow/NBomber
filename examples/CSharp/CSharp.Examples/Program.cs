using System;
using NBomber.Contracts;
using NBomber.CSharp;
using CSharp.Examples.Scenarios;

namespace CSharp.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var scenario = HelloWorldScenario
            //var scenario = HttpScenario
            //var scenario = MongoDbScenario
            //var scenario = WebSocketsScenario
                .BuildScenario()
                .WithConcurrentCopies(10)
                .WithDuration(TimeSpan.FromSeconds(5));

            var scenario2 = HelloWorldScenario
            //var scenario = HttpScenario
            //var scenario = MongoDbScenario
            //var scenario = WebSocketsScenario
                .BuildScenario()
                .WithConcurrentCopies(10)
                .WithDuration(TimeSpan.FromSeconds(5));

            NBomberRunner.RegisterScenarios(scenario, scenario2)
                         //.LoadConfig("config.json")
                         //.WithReportFileName("custom_report_name")
                         .WithReportFormats(ReportFormat.Html)
                         .RunInConsole();
        }
    }
}
