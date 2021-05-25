using System.Linq;
using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using NBomber.Plugins.Network.Ping;
using Newtonsoft.Json;

namespace CSharpProd.HttpTests
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
                "userFeed",
                FeedData.FromJson<UserId>("./HttpTests/Configs/user-feed.json")
            );
            var httpFactory = HttpClientFactory.Create();

            var getUser = Step.Create("get_user", httpFactory, userFeed, async context =>
            {
                var userId = context.FeedItem;
                var url = $"https://jsonplaceholder.typicode.com/users?id={userId}";

                var request =  Http.CreateRequest("GET", url)
                    .WithCheck(async response =>
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        // parse JSON
                        var users = JsonConvert.DeserializeObject<UserResponse[]>(json);

                        return users?.Length == 1
                            ? Response.Ok(users.First()) // we pass user object response to the next step
                            : Response.Fail("not found user");
                    });
                var response = await Http.Send(request, context);
                return response;
            });

            // this 'getPosts' will be executed only if 'getUser' finished OK.
            var getPosts = Step.Create("get_posts", httpFactory, async context =>
            {
                var user = context.GetPreviousStepResponse<UserResponse>();
                var url = $"https://jsonplaceholder.typicode.com/posts?userId={user.Id}";

                var request =  Http.CreateRequest("GET", url)
                    .WithCheck(async response =>
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        // parse JSON
                        var posts = JsonConvert.DeserializeObject<PostResponse[]>(json);

                        return posts?.Length > 0
                            ? Response.Ok()
                            : Response.Fail($"not found posts for user: {user.Id}");
                    });
                var response = await Http.Send(request, context);
                return response;
            });

            var scenario = ScenarioBuilder
                .CreateScenario("rest_api", getUser, getPosts);

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithWorkerPlugins(new PingPlugin())
                .LoadConfig("./HttpTests/Configs/config.json")
                .LoadInfraConfig("./HttpTests/Configs/infra-config.json")
                .Run();
        }
    }
}
