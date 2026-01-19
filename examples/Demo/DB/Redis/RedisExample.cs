using NBomber.CSharp;

namespace Demo.DB.Redis;

public class RedisExample
{
    public void Run()
    {
        var scn1 = new RedisInitScenario().Create();
        var scn2 = new RedisReadScenario().Create();
        var scn3 = new RedisWriteScenario().Create();

        NBomberRunner.RegisterScenarios(scn1, scn2, scn3)
            .LoadConfig("./DB/Redis/config.json")
            .Run();
    }
}
