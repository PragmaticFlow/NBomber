using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;

namespace Demo.DB.MongoDB
{
    public class MongoConfig
    {
        public string ConnectionString { get; set; }
        public int DataSize { get; set; }
        public int UsersCount { get; set; }
    }

    public class MongoInitScenario
    {
        public ScenarioProps Create()
        {
            return Scenario.Empty("mongo_init")
                .WithInit(async context =>
                {
                    var config = context.GlobalCustomSettings.Get<MongoConfig>();
                    var client = new MongoClient(config.ConnectionString);
                    var db = client.GetDatabase("MongoDb");
                    var usersCollection = db.GetCollection<User>("Users");
                    var data = Data.GenerateRandomBytes(config.DataSize);

                    var users = new List<User>();

                    foreach (var i in Enumerable.Range(0, config.UsersCount))
                    {
                        users.Add(new User() { Id = i, Data = data });
                    }

                    await usersCollection.DeleteManyAsync(x => true);
                    await usersCollection.InsertManyAsync(users);
                });
        }
    }
}
