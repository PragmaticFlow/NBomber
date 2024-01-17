using NBomber.CSharp;
using NBomber.Data;
using NBomber.Data.CSharp;

namespace Demo.Features.DataDemo;

public class JsonUser
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class JsonFeed
{
    private IDataFeed<JsonUser> _usersFeed;

    public void Run()
    {
        var scenario = Scenario.Create("scenario", async ctx =>
        {
            var user = _usersFeed.GetNextItem(ctx.ScenarioInfo);

            await Task.Delay(1_000);

            ctx.Logger.Information($"ScenarioCopyId: {ctx.ScenarioInfo.ThreadNumber}, UserId: {user.Id}");

            return Response.Ok();
        })
        .WithInit(ctx =>
        {
            var users = Data.LoadJson<JsonUser[]>("./Features/DataDemo/data.json");

            // you can also load CSV data by URL
            // var users = Data.LoadJson<JsonUser[]>("http:// path to json file");

            _usersFeed = DataFeed.Constant(users);

            return Task.CompletedTask;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 3, during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
