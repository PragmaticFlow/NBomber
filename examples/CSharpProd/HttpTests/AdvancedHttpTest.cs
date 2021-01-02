using System;
using System.Linq;
using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using NBomber.Plugins.Network.Ping;
using Newtonsoft.Json;

namespace CSharpProd.HttpTests
{
    // in this example we use:
    // - NBomber.Http (https://nbomber.com/docs/plugins-http)

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

    public class AdvancedHttpTest
    {
        public static void Run()
        {
            var userFeed = Feed.CreateRandom(
                name: "userFeed",
                provider: FeedData.FromSeq(new[] {"1", "2", "3", "4", "5"})
            );

            var getUser = HttpStep.Create("get_user", userFeed, context =>
            {
                var userId = context.FeedItem;
                var url = $"https://jsonplaceholder.typicode.com/users?id={userId}";

                return Http.CreateRequest("GET", url)
                    .WithCheck(async response =>
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        // parse JSON
                        var users = JsonConvert.DeserializeObject<UserResponse[]>(json);

                        return users?.Length == 1
                            ? Response.Ok(users.First()) // we pass user object response to the next step
                            : Response.Fail($"not found user: {userId}");
                    });
            });

            // this 'getPosts' will be executed only if 'getUser' finished OK.
            var getPosts = HttpStep.Create("get_posts", context =>
            {
                var user = context.GetPreviousStepResponse<UserResponse>();
                var url = $"https://jsonplaceholder.typicode.com/posts?userId={user.Id}";

                return Http.CreateRequest("GET", url)
                    .WithCheck(async response =>
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        // parse JSON
                        var posts = JsonConvert.DeserializeObject<PostResponse[]>(json);

                        return posts?.Length > 0
                            ? Response.Ok()
                            : Response.Fail($"not found posts for user: {user.Id}");
                    });
            });

            var scenario = ScenarioBuilder
                .CreateScenario("rest_api", getUser, getPosts)
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(new[]
                {
                    Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromSeconds(30))
                });

            var pingPluginConfig = PingPluginConfig.CreateDefault(new[] {"jsonplaceholder.typicode.com"});
            var pingPlugin = new PingPlugin(pingPluginConfig);

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithWorkerPlugins(pingPlugin)
                .WithTestSuite("http")
                .WithTestName("advanced_test")
                .Run();
        }
    }
}
