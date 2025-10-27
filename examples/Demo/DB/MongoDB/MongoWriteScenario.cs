using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;

namespace Demo.DB.MongoDB
{
    public class MongoWriteScenario
    {
        private MongoConfig _config;
        private IMongoCollection<User> _users;
        private byte[] _data;

        public ScenarioProps Create()
        {
            return Scenario.Create("mongo_write", async context =>
            {
                var id = context.Random.Next(_config.UsersCount);
                var user = new User() { Id = id, Data = _data };
                var result = await _users.ReplaceOneAsync(x => x.Id == id, user, new ReplaceOptions() { IsUpsert = true });

                return result.IsAcknowledged
                ? Response.Ok(sizeBytes: _data.Length)
                : Response.Fail();
            })
            .WithInit(context =>
            {
                _config = context.GlobalCustomSettings.Get<MongoConfig>();
                _data = Data.GenerateRandomBytes(_config.DataSize);

                var settings = MongoClientSettings.FromConnectionString(_config.ConnectionString);
                settings.ConnectTimeout = TimeSpan.FromSeconds(5);
                var connection = new MongoClient(settings);
                var db = connection.GetDatabase("MongoDb");
                _users = db.GetCollection<User>("Users");

                return Task.CompletedTask;
            });
        }
    }
}
