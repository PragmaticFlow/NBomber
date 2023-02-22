using System.Net;
using Cassandra;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;

namespace CSharpProd.DB.ScyllaDB;

public class ScyllaDBInitSettings
{
    public string ConnectionString { get; set; }
    public int UserCount { get; set; }
    public int UserRecordSizeBytes { get; set; }
    public int InsertBulkSize { get; set; }
    public string KeySpace { get; set; }
    public int ReplicationFactor { get; set; }
    public bool ShouldPopulateDB { get; set; }
}

public class ScyllaInitDBScenario
{
    public ScyllaDBInitSettings DBSettings { get; private set; }
    public ISession Session { get; private set; }
    public PreparedStatement InsertQuery { get; private set; }
    public PreparedStatement GetByIdQuery { get; private set; }
    public byte[] UserRecord { get; set; }

    public ScenarioProps Create()
    {
        return Scenario
            .Empty("initDB")
            .WithInit(context =>
            {
                DBSettings = context.CustomSettings.Get<ScyllaDBInitSettings>();
                
                // var ip = new IPEndPoint(IPAddress.Parse("206.189.62.89"), 9042);

                var hosts = DBSettings.ConnectionString.Split(",");
                
                var cluster = Cluster.Builder()
                    .AddContactPoints(hosts)
                    .WithCompression(CompressionType.NoCompression)
                    .Build();
                
                Session = cluster.Connect();

                Session.CreateKeyspaceIfNotExists(DBSettings.KeySpace, new Dictionary<string, string>
                {
                    { "class", ReplicationStrategies.SimpleStrategy }, { "replication_factor", DBSettings.ReplicationFactor.ToString() }
                });
                
                Session.ChangeKeyspace(DBSettings.KeySpace);
                
                //PopulateDB();
                
                if (DBSettings.ShouldPopulateDB)
                    PopulateDB();
                else
                    PrepareQueries();

                return Task.CompletedTask;
            });
    }

    void PrepareQueries()
    {
        InsertQuery = Session.Prepare("INSERT INTO users (id, data) VALUES (?, ?)");
        GetByIdQuery = Session.Prepare("SELECT id, data FROM users WHERE id=?");
        UserRecord = Data.GenerateRandomBytes(DBSettings.UserRecordSizeBytes);
    }
    
    void PopulateDB()
    {
        Session.Execute(new SimpleStatement(
            "DROP TABLE IF EXISTS users"
        ));
                
        Session.Execute(new SimpleStatement(
                // "CREATE TABLE users (id text PRIMARY KEY, data blob) WITH COMPACT STORAGE")
            "CREATE TABLE users (id text PRIMARY KEY, data blob) WITH COMPACT STORAGE AND compression = {}")
        );

        PrepareQueries();
        
        var userId = 0;
        while (userId < DBSettings.UserCount)
        {
            var batch = new BatchStatement();
            for (int i = 0; i < DBSettings.InsertBulkSize; i++)
            {
                batch.Add(InsertQuery.Bind(userId.ToString(), UserRecord));
                userId++;
            }

            Session.Execute(batch);
        }
    }
}