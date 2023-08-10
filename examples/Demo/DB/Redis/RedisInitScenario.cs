using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;
using StackExchange.Redis;

namespace Demo.DB.Redis;

public class RedisDbConfig
{
    public string ConnectionString { get; set; }
    public int RecordsCount { get; set; }
    public int RecordSize { get; set; }
}

public class RedisInitScenario
{
    public ScenarioProps Create()
    {
        return Scenario.Empty("redis_init")
            .WithInit(context =>
            {
                var dbConfig = context.GlobalCustomSettings.Get<RedisDbConfig>();

                var redis = ConnectionMultiplexer.Connect(dbConfig.ConnectionString);
                var db = redis.GetDatabase();
                var payload = Data.GenerateRandomBytes(dbConfig.RecordSize);

                foreach (var i in Enumerable.Range(0, dbConfig.RecordsCount))
                {
                    db.StringSet($"user-{i}", payload);
                }

                return Task.CompletedTask;
            });
    }
}
