module FSharpDev.Plugin.PluginHtmlReportExample


open System.Data
open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine

open NBomber.Contracts
open NBomber.FSharp

type CustomHtmlPlugin () =

    let style =
        "<style>" +
        "   #custom-html { color: red; }" +
        "</style>"
    let componentTemplate =
        "<script type=\"text/x-template\" id=\"custom-message-template\">" +
        "   <div>" +
        "       <h3>Message: {{message}}</h3>" +
        "       <slot></slot>" +
        "   <div>" +
        "</script>"

    let componentJs =
       "<script>" +
       "    Vue.component('custom-message', {" +
       "        props: ['message']," +
       "        template: '#custom-message-template'" +
       "    });" +
       "</script>"

    let html =
        "<div id=\"custom-html\">" +
        "   <custom-message :message=\"message\">some html goes here</custom-message>" +
        "</div>"
    let js =
        "<script>" +
        "   new Vue({" +
        "       el: '#custom-html'," +
        "       data: {message: 'hello from plugin'}" +
        "   });" +
        "</script>"

    let createPluginStats (currentOperation) =
        let pluginStats = new DataSet()

        if currentOperation = NodeOperationType.Complete then
            NBomber.PluginReport.create()
            |> NBomber.PluginReport.addToHtmlReportHead style
            |> NBomber.PluginReport.addToHtmlReportHead componentTemplate
            |> NBomber.PluginReport.addToHtmlReportBody html
            |> NBomber.PluginReport.addToHtmlReportBody componentJs
            |> NBomber.PluginReport.addToHtmlReportBody js
            |> pluginStats.Tables.Add

        pluginStats

    interface IWorkerPlugin with
        member _.PluginName = "Plugin1"
        member _.Init(_, _) = Task.CompletedTask
        member _.Start() = Task.CompletedTask
        member _.GetStats(currentOperation) = createPluginStats(currentOperation)
        member _.GetHints() = Array.empty
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()

let run () =

    let plugin = new CustomHtmlPlugin()

    let step = Step.create("step_1", fun context -> task {
        do! Task.Delay(seconds 1)
        return Response.Ok()
    })

    Scenario.create "scenario_1" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 30)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.withTestSuite "custom_html"
    |> NBomberRunner.withTestName "simple_test"
    |> NBomberRunner.run
    |> ignore
