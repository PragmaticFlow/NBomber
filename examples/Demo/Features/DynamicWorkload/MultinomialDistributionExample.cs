using NBomber.CSharp;

namespace Demo.Features.DynamicWorkload;

public class MultinomialDistributionExample
{
    public void Run()
    {
        // 70% for read, 20% for write, 10% for delete
        var items = new [] { ("read", 70), ("write", 20), ("delete", 10) };

        var scenario = Scenario.Create("multinomial_distribution", async context =>
        {
            var randomItem = context.Random.Choice(items);
            switch (randomItem)
            {
                case "read": // 70% for read
                    await Step.Run("read", context, async () =>
                    {
                        await Task.Delay(100);
                        return Response.Ok();
                    });
                    break;

                case "write": // 20% for write
                    await Step.Run("write", context, async () =>
                    {
                        await Task.Delay(100);
                        return Response.Ok();
                    });
                    break;

                case "delete": // 10% for delete
                    await Step.Run("delete", context, async () =>
                    {
                        await Task.Delay(100);
                        return Response.Ok();
                    });
                    break;
            }

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
