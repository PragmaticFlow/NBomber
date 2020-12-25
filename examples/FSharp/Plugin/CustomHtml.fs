module FSharp.Plugin.CustomHtmlExample

open System.Data
open System.Threading.Tasks

open FSharp.Control.Tasks.V2

open NBomber.Contracts
open NBomber.FSharp

type CustomHtmlPlugin () =
    let header = "<script>console.log('custom header');</script>"
    let js = "console.log('custom js');"
    let viewModel = "{ message: 'Hello from custom html' }"
    let htmlTemplate = "<h3>Message: {{viewModel.message}}</h3>"

    let createPluginStats (currentOperation) =
        let pluginStats = new DataSet()

        if currentOperation = NodeOperationType.Complete then
            let table =
                "Custom html"
                |> CustomPluginDataBuilder.create
                |> CustomPluginDataBuilder.withHeader(header)
                |> CustomPluginDataBuilder.withJs(js)
                |> CustomPluginDataBuilder.withViewModel(viewModel)
                |> CustomPluginDataBuilder.withHtmlTemplate(htmlTemplate)
                |> CustomPluginDataBuilder.build

            pluginStats.Tables.Add(table)

        pluginStats

    interface IWorkerPlugin with
        member _.PluginName = "Plugin1"
        member _.Init(_, _) = ()
        member _.Start(_) = Task.CompletedTask
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
