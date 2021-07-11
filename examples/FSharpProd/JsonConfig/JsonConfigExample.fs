module FSharpProd.JsonConfig.JsonConfigExample

open System
open System.Data
open System.Net.Http
open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine
open Microsoft.Extensions.Configuration
open Newtonsoft.Json

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Network.Ping
open NBomber.Sinks.InfluxDB

// in this example we use JSON configuration files:
// - 'config.json' to configure load test settings
// - 'infra-config.json' to configure infrastructure settings (logging, worker plugins, reposting sinks)
// - 'user-feed.json' to configure data source to feed from

module Option =

    let ofRecord (value: 'T) =
        let boxed = box(value)
        if isNull boxed then None
        else Some value

[<CLIMutable>]
type User = {
    Id: int
    Name: string
}

[<CLIMutable>]
type UserResponse = {
    Id: string
    Name: string
    Email: string
    Phone: string
}

[<CLIMutable>]
type PostResponse = {
    Id: string
    UserId: string
    Title: string
    Body: string
}

[<CLIMutable>]
type CustomScenarioSettings = {
    BaseUri: string
}

[<CLIMutable>]
type CustomPluginSettings = {
    Message: string
}

[<CLIMutable>]
type CustomReportingSinkSettings = {
    Message: string
}

type CustomPlugin(customPluginSettings: CustomPluginSettings) =
    let mutable _pluginStats = new DataSet()

    interface IWorkerPlugin with
        member _.PluginName = nameof(CustomPlugin)

        member _.Init(context, infraConfig) =
            let logger = context.Logger.ForContext<CustomPluginSettings>()

            let settings =
                infraConfig.GetSection(nameof(CustomPlugin)).Get<CustomPluginSettings>()
                |> Option.ofRecord
                |> Option.defaultValue customPluginSettings

            logger.Information($"{nameof(CustomPlugin)} settings: {settings}")

            Task.CompletedTask

        member _.Start() = Task.CompletedTask
        member _.GetStats(currentOperation) = Task.FromResult(_pluginStats)
        member _.GetHints() = Array.empty
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()

type CustomReportingSink(customReportingSinkSettings: CustomReportingSinkSettings) =
    interface IReportingSink with
        member _.SinkName = nameof(CustomReportingSink)
        member _.Init(context, infraConfig) =
            let logger = context.Logger.ForContext<CustomReportingSinkSettings>()

            let settings =
                infraConfig.GetSection(nameof(CustomReportingSink)).Get<CustomReportingSinkSettings>()
                |> Option.ofRecord
                |> Option.defaultValue customReportingSinkSettings

            logger.Information($"{nameof(CustomReportingSink)} settings: {settings}")

            Task.CompletedTask
        member _.Start() = Task.CompletedTask
        member _.SaveRealtimeStats(stats) = Task.CompletedTask
        member _.SaveFinalStats(stats) = Task.CompletedTask
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()

let run () =

    let mutable _customSettings = Unchecked.defaultof<CustomScenarioSettings>

    let scenarioInit (context: IScenarioContext) = task {
        _customSettings <- context.CustomSettings.Get<CustomScenarioSettings>()

        context.Logger.Information($"test init received CustomSettings: {_customSettings}")
    }

    let userFeed = FeedData.fromJson<User> "./JsonConfig/Configs/user-feed.json"
                   |> Feed.createRandom "userFeed"

    let httpFactory =
        ClientFactory.create(name = "http_factory",
                             initClient = fun (number,context) -> task {
                                 return new HttpClient(BaseAddress = Uri(_customSettings.BaseUri))
                             })

    let getUser = Step.create("get_user",
                              clientFactory = httpFactory,
                              feed = userFeed,
                              execute = fun context -> task {

        let userId = context.FeedItem
        let url = $"users?id={userId.Id}"

        let! response = context.Client.GetAsync(url, context.CancellationToken)
        let! json = response.Content.ReadAsStringAsync()

        // parse JSON
        let users = json
                    |> JsonConvert.DeserializeObject<UserResponse[]>
                    |> ValueOption.ofObj

        match users with
        | ValueSome usr when usr.Length = 1 ->
            return Response.ok(usr.[0]) // we pass user object response to the next step

        | _ -> return Response.fail($"not found user: {userId.Id}")
    })

    // this 'getPosts' will be executed only if 'getUser' finished OK.
    let getPosts = Step.create("get_posts",
                               clientFactory = httpFactory,
                               execute = fun context -> task {

        let user = context.GetPreviousStepResponse<UserResponse>()
        let url = $"posts?userId={user.Id}"

        let! response = context.Client.GetAsync(url, context.CancellationToken)
        let! json = response.Content.ReadAsStringAsync()

        // parse JSON
        let posts = json
                    |> JsonConvert.DeserializeObject<PostResponse[]>
                    |> ValueOption.ofObj

        match posts with
        | ValueSome ps when ps.Length > 0 ->
            return Response.ok()

        | _ -> return Response.fail()
    })

    let scenario1 =
        Scenario.create "rest_api" [getUser; getPosts]
        |> Scenario.withInit scenarioInit

    // the following scenario will be ignored since it is not included in target scenarios list of config.json
    let scenario2 = Scenario.create "ignored" [getUser]

    // settings for plugins and reporting sinks are overriden in infra-config.json
    let pingPlugin = new PingPlugin()
    let customPlugin = new CustomPlugin({ Message = "Plugin is configured via constructor" })
    let influxDbReportingSink = new InfluxDBSink();
    let customReportingSink = new CustomReportingSink({ Message = "Reporting sink is configured via constructor" })

    NBomberRunner.registerScenarios [scenario1; scenario2]
    |> NBomberRunner.withWorkerPlugins [pingPlugin; customPlugin]
    |> NBomberRunner.withReportingSinks [influxDbReportingSink; customReportingSink]
    |> NBomberRunner.loadConfig "./JsonConfig/Configs/config.json"
    |> NBomberRunner.loadInfraConfig "./JsonConfig/Configs/infra-config.json"
    |> NBomberRunner.run
    |> ignore
