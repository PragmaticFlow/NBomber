using NBomber.CSharp;
using NBomber.Data;
using NBomber.Data.CSharp;
// ReSharper disable CheckNamespace

namespace Demo.Features;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class DataFeedExample
{
    public void Run()
    {
        //-------- Load by file --------//

        //var data = new[] {1, 2, 3, 4, 5};
        //var data = Data.LoadCsv<User>("./Features/DataFeed/users-feed-data.csv");
        var data = Data.LoadJson<User[]>("./Features/DataFeed/users-feed-data.json");

        //-------- Load by URL ---------//

        //var data = Data.LoadCsv<User>("https://raw.githubusercontent.com/PragmaticFlow/NBomber/e54c45912b1826f54376a8668da556aeb922b9d6/examples/Demo/DataFeed/users-feed-data.csv");
        //var data = Data.LoadJson<User[]>("https://raw.githubusercontent.com/PragmaticFlow/NBomber/e54c45912b1826f54376a8668da556aeb922b9d6/examples/Demo/DataFeed/users-feed-data.json");

        //var feed = DataFeed.Constant(data);
        //var feed = DataFeed.Random(data);
        var feed = DataFeed.Circular(data);

        var scenario = Scenario.Create("scenario", async context =>
        {
            var item = feed.GetNextItem(context.ScenarioInfo);
            context.Logger.Information("Data from feed: {0}", item.Name);

            await Task.Delay(1_000);
            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 2, during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
