module FSharpDev.Plugin.AdvancedPluginReportExample


open System.Data
open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine

open NBomber.Contracts
open NBomber.FSharp

/// This plugin injects data in txt, md, html reports.
/// Vue.js is already added in html report.
/// You have to create a new instance of Vue application to use its functionality.
/// Existing Vue.js components:
/// - plugin-card
/// - plugin-chart
/// Also new Vue.js components can be created.
type ReportPlugin () =

    let text = "hello from plugin"
    let md = "hello from `plugin`"

    let style =
        "<style>" +
        "   #plugin-app .plugin-message { color: red; }" +
        "   #plugin-app .chart-plugin { height: 500px; }" +
        "</style>"

    let html =
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
        "</div>"

    let js =
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
        "</script>"

    let createPluginStats (currentOperation) =
        let pluginStats = new DataSet()

        if currentOperation = NodeOperationType.Complete then
            PluginReport.create()
            |> PluginReport.addToTxtReport text
            |> PluginReport.addToMdReport md
            |> PluginReport.addToHtmlReportHead style
            |> PluginReport.addToHtmlReportBody html
            |> PluginReport.addToHtmlReportBody js
            |> pluginStats.Tables.Add

        pluginStats

    interface IWorkerPlugin with
        member _.PluginName = "ReportPlugin"
        member _.Init(_, _) = Task.CompletedTask
        member _.Start() = Task.CompletedTask
        member _.GetStats(currentOperation) = createPluginStats(currentOperation)
        member _.GetHints() = Array.empty
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()

let run () =

    let reportPlugin = new ReportPlugin()

    let step = Step.create("step_1", fun context -> task {
        do! Task.Delay(seconds 1)
        return Response.Ok()
    })

    Scenario.create "scenario_1" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 30)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withWorkerPlugins [reportPlugin]
    |> NBomberRunner.withTestSuite "plugin_report"
    |> NBomberRunner.withTestName "simple_test"
    |> NBomberRunner.run
    |> ignore
