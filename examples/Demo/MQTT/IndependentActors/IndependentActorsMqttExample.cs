using NBomber.CSharp;

namespace Demo.MQTT.IndependentActors;

public class IndependentActorsMqttExample
{
    public void Run()
    {
        NBomberRunner.RegisterScenarios(
            new MqttPublishScenario().Create(),
            new MqttConsumeScenario().Create()
        )
        .LoadConfig("./MQTT/IndependentActors/config.json")
        .Run();
    }
}
