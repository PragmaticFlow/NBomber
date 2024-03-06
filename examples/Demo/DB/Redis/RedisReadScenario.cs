using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using StackExchange.Redis;

namespace Demo.DB.Redis;

public class RedisReadScenario
{
    private RedisDbConfig _dbConfig;
    private ConnectionMultiplexer _redis;
    private IDatabase _db;
    private readonly Random _random = new();

    public ScenarioProps Create()
    {
        return Scenario
            .Create("redis_read", async context =>
            {
                var randomId = _random.Next(_dbConfig.RecordsCount);
                byte[] data = await _db.StringGetAsync($"user-{randomId}");
                return Response.Ok(sizeBytes: data.Length);
            })
            .WithInit(context =>
            {
                _dbConfig = context.GlobalCustomSettings.Get<RedisDbConfig>();
                _redis = ConnectionMultiplexer.Connect(_dbConfig.ConnectionString);
                _db = _redis.GetDatabase();

                return Task.CompletedTask;
            });
    }
}
