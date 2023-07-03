using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;
using StackExchange.Redis;

namespace CSharpProd.DB.Redis;

public class RedisWriteScenario
{
    private RedisDbConfig _dbConfig;
    private ConnectionMultiplexer _redis;
    private IDatabase _db;
    private readonly Random _random = new();
    private byte[] _payload;

    public ScenarioProps Create()
    {
        return Scenario
            .Create("redis_write", async context =>
            {
                var randomId = _random.Next(_dbConfig.RecordsCount);
                await _db.StringSetAsync($"user-{randomId}", _payload);
                return Response.Ok(sizeBytes: _payload.Length);
            })
            .WithInit(context =>
            {
                _dbConfig = context.GlobalCustomSettings.Get<RedisDbConfig>();
                _redis = ConnectionMultiplexer.Connect(_dbConfig.ConnectionString);
                _db = _redis.GetDatabase();
                _payload = Data.GenerateRandomBytes(_dbConfig.RecordSize);

                return Task.CompletedTask;
            });
    }
}
