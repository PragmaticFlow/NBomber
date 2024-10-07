using NBomber.CSharp;
using NBomber.Data;
using NBomber.Data.CSharp;

namespace Demo.Features.DataDemo;

public class CsvUser
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class CsvFeed
{
    private IDataFeed<CsvUser> _usersFeed;

    public void Run()
    {
        var scenario = Scenario.Create("scenario", async ctx =>
        {
            var user = _usersFeed.GetNextItem(ctx.ScenarioInfo);

            await Task.Delay(1_000);

            ctx.Logger.Information($"ScenarioCopyId: {ctx.ScenarioInfo.InstanceNumber}, UserId: {user.Id}");

            return Response.Ok();
        })
        .WithInit(ctx =>
        {
            var users = Data.LoadCsv<CsvUser>("./Features/DataDemo/data.csv");

            // you can also load CSV data by URL
            // var users = Data.LoadCsv<CsvUser>("http:// path to csv file");

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
