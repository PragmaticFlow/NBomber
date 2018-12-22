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

            NBomberRunner.RegisterScenarios(scenario)
                         //.LoadConfig("config.json")
                         //.WithReportFilename("custom_report_name")
                         //.WithReportFormats(FileType.Txt, FileType.Html)
                         .RunInConsole();
        }
    }
}
