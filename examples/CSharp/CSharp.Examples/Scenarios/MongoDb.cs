using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Extensions;

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

        public static void Run(string[] args)
        {
            var db = new MongoClient().GetDatabase("Test");

            Task initDb(ScenarioContext context)
            {
                var testData = Enumerable.Range(0, 2000)
                    .Select(i => new User { Name = $"Test User {i}", Age = i, IsActive = true })
                    .ToList();

                db.DropCollection("Users", context.CancellationToken);
                return db.GetCollection<User>("Users")
                         .InsertManyAsync(testData, cancellationToken: context.CancellationToken);
            }

            var usersCollection = db.GetCollection<User>("Users");

            var step = Step.Create("read IsActive = true and TOP 500", async context =>
            {
                await usersCollection.Find(u => u.IsActive)
                                     .Limit(500)
                                     .ToListAsync(context.CancellationToken);
                return Response.Ok();
            });

            var scenario = ScenarioBuilder.CreateScenario("test_mongo", new[] { step })
                                          .WithTestInit(initDb);

            NBomberRunner
                .RegisterScenarios(new[] {scenario})
                .RunInConsole();
        }
    }
}
