using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.FSharp.Core;
using NBomber.Contracts;
using NBomber.CSharp;
using Xunit;

namespace CSharpDev.XUnit
{
     public class Plugin1 : IWorkerPlugin
     {
         public string PluginName => "Plugin1";

         public Task Init(IBaseContext context, FSharpOption<IConfiguration> infraConfig) => Task.CompletedTask;

         public Task Start() => Task.CompletedTask;

         public DataSet GetStats(NodeOperationType currentOperation)
         {
             var pluginStats = new DataSet();
             pluginStats.Tables.Add(CreateTable("PluginStats", 5));
             return pluginStats;
         }

         public string[] GetHints() => Array.Empty<string>();

         public Task Stop() => Task.CompletedTask;

         public void Dispose()
         {
         }

         public static string TryGetValueForKey(string key, DataSet pluginStats)
         {
             var table = pluginStats?.Tables["PluginStats"];
             var row = table?.Rows.Cast<DataRow>().FirstOrDefault(r => r["Key"].ToString() == key);
             return row?["Value"].ToString();
         }

         private IEnumerable<DataColumn> CreateTableCols()
         {
             yield return new DataColumn("Key", Type.GetType("System.String"))
             {
                 Caption = "Key"
             };

             yield return new DataColumn("Value", Type.GetType("System.String"))
             {
                 Caption = "Value"
             };
         }

         private IEnumerable<DataRow> CreateTableRows(DataTable table, int count)
         {
             for (var i = 1; i <= count; i++)
             {
                 var row = table.NewRow();
                 row["Key"] = $"Key{i}";
                 row["Value"] = $"Value{i}";
                 yield return row;
             }
         }

         private DataTable CreateTable(string tableName, int rowsCount)
         {
             var table = new DataTable(tableName);
             table.Columns.AddRange(CreateTableCols().ToArray());
             var rows = CreateTableRows(table, rowsCount).ToArray();

             foreach (var row in rows)
             {
                 table.Rows.Add(row);
             }

             return table;
         }
     }

     public class PluginStatsTest
     {
         // in this example we use:
         // - XUnit (https://xunit.net/)
         // - Fluent Assertions (https://fluentassertions.com/)
         // to get more info about test automation, please visit: (https://nbomber.com/docs/test-automation)

         Scenario BuildScenario()
         {
             var step = Step.Create("simple step", async context =>
             {
                 await Task.Delay(TimeSpan.FromSeconds(0.1));
                 return Response.Ok(sizeBytes: 1024);
             });

             return ScenarioBuilder.CreateScenario("xunit_plugin_stats", step);
         }

         [Fact]
         public void Test()
         {
             var scenario = BuildScenario()
                 .WithoutWarmUp()
                 .WithLoadSimulations(new[]
                 {
                      Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(2))
                 });

             var nodeStats = NBomberRunner.RegisterScenarios(scenario).WithWorkerPlugins(new Plugin1()).Run();
             var (success, pluginStats) = PluginStats.TryFind("Plugin1", nodeStats);
             var pluginStatsValue = Plugin1.TryGetValueForKey("Key1", pluginStats);

             success.Should().BeTrue();
             pluginStatsValue.Should().Be("Value1");
         }
    }
}
