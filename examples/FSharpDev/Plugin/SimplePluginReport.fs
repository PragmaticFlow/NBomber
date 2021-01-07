module FSharpDev.Plugin.SimplePluginReportExample


open System.Data
open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine

open NBomber.Contracts
open NBomber.FSharp

/// This plugin injects data in txt, md, html reports.
type ReportPlugin () =

    let text = "hello from plugin"
    let md = "hello from `plugin`"
    let style = "<style>.plugin-report { color: red; }</style>"
    let html = "<div class=\"plugin-report\">hello from plugin</div>"

    let createPluginStats (currentOperation) =
        let pluginStats = new DataSet()

        if currentOperation = NodeOperationType.Complete then
            PluginReport.create()
            |> PluginReport.addToTxtReport text
            |> PluginReport.addToMdReport md
            |> PluginReport.addToHtmlReportHead style
            |> PluginReport.addToHtmlReportBody html
            |> pluginStats.Tables.Add

        pluginStats

    interface IWorkerPlugin with
        member _.PluginName = "PluginReport"
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
