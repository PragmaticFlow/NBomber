using NBomber.CSharp;

namespace Demo.AMQP.IndependentActors;

public class IndependentActorsAmqpExample
{
    public void Run()
    {
        NBomberRunner.RegisterScenarios(
            new AmqpPublishScenario().Create(),
            new AmqpConsumeScenario().Create()
        )
        .Run();
    }
}
