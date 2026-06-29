using System.Text.Json;
using HttpApiSimulator.Contracts;
using StackExchange.Redis;

namespace HttpApiSimulator.Infra.DAL;

public class RedisRepository: IUserRepository
{
    private ConnectionMultiplexer _redis;
    private IDatabase _database;
    private RedisSettings _settings;

    public RedisRepository(RedisSettings settings)
    {
        _settings = settings;
        _redis = ConnectionMultiplexer.Connect(_settings.ConnectionString);
    }

    public void CreateDB()
    {
        _database = _redis.GetDatabase();
    }

    public void DeleteTable()
    {
        var server = _redis.GetServer(_settings.ServerName);
        server.FlushDatabase();
    }

    public async ValueTask<User> GetById(int id)
    {
        byte[] data = await _database.StringGetAsync(id.ToString());
        var user = data != null ? JsonSerializer.Deserialize<User>(data) : null;
        return user;
    }

    public ValueTask<bool> Update(User user)
    {
        var data = JsonSerializer.SerializeToUtf8Bytes(user);
        return new ValueTask<bool>(_database.StringSetAsync(user.Id.ToString(), data));
    }
}
