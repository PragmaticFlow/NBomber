using System.Net;
using System.Text;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using NBomber.CSharp;
using NBomber.Data;

namespace CSharpProd.DB.ScyllaDB;

public class User
{
    public string Id { get; set; }
    public byte[] Data { get; set; }
}

public class ScyllaDBTests
{
    public void Run(string[] args)
    {
        var initDbScn = new ScyllaInitDBScenario();
        var random = new Random();

        var getById = Scenario.Create("get_by_id", async context =>
        {
            var randomId = random.Next(1, initDbScn.DBSettings.UserCount);
            var query = initDbScn.GetByIdQuery.Bind(randomId.ToString());
            var response = await initDbScn.Session.ExecuteAsync(query);
            
            // var response = await initDbScn.Session.ExecuteAsync(
            //     new SimpleStatement($"SELECT id, data FROM myspace.users WHERE id='{randomId}'")
            // );

            return response.Columns.Length > 0
                ? Response.Ok(initDbScn.DBSettings.UserRecordSizeBytes)
                : Response.Fail(statusCode: "not found");
        });
        
        var readModifyWrite = Scenario.Create("read_modify_write", async context =>
        {
            var randomId = random.Next(1, initDbScn.DBSettings.UserCount);
            var readQuery = initDbScn.GetByIdQuery.Bind(randomId.ToString());
            var response = await initDbScn.Session.ExecuteAsync(readQuery);

            var writeQuery = initDbScn.InsertQuery.Bind(randomId.ToString(), initDbScn.UserRecord);
            response = await initDbScn.Session.ExecuteAsync(writeQuery);

            return response.IsFullyFetched
                ? Response.Ok(initDbScn.DBSettings.UserRecordSizeBytes * 2)
                : Response.Fail(statusCode: "not found");
        });
        
        NBomberRunner
            .RegisterScenarios(initDbScn.Create(), getById, readModifyWrite)
            .LoadConfig("DB/ScyllaDB/config.json")
            .Run(args);

        // // var config = new MappingConfiguration().Define(
        // //     new Map<User>()
        // //         .KeyspaceName("myspace")
        // //         .TableName("users")
        // //         .PartitionKey(a => a.Id)
        // //         //.CompactStorage()
        // // );
        //
        // // var table = new Table<User>(session, config);
        // //table.CreateIfNotExists();
        //
        // session.ChangeKeyspace("myspace");
        // session.Execute(new SimpleStatement(
        //     "CREATE TABLE users (id text PRIMARY KEY, data blob) WITH COMPACT STORAGE AND compression = {}")
        // );
        //
        // var query = session.Prepare("INSERT INTO users (id, data) VALUES (?, ?)");
        // var record = Data.GenerateRandomBytes(800);
        //
        // var batch = new BatchStatement();
        // for (int i = 0; i < 100; i++)
        // {
        //     batch.Add(query.Bind(i.ToString(), record));
        // }
        //
        // // var statement = query.Bind("1", Encoding.UTF8.GetBytes("data"));
        //
        // var result = session.Execute(batch);
        //
        //
        //
        // // session.ExecuteAsync(new SimpleStatement(""))
        //
        // // User user = mapper.Single<User>("SELECT name, email FROM users WHERE id = ?", userId);
    }
}