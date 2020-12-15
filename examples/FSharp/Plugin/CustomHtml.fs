module Plugin.CustomHtmlExample
//
//open System.Data
//open System.Threading.Tasks
//
//open FSharp.Control.Tasks.V2
//
//open NBomber.FSharp
//open NBomber.Contracts
//
//type CustomHtmlPlugin () =
//    let header = "<script>console.log('custom header');</script>"
//    let js = "console.log('custom js');"
//    let viewModel = "{ message: 'Hello from custom html' }"
//    let htmlTemplate = "<h3>Message: {{viewModel.message}}</h3>"
//
//    let createPluginStats (currentOperation) =
//        let pluginStats = new DataSet()
//        let table =
//            "Custom html"
//            |> CustomPluginDataBuilder.create
//            |> CustomPluginDataBuilder.withHeader(header)
//            |> CustomPluginDataBuilder.withJs(js)
//            |> CustomPluginDataBuilder.withViewModel(viewModel)
//            |> CustomPluginDataBuilder.withHtmlTemplate(htmlTemplate)
//            |> CustomPluginDataBuilder.build
//
//        pluginStats.Tables.Add(table)
//        pluginStats
//
//    interface IWorkerPlugin with
//        member _.PluginName = "Plugin1"
//        member _.Init(_, _) = ()
//        member _.Start(_) = Task.CompletedTask
//        member _.GetStats(currentOperation) = createPluginStats(currentOperation)
//        member _.GetHints() = Array.empty
//        member _.Stop() = Task.CompletedTask
//        member _.Dispose() = ()
//
//let run () =
//
//    let plugin = new CustomHtmlPlugin()
//
//    let step = Step.create("step_1", fun context -> task {
//        return Response.Ok()
//    })
//
//    Scenario.create "scenario_1" [step]
//    |> Scenario.withWarmUpDuration(seconds 5)
//    |> Scenario.withLoadSimulations [InjectPerSec(rate = 100, during = seconds 30)]
//    |> NBomberRunner.registerScenario
//    |> NBomberRunner.withWorkerPlugins [plugin]
//    |> NBomberRunner.withTestSuite "assert_plugin_stats"
//    |> NBomberRunner.withTestName "simple_test"
//    |> NBomberRunner.run
//    |> ignore
