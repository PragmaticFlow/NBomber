using System;
using System.Linq;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;
using Serilog;

namespace CSharp.Examples
{
    class DataFeedScenario
    {
        public static void Run()
        {
            var feed =
                Feed
                    .Circular("numbers", Enumerable.Range(0, 10))
                    .Select(x => (x + 1) * 10)
                    .Select(x => x.ToString());

            var step = Step.Create("step", async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1));
                // to access feed data use feed name as a key
                var number = (string)context.Data["feed.numbers"];
                context.Logger.Information("Data from feed: {number}", number);
                return Response.Ok();
            });

            var tryNextTime = "Feeds to try as next";
            if (string.IsNullOrWhiteSpace(tryNextTime))
            {
                var nop = Feed.Empty;
                var seq = Feed.Sequence("index", new[] {1,2,3});
                var cir = Feed.Circular("index", new[] {1,2,3});
                var sfl = Feed.Shuffle("index", new[] {1,2,3});
                var csv = Feed.FromJson<User>("user", "C:/Files/users.json");
            }

            var scenario = ScenarioBuilder
                .CreateScenario("Hello World!", new[] { step })
                .WithFeed(feed);

            NBomberRunner.RegisterScenarios(scenario)
                         .RunInConsole();
        }

        public class User
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}
