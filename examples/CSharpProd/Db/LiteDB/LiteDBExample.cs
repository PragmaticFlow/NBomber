using LiteDB;
using MyLoadTest;
using NBomber.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpProd.Db.LiteDB
{
    public class LiteDBExample
    {
        public void Run()
        {
            var initDBScn = new InitDBScenario();

            var getById = Scenario.Create("get_by_id", async context =>
            {
                var randomId = new Random().Next(1, initDBScn.DBSettings.UserCount);
                var randomUser = initDBScn.Collection.FindById(new BsonValue(randomId));
                return Response.Ok(sizeBytes: initDBScn.RecordSize);
            });
          
            var update = Scenario.Create("update", async context =>
            {
                var randomId = new Random().Next(1, initDBScn.DBSettings.UserCount);
                var num = initDBScn.Collection.UpdateMany("{Age:$.Age+1}", "_id = '{randomId}'");

                return Response.Ok(sizeBytes: initDBScn.RecordSize);
            });

            var readModifyWrite = Scenario.Create("read_modify_write", async context =>
            {
                var randomId = new Random().Next(1, initDBScn.DBSettings.UserCount);
                var randomUser = initDBScn.Collection.FindById(new BsonValue(randomId));

                randomUser.Age++;

                initDBScn.Collection.Upsert(randomUser);

                return Response.Ok(sizeBytes: initDBScn.RecordSize * 2);
            });
           
            NBomberRunner
                .RegisterScenarios(initDBScn.Create(), getById, update, readModifyWrite)
                .LoadConfig("Db/LiteDB/config.json")
                .Run();
        }
    }
}
