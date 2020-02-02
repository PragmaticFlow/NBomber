using System;
using System.Linq;
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
            var step = Step.Create("step", async context =>
            {
                // you can do any logic here: go to http, websocket etc

                await Task.Delay(TimeSpan.FromSeconds(0.1));
                var input = context.Data["words"];
                Console.WriteLine($"Data from feed: {input}");
                return Response.Ok();
            });

            var feed = Feed.Circular( "words", Enumerable.Range(0, 10));

            var tryNextTime = 1 != 0;
            if (tryNextTime)
            {
                var seq = Feed.Empty<object>();
                var rnd1 = Feed.Shuffle("index", new[] {1,2,3});
                var rnd2 = Feed.RandomFloats("length", 0.0, 99.9);
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
