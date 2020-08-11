using System;
using System.Threading.Tasks;

using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.DataFeed
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class DataFeedTest
    {
        public static void Run()
        {
            var data = FeedData.FromSeq(new[] {1, 2, 3, 4, 5}).ShuffleData();
            //var data = FeedData.FromJson<User>("./DataFeed/users-feed-data.json");
            //var data = FeedData.FromCsv<User>("./DataFeed/users-feed-data.csv");

            var feed = Feed.CreateCircular("numbers", data);
            //var feed = Feed.CreateConstant("numbers", data);
            //var feed = Feed.CreateRandom("numbers", data);

            var step = Step.Create("step", feed, async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                context.Logger.Debug("Data from feed: {FeedItem}", context.FeedItem);
                return Response.Ok();
            });

            var scenario = ScenarioBuilder.CreateScenario("data_feed_scenario", step);

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}
