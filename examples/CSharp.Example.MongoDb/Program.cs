using System;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Example.MongoDb
{
    class Program
    {
        public class User
        {
            [BsonId]
            public ObjectId Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public bool IsActive { get; set; }
        }

        static Scenario BuildScenario()
        {
            var db = new MongoClient().GetDatabase("Test");
            var usersCollection = db.GetCollection<User>("Users");

            var testData = Enumerable.Range(0, 2000)
                                     .Select(i => new User { Name = $"Test User {i}", Age = i, IsActive = true })
                                     .ToList();

            Func<Request, Task<Response>> initDb = async _ =>
            {
                db.DropCollection("Users");
                await usersCollection.InsertManyAsync(testData);
                return Response.Ok();
            };

            var readQuery1 = usersCollection.Find(u => u.IsActive == true).Limit(500);
            var readQuery2 = usersCollection.Find(u => u.Age > 50).Limit(100);

            var step1 = Step.CreateRequest("read IsActive = true and TOP 500", async _ =>
            {
                await readQuery1.ToListAsync();
                return Response.Ok();
            });

            var step2 = Step.CreateRequest("read Age > 50 and TOP 100", async _ =>
            {
                await readQuery2.ToListAsync();
                return Response.Ok();
            });

            return new ScenarioBuilder(scenarioName: "Test MongoDb with 2 READ quries and 2000 docs")
                .AddTestInit(initDb)
                .AddTestFlow("READ Users 1", steps: new[] { step1 }, concurrentCopies: 20)
                .AddTestFlow("READ Users 2", steps: new[] { step2 }, concurrentCopies: 20)
                .Build(duration: TimeSpan.FromSeconds(10));
        }

        static void Main(string[] args)
        {
            var scenario = BuildScenario();
            scenario.Run();
        }
    }
}
