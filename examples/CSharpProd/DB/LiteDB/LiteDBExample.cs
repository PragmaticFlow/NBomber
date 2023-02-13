using Bogus;
using LiteDB;
using NBomber.CSharp;

namespace CSharpProd.DB.LiteDB
{
    public class LiteDBExample
    {
        public void Run()
        {
            var initDBScn = new InitDBScenario();
            var random = new Random();

            var getById = Scenario.Create("get_by_id", async context =>
            {
                var randomId = random.Next(1, initDBScn.DBSettings.UserCount);
                var randomUser = initDBScn.Collection.FindById(new BsonValue(randomId));

                return Response.Ok(sizeBytes: initDBScn.RecordSizeBytes);
            });

            var update = Scenario.Create("update", async context =>
            {
                var randomId = random.Next(1, initDBScn.DBSettings.UserCount);

                var randomUser = initDBScn.Collection.FindById(new BsonValue(randomId));
                var num = initDBScn.Collection.UpdateMany("{Age:$.Age+1, Updated:NOW_UTC()}", $"_id = {randomId}");

                return Response.Ok(sizeBytes: initDBScn.RecordSizeBytes);
            });

            var readModifyWrite = Scenario.Create("read_modify_write", async context =>
            {
                var randomId = random.Next(1, initDBScn.DBSettings.UserCount);
                var randomUser = initDBScn.Collection.FindById(new BsonValue(randomId));

                randomUser.PetsCount++;
                randomUser.Updated = DateTime.UtcNow;

                initDBScn.Collection.Upsert(randomUser);

                return Response.Ok(sizeBytes: initDBScn.RecordSizeBytes * 2);
            });

            var faker = new Faker();
            var conditionalQuery = Scenario.Create("conditional_query", async context =>
            {
                var listOfRandomUser = initDBScn.Collection.Query()
                    .Where(x => x.City == faker.Address.City())
                    .OrderBy(x => x._id)
                    .Limit(10)
                    .ToList();

                return Response.Ok(sizeBytes: initDBScn.RecordSizeBytes * listOfRandomUser.Count);
            });
            
            var checkpointDB = Scenario.Create("checkpointDB", async context =>
            {
                await Task.Delay(5_000);
                initDBScn.Db.Checkpoint();
                return Response.Ok();
            });

            NBomberRunner
                .RegisterScenarios(initDBScn.Create(), checkpointDB, getById, update, readModifyWrite, conditionalQuery)
                .LoadConfig("Db/LiteDB/config.json")
                .Run();
        }
    }
}
