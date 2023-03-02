using System.Data.SQLite;
using Bogus;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using NBomber;
using NBomber.CSharp;

namespace CSharpProd.DB.SQLiteDB
{
    public class SQLiteDBExample
    {
        public void Run()
        {
            var initDBScn = new InitDBScenario();

            var getByIdConnectionPool = new ClientPool<SQLiteConnection>();
            var getById = Scenario.Create("get_by_id", async context =>
            {
                var randomId = new Random().Next(1, initDBScn.DBSettings.UserCount);
                var con = getByIdConnectionPool.GetClient(context.ScenarioInfo);
                await con.GetAsync<User>(randomId);

                return Response.Ok();
            })
            .WithInit(context =>
            {
                var dbSetings = context.CustomSettings.Get<SQLiteDBCustomSettingScenario>();
                FillConnectionPool(dbSetings, getByIdConnectionPool);
                return Task.CompletedTask;
            })
            .WithClean(context =>
            {
                getByIdConnectionPool.DisposeClients();
                return Task.CompletedTask;
            });

            var updetedTimeinFormat = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            var updatePool = new ClientPool<SQLiteConnection>();
            var update = Scenario.Create("update", async context =>
            {
                var randomId = new Random().Next(1, initDBScn.DBSettings.UserCount);
                var stringQuery = $"UPDATE users SET Age = Age+1, Updated = '{updetedTimeinFormat}' WHERE Id={randomId}";
                using var cmd = new SQLiteCommand(stringQuery, updatePool.GetClient(context.ScenarioInfo));
                var result = await cmd.ExecuteNonQueryAsync();

                return Response.Ok();
            })
            .WithInit(context =>
            {
                var dbSetings = context.CustomSettings.Get<SQLiteDBCustomSettingScenario>();
                FillConnectionPool(dbSetings, updatePool);
                return Task.CompletedTask;
            })
            .WithClean(context =>
            {
                updatePool.DisposeClients();
                return Task.CompletedTask;
            });

            var updetedTime = DateTime.UtcNow;
            var readModifyWritePool = new ClientPool<SQLiteConnection>();
            var readModifyWrite = Scenario.Create("read_modify_write", async context =>
            {
                var randomId = new Random().Next(1, initDBScn.DBSettings.UserCount);
                var connection = readModifyWritePool.GetClient(context.ScenarioInfo);
                var rundomUser = await connection.GetAsync<User>(randomId);

                rundomUser.Updated = updetedTime;
                await connection.UpdateAsync<User>(rundomUser);

                return Response.Ok();
            })
            .WithInit(context =>
            {
                var dbSetings = context.CustomSettings.Get<SQLiteDBCustomSettingScenario>();
                FillConnectionPool(dbSetings, readModifyWritePool);
                return Task.CompletedTask;
            })
            .WithClean(context =>
            {
                readModifyWritePool.DisposeClients();
                return Task.CompletedTask;
            });

            var conditionalQueryPool = new ClientPool<SQLiteConnection>();
            var faker = new Faker();
            var conditionalQuery = Scenario.Create("conditional_query", async context =>
            {
                var connection = conditionalQueryPool.GetClient(context.ScenarioInfo);
                var stringQuery = $"SELECT * FROM users WHERE City = '{faker.Address.City().Replace("'", "''")}' ORDER BY Id LIMIT 10";
                using var cmd = new SQLiteCommand(stringQuery, connection);
                var result = await cmd.ExecuteReaderAsync();

                return Response.Ok();
            })
            .WithInit(context =>
            {
                var dbSetings = context.CustomSettings.Get<SQLiteDBCustomSettingScenario>();
                FillConnectionPool(dbSetings, conditionalQueryPool);
                return Task.CompletedTask;
            })
            .WithClean(context =>
            {
                conditionalQueryPool.DisposeClients();
                return Task.CompletedTask;
            });

            NBomberRunner
               .RegisterScenarios(initDBScn.Create(), getById, update, readModifyWrite, conditionalQuery)
               .LoadConfig("DB/SQLiteDB/config.json")
               .Run();
        }

        static void FillConnectionPool(SQLiteDBCustomSettingScenario dbSetings, ClientPool<SQLiteConnection> pool)
        {
            for (var i = 0; i < dbSetings.ConnectionCount; i++)
            {
                var connection = new SQLiteConnection(dbSetings.ConnectionString);
                connection.Open();
                pool.AddClient(connection);
            }
        }
    }

    internal class SQLiteDBCustomSettingScenario
    {
        public string ConnectionString { get; set; }
        public int ConnectionCount { get; set; }
    }
}
