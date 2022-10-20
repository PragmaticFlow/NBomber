using Microsoft.FSharp.Core;

namespace CSharpDev.HelloWorld;

using System;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;

public class HelloWorldExample
{
    public static void Run()
    {
        // doNotTrack = true
        // statusCode = string
        // stepTimeout
        // scenarioTimeOut
        // disableHintAnalizer on data transfer

        // var scenario = Scenario.Create("scenario", async context =>
        // {
            // var exc = new Exception("asdasd");
            // var dd = FlowResponse.Fail("");
            // var dd2 = FlowResponse.Fail(exc);
            // var dd3 = FlowResponse.Fail();
            //
            // var ok = FlowResponse.Ok();
            // var ok1 = FlowResponse.Ok(92);

        //     var response1 = await Step.Run("step_1", context, async () =>
        //     {
        //         await Task.Delay(TimeSpan.FromMilliseconds(1000));
        //         return FlowResponse.Ok();
        //     });
        //
        //     var response2 = await Step.Run("step_2", context, async () =>
        //     {
        //         await Task.Delay(TimeSpan.FromMilliseconds(1000));
        //         return FlowResponse.Ok("ok response");
        //     });
        //
        //     var response3 = await Step.Run("step_3", context, async () =>
        //     {
        //         await Task.Delay(TimeSpan.FromMilliseconds(1000));
        //         return FlowResponse.Fail("fail response");
        //     });
        //
        //
        //     //response1.IsError
        //
        //     //response3.Payload.IsNone()
        //     if (response3.Payload.IsSome())
        //     {
        //
        //     }
        //
        //     var response4 = await Step.Run("step_4", context, async () =>
        //     {
        //         await Task.Delay(TimeSpan.FromMilliseconds(1000));
        //         return FlowResponse.Fail();
        //     });
        //
        //     var response = Step.Run("step_5", context, async () =>
        //     {
        //         await Task.Delay(TimeSpan.FromMilliseconds(1000));
        //         return FlowResponse.Ok(92);
        //     });
        //
        //     return FlowResponse.Ok();
        // })
        //     .WithSteps(Step.Create("mystep", async ctx => Response.Ok()))
        //     .WithoutWarmUp();
        //
        // NBomberRunner
        //     .RegisterScenarios(scenario)
        //     .Run();

        // // here you create scenario and define (default) step order
        // // you also can define them in opposite direction, like [step2; step1]
        // // or even repeat [step1; step1; step1; step2]
        // var scenario = Scenario
        //     .Create("hello_world_scenario", step1, pause, step2)
        //     .WithoutWarmUp()
        //     .WithLoadSimulations(
        //         Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
        //     );
        //
        // NBomberRunner
        //     .RegisterScenarios(scenario)
        //     .WithTestSuite("example")
        //     .WithTestName("hello_world_test")
        //     .Run();
    }
}
