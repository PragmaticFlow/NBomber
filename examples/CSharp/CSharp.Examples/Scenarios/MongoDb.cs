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

        public static void Run()
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

            var step = Step.Create("read IsActive = true and TOP 500", async context =>
            {
                await usersCollection.Find(u => u.IsActive == true)
                                     .Limit(500)
                                     .ToListAsync(context.CancellationToken);
                return Response.Ok();
            });

            var scenario = ScenarioBuilder.CreateScenario("test_mongo", step)
                                          .WithTestInit(initDb);

            NBomberRunner.RegisterScenarios(scenario)                         
                         .RunInConsole();
        }
    }
}
