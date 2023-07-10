using System.Text.Json;
using StackExchange.Redis;
using WebAppSimulator.Contracts;

namespace WebAppSimulator.Infra.DAL
{
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

        public void DeleTable()
        {
            var server = _redis.GetServer(_settings.ServerName);
            server.FlushDatabase();           
        }

        public async Task<User> GetById(int id)
        {
            var data = await _database.StringGetAsync(id.ToString());
            var user = JsonSerializer.Deserialize<User>(data);
            return user;
        }

        public async Task Insert(User user)
        {
            var data  = JsonSerializer.SerializeToUtf8Bytes(user);
            await _database.StringSetAsync(user.Id.ToString(), data);
        }

        public Task<bool> Update(User user)
        {
            var data = JsonSerializer.SerializeToUtf8Bytes(user);
            return _database.StringSetAsync(user.Id.ToString(), data);
        }
    }
}
