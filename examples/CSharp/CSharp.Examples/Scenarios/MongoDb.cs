using System;
using System.Linq;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Examples.Scenarios
{
    class MongoDbScenario
    {
        public class User
        {
            [BsonId]
            public ObjectId Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public bool IsActive { get; set; }
        }

        public static Scenario BuildScenario()
        {
            var db = new MongoClient().GetDatabase("Test");

            Action initDb = () =>
            {
                var usersCollection = db.GetCollection<User>("Users");

                var testData = Enumerable.Range(0, 2000)
                    .Select(i => new User { Name = $"Test User {i}", Age = i, IsActive = true })
                    .ToList();

                db.DropCollection("Users");
                usersCollection.InsertMany(testData);
            };

            var pool = ConnectionPool.Create("mongoPool", () => db.GetCollection<User>("Users"));

            var step1 = Step.CreatePull("read IsActive = true and TOP 500", pool, async context =>
            {
                await context.Connection.Find(u => u.IsActive == true)
                                        .Limit(500)
                                        .ToListAsync();
                return Response.Ok();
            });

            var step2 = Step.CreatePull("read Age > 50 and TOP 100", pool, async context =>
            {
                await context.Connection.Find(u => u.IsActive == true)
                                        .Limit(500)
                                        .ToListAsync();
                return Response.Ok();
            });

            return ScenarioBuilder.CreateScenario("test mongo", step1, step2)
                                  .WithTestInit(initDb);                                  
        }
    }
}
