using System;
using System.Threading.Tasks;

using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Extensions;

namespace CSharp.Examples.Scenarios
{
    public class FakeSocketClient
    {
        public int Id { get; set; }
    }

    public class CustomScenarioSettings
    {
        public int TestField { get; set; }
    }

    public class AdvancedHelloWorldScenario
    {
        CustomScenarioSettings _settings = new CustomScenarioSettings();

        Task TestInit(ScenarioContext context)
        {
            if (!String.IsNullOrEmpty(context.CustomSettings))
            {
                _settings = context.CustomSettings.DeserializeJson<CustomScenarioSettings>();
                context.Logger.Information("test init received CustomSettings.TestField '{TestField}'", _settings.TestField);
            }
            return Task.CompletedTask;
        }

        Task TestClean(ScenarioContext context)
        {
            return Task.CompletedTask;
        }

        public void Run()
        {
            var data = FeedData.FromSeq(new[] {1, 2, 3, 4, 5});
            var dataFeed = Feed.CreateRandom("random_feed", data);

            var webSocketConnectionPool =
                ConnectionPoolArgs.Create(
                    name: "web_socket_pool",
                    getConnectionCount: () => 10,
                    openConnection: async (number, token) =>
                    {
                        await Task.Delay(1_000);
                        return new FakeSocketClient {Id = number};
                    },
                    closeConnection: (connection, token) =>
                    {
                        Task.Delay(1_000).Wait();
                        return Task.CompletedTask;
                    });

            var step1 = Step.Create("step_1", webSocketConnectionPool, dataFeed, async context =>
            {
                // you can do any logic here: go to http, websocket etc

                // context.CorrelationId - every copy of scenario has correlation id
                // context.Connection    - fake websocket connection taken from pool
                // context.FeedItem      - item taken from data feed
                // context.Logger
                // context.StopScenario("hello_world_scenario", reason = "")
                // context.StopTest(reason = "")

                await Task.Delay(TimeSpan.FromMilliseconds(200));
                return Response.Ok(42);    // this value will be passed as response for the next step

                // return Response.Ok(42, sizeBytes: 100, latencyMs: 100); - you can specify response size and custom latency
                // return Response.Fail();                                 - in case of fail, the next step will be skipped
            });

            var step2 = Step.Create("step_2", async context =>
            {
                // you can do any logic here: go to http, websocket etc

                await Task.Delay(TimeSpan.FromMilliseconds(200));
                var value = context.GetPreviousStepResponse<int>(); // 42
                return Response.Ok();
            });

            var scenario = ScenarioBuilder
                .CreateScenario("hello_world_scenario", new[] { step1, step2 })
                .WithTestInit(TestInit)
                .WithTestClean(TestClean)
                .WithWarmUpDuration(TimeSpan.FromSeconds(10))
                // .WithOutWarmUp() - disable warm up
                .WithLoadSimulations(new []
                {
                    Simulation.RampConcurrentScenarios(copiesCount: 20, during: TimeSpan.FromSeconds(20)),
                    Simulation.KeepConcurrentScenarios(copiesCount: 20, during: TimeSpan.FromMinutes(1)),
                    // Simulation.RampScenariosPerSec(copiesCount: 10, during: TimeSpan.FromSeconds(20)),
                    // Simulation.InjectScenariosPerSec(copiesCount: 10, during: TimeSpan.FromMinutes(1))
                });

            NBomberRunner
                .RegisterScenarios(new[] {scenario})
                // .LoadTestConfig("test_config.json")   // test config for test settings only
                // .LoadInfraConfig("infra_config.json") // infra config for infra settings only
                .RunInConsole();
        }
    }
}
