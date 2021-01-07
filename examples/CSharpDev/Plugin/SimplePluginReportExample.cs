using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.FSharp.Core;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharpDev.Plugin
{
    /// This plugin injects data in txt, md, html reports.
    public class SimpleReportPlugin : IWorkerPlugin
    {
        private const string Text = "hello from plugin";

        private const string Md = "hello from `plugin`";

        private const string Style = "<style>.plugin-report { color: red; }</style>";

        private const string Html = "<div class=\"plugin-report\">hello from plugin</div>";

        public string PluginName => "ReportPlugin";

        public Task Init(IBaseContext context, FSharpOption<IConfiguration> infraConfig) => Task.CompletedTask;

        public Task Start() => Task.CompletedTask;

        public DataSet GetStats(NodeOperationType currentOperation)
        {
            var pluginStats = new DataSet();

            if (currentOperation == NodeOperationType.Complete)
            {
                var table = PluginReport.Create()
                    .AddToTxtReport(Text)
                    .AddToMdReport(Md)
                    .AddToHtmlReportHead(Style)
                    .AddToHtmlReportBody(Html);

                pluginStats.Tables.Add(table);
            }

            return pluginStats;
        }

        public string[] GetHints() => Array.Empty<string>();

        public Task Stop() => Task.CompletedTask;

        public void Dispose()
        {
        }
    }

    public class SimplePluginReportExample
     {
         public static void Run()
         {
             var step1 = Step.Create("step_1", async context =>
             {
                 await Task.Delay(TimeSpan.FromSeconds(0.1));
                 return Response.Ok();
             });

             var scenario = ScenarioBuilder
                 .CreateScenario("scenario_1", step1)
                 .WithoutWarmUp()
                 .WithLoadSimulations(
                     Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
                 );

             NBomberRunner
                 .RegisterScenarios(scenario)
                 .WithWorkerPlugins(new SimpleReportPlugin())
                 .WithTestSuite("plugin_report")
                 .WithTestName("simple_test")
                 .Run();
         }
     }
}
