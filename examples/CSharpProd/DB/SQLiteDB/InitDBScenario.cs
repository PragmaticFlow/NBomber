using Bogus;
using NBomber.Contracts;
using NBomber.CSharp;
using Microsoft.Extensions.Configuration;
using System.Data.SQLite;
using Dapper.Contrib.Extensions;

namespace CSharpProd.DB.SQLiteDB
{
    public class SQLiteDBCustomSettings
    {
        public int UserCount { get; set; }
        public int InsertBulkSize { get; set; }
        public string ConnectionString { get; set; }    
    }

    internal class InitDBScenario
    {
        private SQLiteCommand _command = null;

        public SQLiteConnection Connection { get; set; }
        public SQLiteDBCustomSettings DBSettings { get; set; }
      
        public ScenarioProps Create()
        {
            return Scenario
                .Empty("initDB")
                .WithInit(context =>
                {
                    DBSettings = context.CustomSettings.Get<SQLiteDBCustomSettings>();

                    Connection = new SQLiteConnection(DBSettings.ConnectionString);
                    Connection.Open();
                    _command = new SQLiteCommand(Connection);
                    _command.CommandText = "PRAGMA journal_mode=WAL";
                    _command.ExecuteNonQuery();

                    _command.CommandText = "pragma synchronous = normar";
                    _command.ExecuteNonQuery();

                    CreateTable(_command);

                    var queryString = "SELECT MAX(Id) FROM users";
                    using var commandSelect = new SQLiteCommand(queryString, Connection);
                    var result = commandSelect.ExecuteScalar();
                    var maxId = 0;
                    if (!(result is DBNull))
                        maxId = Convert.ToInt32(result);

                    if (DBSettings.UserCount > maxId)
                    {
                        var listOfUser = new List<User>();
                        var usersToInsertCount = DBSettings.UserCount - maxId;
                        var createdDateTime = DateTime.UtcNow;

                        for (int i = 1; i <= usersToInsertCount; i++)
                        {
                            var fakeUser = new Faker<User>()
                                .RuleFor(u => u.Id, f => maxId == 0 ? i : maxId + i)
                                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                                .RuleFor(u => u.LastName, f => f.Person.LastName)
                                .RuleFor(u => u.Age, f => f.Random.Number(10, 90))
                                .RuleFor(u => u.Birthday, (f, u) => f.Date.Past(u.Age, DateTime.UtcNow))
                                .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                                .RuleFor(u => u.Country, f => f.Address.Country())
                                .RuleFor(u => u.State, f => f.Address.State())
                                .RuleFor(u => u.City, f => f.Address.City())
                                .RuleFor(u => u.StreetAddress, f => f.Address.StreetAddress())
                                .RuleFor(u => u.BuildingNumber, f => f.Address.BuildingNumber())
                                .RuleFor(u => u.ZipCode, f => f.Address.ZipCode())
                                .RuleFor(u => u.SecondaryAddress, f => f.Address.SecondaryAddress())
                                .RuleFor(u => u.Company, f => f.Company.CompanyName())
                                .RuleFor(u => u.CompanySuffix, f => f.Company.CompanySuffix())
                                .RuleFor(u => u.Department, f => f.Commerce.Department())
                                .RuleFor(u => u.JobTitle, f => f.Name.JobTitle())
                                .RuleFor(u => u.JobDescriptor, f => f.Name.JobDescriptor())
                                .RuleFor(u => u.JobArea, f => f.Name.JobArea())
                                .RuleFor(u => u.AccountName, f => f.Finance.AccountName())
                                .RuleFor(u => u.Account, f => f.Finance.Account(12))
                                .RuleFor(u => u.CreditCardNumber, f => f.Finance.CreditCardNumber())
                                .RuleFor(u => u.VehicleModel, f => f.Vehicle.Model())
                                .RuleFor(u => u.VehicleType, f => f.Vehicle.Type())
                                .RuleFor(u => u.Vin, f => f.Vehicle.Vin())
                                .RuleFor(u => u.PetsNumber, f => f.Random.Number(0, 4))
                                .RuleFor(u => u.Created, f => createdDateTime)
                                .RuleFor(u => u.Updated, f => createdDateTime);

                            listOfUser.Add(fakeUser);
                            if (i % DBSettings.InsertBulkSize == 0)
                            {
                                Connection.Insert(listOfUser);
                                listOfUser.Clear();
                            }
                        }
                        if (listOfUser.Count > 0)
                            Connection.Insert(listOfUser);
                    }
                    return Task.CompletedTask;
                }).WithClean(context =>
                {
                    _command.Dispose();
                    Connection.Dispose();
                    return Task.CompletedTask;
                }); ;
        }

        internal void CreateTable(SQLiteCommand command)
        { 
            command.CommandText = @"CREATE TABLE IF NOT EXISTS  users 
                    (Id INTEGER PRIMARY KEY,
                    FirstName TEXT, 
                    LastName TEXT,
                    Age INTEGER,
                    Birthday DATETAME,
                    Gender INTEGER,
                    Email TEXT,
                    PhoneNumber TEXT,
                    Country TEXT,
                    State TEXT,
                    City TEXT,
                    StreetAddress TEXT,
                    BuildingNumber TEXT,
                    ZipCode TEXT,
                    SecondaryAddress TEXT,
                    Company TEXT,
                    CompanySuffix TEXT,
                    Department TEXT,
                    JobTitle TEXT,
                    JobDescriptor TEXT,
                    JobArea TEXT,
                    AccountName TEXT,
                    Account TEXT,
                    CreditCardNumber TEXT,
                    VehicleModel TEXT,
                    VehicleType TEXT,
                    Vin TEXT,
                    PetsNumber INTEGER,
                    Created DATETIME,
                    Updated Datetime)";
            command.ExecuteNonQuery();
        }   
    }
}
