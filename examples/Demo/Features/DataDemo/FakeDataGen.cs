using Bogus;
using NBomber.CSharp;
using NBomber.Data;
using NBomber.Data.CSharp;

namespace Demo.Features.DataDemo;

public class FakeUser
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
}

public class FakeDataGenExample
{
    private IDataFeed<FakeUser> _usersFeed;

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
            // we crate 5 users and our Simulation.KeepConstant(copies: 5)
            var users = GenerateFakeUsers(5).ToArray();

            _usersFeed = DataFeed.Constant(users);

            return Task.CompletedTask;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 5, during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    private IEnumerable<FakeUser> GenerateFakeUsers(int count)
    {
        var faker = new Faker<FakeUser>()
            .RuleFor(u => u.Id, f => f.UniqueIndex)
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Address, f => f.Address.FullAddress());

        return faker.GenerateLazy(count);
    }
}
