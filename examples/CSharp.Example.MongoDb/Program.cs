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

            var step1 = Step.CreatePull("read IsActive = true and TOP 500", async _ =>
            {
                await readQuery1.ToListAsync();
                return Response.Ok();
            });

            var step2 = Step.CreatePull("read Age > 50 and TOP 100", async _ =>
            {
                await readQuery2.ToListAsync();
                return Response.Ok();
            });

            return ScenarioBuilder.CreateScenario("test mongo", step1, step2)
                                  .WithConcurrentCopies(40)
                                  .WithDuration(TimeSpan.FromSeconds(10));
        }

        static void Main(string[] args)
        {
            var scenario = BuildScenario();
            NBomberRunner.RegisterScenarios(scenario)
                         .RunInConsole();
        }
    }
}
