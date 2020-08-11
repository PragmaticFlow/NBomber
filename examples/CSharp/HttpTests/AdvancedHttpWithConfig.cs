using System;
using System.Linq;

using Newtonsoft.Json;

using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using NBomber.Plugins.Network.Ping;

namespace CSharp.HttpTests
{
    public class UserId
    {
        public string Id { get; set; }
    }

    public class AdvancedHttpWithConfig
    {
        public static void Run()
        {
            var userFeed = Feed.CreateRandom(
                name: "userFeed",
                provider: FeedData.FromJson<UserId>("./HttpTests/Configs/user-feed.json")
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
                            : Response.Fail("not found user");
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
                .CreateScenario("rest_api", getUser, getPosts);

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithPlugins(new PingPlugin())
                .LoadConfig("./HttpTests/Configs/config.json")
                .LoadInfraConfig("./HttpTests/Configs/infra-config.json")
                .Run();
        }
    }
}
