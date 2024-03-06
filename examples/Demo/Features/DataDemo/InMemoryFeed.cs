using NBomber.CSharp;
using NBomber.Data;
using NBomber.Data.CSharp;

namespace Demo.Features.DataDemo;

public class InMemoryFeed
{
    private IDataFeed<FakeUser> _usersFeed;

    public void Run()
    {
        var scenario = Scenario.Create("scenario", async ctx =>
        {
            var user = _usersFeed.GetNextItem(ctx.ScenarioInfo);

            await Task.Delay(1_000);

            ctx.Logger.Information($"ScenarioCopy Id: {ctx.ScenarioInfo.ThreadNumber}, User ID: {user.Id}");

            return Response.Ok();
        })
        .WithInit(ctx =>
        {
            // we crate 3 users and our Simulation.KeepConstant(copies: 3)
            var users = new[]
            {
                new FakeUser { Id = 0 },
                new FakeUser { Id = 1 },
                new FakeUser { Id = 2 }
            };

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
