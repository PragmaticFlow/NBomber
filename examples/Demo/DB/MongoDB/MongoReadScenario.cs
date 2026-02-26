using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NBomber.Contracts;
using NBomber.CSharp;

namespace Demo.DB.MongoDB
{
    public class MongoReadScenario
    {
        private MongoConfig _config;
        private IMongoCollection<User> _users;

        public ScenarioProps Create()
        {
            return Scenario.Create("mongo_read", async context =>
            {
                var id = context.Random.Next(_config.UsersCount);
                var user = (await _users.FindAsync(x => x.Id == id)).FirstOrDefault();

                return user != null
                    ? Response.Ok(sizeBytes: user.Data.Length)
                    : Response.Fail(message: "Not found");
            })
            .WithInit(context =>
            {
                _config = context.GlobalCustomSettings.Get<MongoConfig>();
                var connection = new MongoClient(_config.ConnectionString);
                var db = connection.GetDatabase("MongoDb");
                _users = db.GetCollection<User>("Users");

                return Task.CompletedTask;
            });
        }
    }
}
