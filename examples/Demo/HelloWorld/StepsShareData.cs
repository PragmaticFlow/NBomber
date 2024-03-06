using NBomber.CSharp;

namespace Demo.HelloWorld;

public class StepsShareData
{
    public void Run()
    {
        var scenario = Scenario.Create("hello_world_scenario", async context =>
        {
            var step1 = await Step.Run("step_1", context, async () =>
            {
                await Task.Delay(1000);

                // OPTION 1
                // we can share data between steps via Response type

                return Response.Ok(payload: "step_1 response", sizeBytes: 10);
            });

            // now we can get data from step1
            if (step1.Payload.IsSome())
            {
                var step1Response = step1.Payload.Value;
                context.Logger.Information(step1Response);
            }

            var step2 = await Step.Run("step_2", context, async () =>
            {
                await Task.Delay(1000);

                // OPTION 2
                // we can share data between steps via context.Data dictionary
                context.Data["item_1"] = "txt";
                context.Data["item_2"] = 2;

                return Response.Ok();
            });

            context.Logger.Information(context.Data["item_1"].ToString());
            context.Logger.Information(context.Data["item_2"].ToString());

            // OPTION 3
            // we can share data between steps via regular variables (closure)

            var myData = "";

            var step3 = await Step.Run("step_3", context, async () =>
            {
                await Task.Delay(1000);

                myData = "data from step 3";

                return Response.Ok();
            });

            context.Logger.Information(myData);

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
