using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.FSharp.Core;
using NBomber.Contracts;
using NBomber.CSharp;
using Serilog;

namespace CSharpDev.Plugin
{
     public class CustomHtmlPlugin : IWorkerPlugin
     {
         private const string Style =
             "<style>" +
             "   #custom-html { color: red; }" +
             "</style>";

         private const string ComponentTemplate =
             "<script type=\"text/x-template\" id=\"custom-message-template\">" +
             "   <div>" +
             "       <h3>Message: {{message}}</h3>" +
             "       <slot></slot>" +
             "   <div>" +
             "</script>";

         private const string ComponentJs =
             "<script>" +
             "    Vue.component('custom-message', {" +
             "        props: ['message']," +
             "        template: '#custom-message-template'" +
             "    });" +
             "</script>";

         private const string Html =
             "<div id=\"custom-html\">" +
             "   <custom-message :message=\"message\">some html goes here</custom-message>" +
             "</div>";

         private const string Js =
             "<script>" +
             "   new Vue({" +
             "       el: '#custom-html'," +
             "       data: {message: 'hello from plugin'}" +
             "   });" +
             "</script>";

         public string PluginName => "CustomHtml";

         public Task Init(IBaseContext context, FSharpOption<IConfiguration> infraConfig) => Task.CompletedTask;

         public Task Start() => Task.CompletedTask;

         public DataSet GetStats(NodeOperationType currentOperation)
         {
             var pluginStats = new DataSet();

             if (currentOperation == NodeOperationType.Complete)
             {
                 var table = NBomber.PluginReport.Create();
                 NBomber.PluginReport.AddToHtmlReportHead(Style, table);
                 NBomber.PluginReport.AddToHtmlReportHead(ComponentTemplate, table);
                 NBomber.PluginReport.AddToHtmlReportBody(Html, table);
                 NBomber.PluginReport.AddToHtmlReportBody(ComponentJs, table);
                 NBomber.PluginReport.AddToHtmlReportBody(Js, table);
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

     public class PluginHtmlReportExample
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
                 .WithWorkerPlugins(new CustomHtmlPlugin())
                 .WithTestSuite("custom_html")
                 .WithTestName("simple_test")
                 .Run();
         }
     }
}
