using NBomber.CSharp;

namespace Demo.MQTT.IndependentActors;

public class MqttCustomSettings
{
    public string MqttServerUrl { get; set; }
    public int MsgSizeBytes { get; set; }
}

public class IndependentActorsMqttExample
{
    // For this example, please spin up local MQTT broker via docker-compose.yml located in the MQTT folder.

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
