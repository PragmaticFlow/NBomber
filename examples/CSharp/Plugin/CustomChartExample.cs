// using System;
// using System.Data;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Configuration;
// using Microsoft.FSharp.Core;
// using NBomber.Contracts;
// using NBomber.CSharp;
// using Serilog;
//
// namespace CSharp.Plugin
// {
//     public class CustomChartPlugin: IWorkerPlugin
//     {
//         private const string Header = "<style>.my-custom-chart.chart { height: 500px; }</style>";
//         private const string ViewModel = @"{
//             settings: {
//                 credits: { enabled: false },
//                 title: { text: 'custom chart' },
//                 yAxis: [ { title: { text: 'value' } } ],
//                 xAxis: {
//                     title: { text: 'time' },
//                     categories: [""00:00:05"", ""00:00:10"", ""00:00:15"", ""00:00:20"", ""00:00:25"", ""00:00:30""]
//                 },
//                 series: [ { name: 'data', type: 'area', data: [100, 150, 120, 120, 150, 100] } ]
//             }
//         }";
//         private const string HtmlTemplate = "<chart-custom class=\"my-custom-chart\" :settings=\"viewModel.settings\"></chart-custom>";
//
//         public string PluginName => "CustomChart";
//
//         public void Init(ILogger logger, FSharpOption<IConfiguration> infraConfig)
//         {
//         }
//
//         public Task Start(TestInfo testInfo) => Task.CompletedTask;
//
//         public DataSet GetStats(NodeOperationType currentOperation)
//         {
//             var pluginStats = new DataSet();
//
//             if (currentOperation == NodeOperationType.Complete)
//             {
//                 var table = CustomPluginDataBuilder
//                     .Create("Custom html")
//                     .WithHeader(Header)
//                     .WithViewModel(ViewModel)
//                     .WithHtmlTemplate(HtmlTemplate)
//                     .Build();
//
//                 pluginStats.Tables.Add(table);
//             }
//
//             return pluginStats;
//         }
//
//         public string[] GetHints() => Array.Empty<string>();
//
//         public Task Stop() => Task.CompletedTask;
//
//         public void Dispose()
//         {
//         }
//     }
//
//     public class CustomChartExample
//     {
//         public static void Run()
//         {
//             var step1 = Step.Create("step_1", asyanc context =>
//             {
//                 await Task.Delay(TimeSpan.FromSeconds(0.1));
//                 return Response.Ok();
//             });
//
//             var scenario = ScenarioBuilder
//                 .CreateScenario("scenario_1", step1)
//                 .WithoutWarmUp()
//                 .WithLoadSimulations(
//                     Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
//                 );
//
//             NBomberRunner
//                 .RegisterScenarios(scenario)
//                 .WithWorkerPlugins(new CustomChartPlugin())
//                 .WithTestSuite("custom_chart")
//                 .WithTestName("simple_test")
//                 .Run();
//         }
//     }
// }
