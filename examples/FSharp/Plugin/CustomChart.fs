module FSharp.Plugin.CustomChartExample

open System.Data
open System.Threading.Tasks

open FSharp.Control.Tasks.V2

open NBomber.Contracts
open NBomber.FSharp

type CustomChartPlugin () =

    let header = "<style>.my-custom-chart.chart { height: 500px; }</style>"
    let viewModel = "{
            settings: {
                credits: { enabled: false },
                title: { text: 'custom chart' },
                yAxis: [ { title: { text: 'value' } } ],
                xAxis: {
                    title: { text: 'time' },
                    categories: [\"00:00:05\", \"00:00:10\", \"00:00:15\", \"00:00:20\", \"00:00:25\", \"00:00:30\"]
                },
                series: [ { name: 'data', type: 'area', data: [100, 150, 120, 120, 150, 100] } ]
            }
        }"

    let htmlTemplate = "<chart-custom class=\"my-custom-chart\" :settings=\"viewModel.settings\"></chart-custom>"

    let createPluginStats (currentOperation) =
        let pluginStats = new DataSet()

        if currentOperation = NodeOperationType.Complete then
            let table =
                "Custom chart"
                |> CustomPluginDataBuilder.create
                |> CustomPluginDataBuilder.withHeader(header)
                |> CustomPluginDataBuilder.withViewModel(viewModel)
                |> CustomPluginDataBuilder.withHtmlTemplate(htmlTemplate)
                |> CustomPluginDataBuilder.build

            pluginStats.Tables.Add(table)

        pluginStats

    interface IWorkerPlugin with
        member _.PluginName = "CustomChart"
        member _.Init(_, _) = ()
        member _.Start(_) = Task.CompletedTask
        member _.GetStats(currentOperation) = createPluginStats(currentOperation)
        member _.GetHints() = Array.empty
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()

let run () =

    let plugin = new CustomChartPlugin()

    let step = Step.create("step_1", fun context -> task {
        do! Task.Delay(seconds 1)
        return Response.Ok()
    })

    Scenario.create "scenario_1" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 30)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.withTestSuite "custom_chart"
    |> NBomberRunner.withTestName "simple_test"
    |> NBomberRunner.run
    |> ignore
