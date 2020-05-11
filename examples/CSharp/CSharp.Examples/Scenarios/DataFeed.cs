using System;
using System.Threading.Tasks;

using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Examples
{
    class DataFeedScenario
    {
        public static void Run()
        {
            var data = FeedData.FromSeq(new[] {1, 2, 3, 4, 5}).ShuffleData();
            //var data = FeedData.FromJson<User>("users_feed_data.json");
            //var data = FeedData.FromCsv<User>("users_feed_data.csv");

            var feed = Feed.CreateCircular("numbers", data);
            //var feed = Feed.CreateConstant("numbers", data);
            //var feed = Feed.CreateRandom("numbers", data);

            var step = Step.Create("step", feed, async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                context.Logger.Information("Data from feed: {FeedItem}", context.FeedItem);
                return Response.Ok();
            });

            var scenario = ScenarioBuilder.CreateScenario("Hello World!", new[] {step});

            NBomberRunner
                .RegisterScenarios(new[] {scenario})
                .RunInConsole();
        }

        public class User
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}
