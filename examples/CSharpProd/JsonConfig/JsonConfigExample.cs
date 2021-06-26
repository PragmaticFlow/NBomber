using System;
using System.Data;
using System.Linq;
using System.Net.Http;

using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

using NBomber;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Plugins.Network.Ping;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CSharpProd.JsonConfig
{
    // in this example we use JSON configuration files:
    // - 'config.json' to configure load test settings
    // - 'infra-config.json' to configure infrastructure settings (logging, worker plugins, reposting sinks)
    // - 'user-feed.json' to configure data source to feed from

    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class UserResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class PostResponse
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class CustomScenarioSettings
    {
        public string BaseUri { get; set; }
    }

    public class CustomPluginSettings
    {
        public string Message { get; set; }
    }

    public class CustomReportingSinkSettings
    {
        public string Message { get; set; }
    }

    public class CustomPlugin : IWorkerPlugin
    {
        private readonly DataSet _pluginStats = new DataSet();
        private CustomPluginSettings _customPluginSettings;

        public CustomPlugin(CustomPluginSettings customPluginSettings) =>
            _customPluginSettings = customPluginSettings;

        public string PluginName => nameof(CustomPlugin);

        public Task Init(IBaseContext context, IConfiguration infraConfig)
        {
            var logger = context.Logger.ForContext<CustomPlugin>();

            _customPluginSettings =
                infraConfig.GetSection(nameof(CustomReportingSink)).Get<CustomPluginSettings>()
                ?? _customPluginSettings;

            var settingsJson = JsonSerializer.Serialize(_customPluginSettings);

            logger.Information($"{nameof(CustomPlugin)} settings: {settingsJson}");

            return Task.CompletedTask;
        }

        public Task Start() => Task.CompletedTask;

        public Task<DataSet> GetStats(OperationType currentOperation) => Task.FromResult(_pluginStats);

        public string[] GetHints() => Array.Empty<string>();

        public Task Stop() => Task.CompletedTask;

        public void Dispose()
        {
        }
    }

    public class CustomReportingSink : IReportingSink
    {
        private CustomReportingSinkSettings _customReportingSinkSettings;

        public CustomReportingSink(CustomReportingSinkSettings customReportingSinkSettings) =>
        _customReportingSinkSettings = customReportingSinkSettings;

        public string SinkName => nameof(CustomReportingSink);

        public Task Init(IBaseContext context, IConfiguration infraConfig)
        {
            var logger = context.Logger.ForContext<CustomReportingSink>();

            _customReportingSinkSettings =
                infraConfig.GetSection(nameof(CustomReportingSink)).Get<CustomReportingSinkSettings>()
                ?? _customReportingSinkSettings;

            var settingsJson = JsonSerializer.Serialize(_customReportingSinkSettings);

            logger.Information($"{nameof(CustomReportingSink)} settings: {settingsJson}");

            return Task.CompletedTask;
        }

        public Task Start() => Task.CompletedTask;
        public Task SaveRealtimeStats(ScenarioStats[] stats) => Task.CompletedTask;
        public Task SaveFinalStats(NodeStats[] stats) => Task.CompletedTask;
        public Task Stop() => Task.CompletedTask;

        public void Dispose()
        {
        }
    }

    public static class JsonConfigExample
    {
        private static CustomScenarioSettings _customSettings = new CustomScenarioSettings();

        private static Task ScenarioInit(IScenarioContext context)
        {
            _customSettings = context.CustomSettings.Get<CustomScenarioSettings>();

            context.Logger.Information($"test init received CustomSettings: {_customSettings}");

            return Task.CompletedTask;
        }

        public static void Run()
        {
            var userFeed = Feed.CreateRandom(
                "userFeed",
                FeedData.FromJson<User>("./JsonConfig/Configs/user-feed.json")
            );

            var httpFactory = ClientFactory.Create("http_factory",
                initClient: (number,context) => Task.FromResult(new HttpClient { BaseAddress = new Uri(_customSettings.BaseUri)}));

            var getUser = Step.Create("get_user", httpFactory, userFeed, async context =>
            {
                var userId = context.FeedItem;
                var url = $"users?id={userId}";

                var response = await context.Client.GetAsync(url, context.CancellationToken);
                var json = await response.Content.ReadAsStringAsync();

                // parse JSON
                var users = JsonConvert.DeserializeObject<UserResponse[]>(json);

                return users?.Length == 1
                    ? Response.Ok(users.First()) // we pass user object response to the next step
                    : Response.Fail("not found user");
            });

            // this 'getPosts' will be executed only if 'getUser' finished OK.
            var getPosts = Step.Create("get_posts", httpFactory, async context =>
            {
                var user = context.GetPreviousStepResponse<UserResponse>();
                var url = $"posts?userId={user.Id}";

                var response = await context.Client.GetAsync(url, context.CancellationToken);
                var json = await response.Content.ReadAsStringAsync();

                // parse JSON
                var posts = JsonConvert.DeserializeObject<PostResponse[]>(json);

                return posts?.Length > 0
                    ? Response.Ok()
                    : Response.Fail();
            });

            var scenario1 = ScenarioBuilder
                .CreateScenario("rest_api", getUser, getPosts)
                .WithInit(ScenarioInit);

            // the following scenario will be ignored since it is not included in target scenarios list of config.json
            var scenario2 = ScenarioBuilder
                .CreateScenario("ignored", getUser, getPosts);

            // settings for plugins and reporting sinks are overriden in infra-config.json
            var pingPlugin = new PingPlugin();

            var customPlugin = new CustomPlugin(new CustomPluginSettings
                {Message = "Plugin is configured via constructor"});

            var customReportingSink = new CustomReportingSink(new CustomReportingSinkSettings
                {Message = "Reporting sink is configured via constructor"});

            NBomberRunner
                .RegisterScenarios(scenario1, scenario2)
                .WithWorkerPlugins(pingPlugin, customPlugin)
                .WithReportingSinks(customReportingSink)
                .LoadConfig("./JsonConfig/Configs/config.json")
                .LoadInfraConfig("./JsonConfig/Configs/infra-config.json")
                .Run();
        }
    }
}
