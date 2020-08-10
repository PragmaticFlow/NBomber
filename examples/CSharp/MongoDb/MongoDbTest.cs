using System;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.MongoDb
{
    // in this example we use:
    // - MongoDB.Driver (https://github.com/mongodb/mongo-csharp-driver)

    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsActive { get; set; }
    }

    public class MongoDbTest
    {
        public static void Run()
        {
            var db = new MongoClient().GetDatabase("Test");

            Task initDb(IScenarioContext context)
            {
                var testData = Enumerable.Range(0, 2000)
                    .Select(i => new User { Name = $"Test User {i}", Age = i, IsActive = true })
                    .ToList();

                db.DropCollection("Users", context.CancellationToken);
                return db.GetCollection<User>("Users")
                    .InsertManyAsync(testData, cancellationToken: context.CancellationToken);
            }

            var usersCollection = db.GetCollection<User>("Users");

            var step = Step.Create("query_users", async context =>
            {
                await usersCollection.Find(u => u.IsActive)
                    .Limit(500)
                    .ToListAsync(context.CancellationToken);
                return Response.Ok();
            });

            var scenario = ScenarioBuilder
                .CreateScenario("mongo_scenario", step)
                .WithInit(initDb)
                .WithLoadSimulations(new []
                {
                    Simulation.KeepConstant(copies: 100, during: TimeSpan.FromSeconds(30))
                });

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithTestSuite("mongo")
                .WithTestName("simple_query_test")
                .Run();
        }
    }
}
