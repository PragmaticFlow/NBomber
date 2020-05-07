module CustomPluginScenario

open System
open System.Threading.Tasks
open System.Net.Http
open System.Data

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

// it's a very basic CustomPlugin example to give you a playground for writing your own plugins

type CustomPlugin () =

    let mutable _logger = Unchecked.defaultof<ILogger>

    interface IPlugin with
        member x.PluginName = "CustomPlugin"

        member x.Init(logger, infraConfig) =
            _logger <- _logger

        member x.StartTest(testInfo: TestInfo) = Task.CompletedTask

        member x.GetStats() =
            let table = new DataTable("Custom Statistics")

            ["Key", "Property", "System.String"
             "Value", "Value", "System.String"]
            |> Seq.map(fun (name, caption, typeName) ->
                  let column = new DataColumn(name, Type.GetType(typeName))
                  column.Caption <- caption
                  column
            )
            |> Seq.toArray
            |> table.Columns.AddRange

            table.Rows.Add("Test property", "Test value") |> ignore

            let stats = new DataSet()
            stats.Tables.Add(table)
            stats

        member x.StopTest() = Task.CompletedTask
        member x.Dispose() = ()

let run (args) =

    let httpClient = new HttpClient()
    use customPlugin = new CustomPlugin()

    let step = Step.create("GET html", fun context -> task {
        use! response = httpClient.GetAsync("https://nbomber.com",
                                            context.CancellationToken)

        match response.IsSuccessStatusCode with
        | true  -> let size = int response.Content.Headers.ContentLength.Value
                   return Response.Ok(sizeBytes = size)
        | false -> return Response.Fail()
    })

    let scenario = Scenario.create "test_nbomber" [step]
                   |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 30.0)
                   |> Scenario.withLoadSimulations [
                       KeepConcurrentScenarios(200, TimeSpan.FromSeconds 60.0)
                   ]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withPlugins [customPlugin]
    |> NBomberRunner.runInConsole args
