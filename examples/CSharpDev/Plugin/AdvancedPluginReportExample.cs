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
    /// Vue.js is already added in html report.
    /// You have to create a new instance of Vue application to use its functionality.
    /// Existing Vue.js components:
    /// - plugin-card
    /// - plugin-chart
    /// Also new Vue.js components can be created.
    public class AdvancedReportPlugin : IWorkerPlugin
    {
        private const string Text = "hello from plugin";

        private const string Md = "hello from `plugin`";

        private const string Style =
            "<style>" +
            "   #plugin-app .plugin-message { color: red; }" +
            "   #plugin-app .chart-plugin { height: 500px; }" +
            "</style>";

        private const string Html =
            "<div id=\"plugin-app\">" +
            "   <plugin-card>" +
            "       <div slot=\"header\">" +
            "           <h6>Custom</h6>" +
            "       </div>" +
            "       <div>" +
            "           <h3 class=\"plugin-message\">{{message}}</h3>" +
            "           <plugin-chart :settings=\"chartSettings\"></plugin-chart>" +
            "       </div>" +
            "   </plugin-card>" +
            "</div>";

        private const string Js =
            "<script>" +
            "   new Vue({" +
            "       el: '#plugin-app'," +
            "       data: {" +
            "           message: 'hello from plugin'," +
            "           chartSettings: {" +
            "               credits: { enabled: false }," +
            "               title: { text: 'plugin chart' }," +
            "               yAxis: [ { title: { text: 'value' } } ]," +
            "               xAxis: {" +
            "                   title: { text: 'time' }," +
            "                   categories: [\"00:00:05\", \"00:00:10\", \"00:00:15\", \"00:00:20\", \"00:00:25\", \"00:00:30\"]" +
            "               }," +
            "               series: [ { name: 'data', type: 'area', data: [100, 150, 120, 120, 150, 100] } ]" +
            "           }" +
            "       }" +
            "   });" +
            "</script>";

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
                    .AddToHtmlReportBody(Html)
                    .AddToHtmlReportBody(Js);

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

    public class AdvancedPluginReportExample
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
                 .WithWorkerPlugins(new AdvancedReportPlugin())
                 .WithTestSuite("plugin_report")
                 .WithTestName("simple_test")
                 .Run();
         }
     }
}
