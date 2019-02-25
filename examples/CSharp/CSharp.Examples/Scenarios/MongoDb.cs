using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

            Task initDb(CancellationToken token)
            {
                var testData = Enumerable.Range(0, 2000)
                    .Select(i => new User { Name = $"Test User {i}", Age = i, IsActive = true })
                    .ToList();

                db.DropCollection("Users", token);
                return db.GetCollection<User>("Users")
                         .InsertManyAsync(testData, cancellationToken: token);
            }

            var usersCollection = db.GetCollection<User>("Users");

            var step1 = Step.CreateAction("read IsActive = true and TOP 500", ConnectionPool.None, async context =>
            {
                await usersCollection.Find(u => u.IsActive == true)
                                     .Limit(500)
                                     .ToListAsync(context.CancellationToken);
                return Response.Ok();
            });

            return ScenarioBuilder.CreateScenario("test_mongo", step1)
                                  .WithTestInit(initDb);                                  
        }
    }
}
