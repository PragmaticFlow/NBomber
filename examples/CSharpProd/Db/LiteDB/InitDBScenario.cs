using Bogus;
using LiteDB;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLoadTest
{
    public class LiteDBCustomSettings
    {
        public int UserCount { get; set; }
        public int InsertBulcSize { get; set; }
    }
    internal class InitDBScenario
    {
        private LiteDatabase _db = null;

        public ILiteCollection<User> Collection { get; private set; }
        public int RecordBiteSize { get; private set; }
        public LiteDBCustomSettings DBSettings { get; private set;}

        public ScenarioProps Create()
        {
            return Scenario
                .Empty("initDB")
                .WithInit(context =>
                {
                    DBSettings = context.CustomSettings.Get<LiteDBCustomSettings>();

                    _db = new LiteDatabase("UsersRegister.db");

                    Collection = _db.GetCollection<User>("users");
                    var collectionCount = Collection.Count();

                    if (DBSettings.UserCount >= collectionCount)
                    {
                        var lastUser = Collection.Query().OrderByDescending(c => c._id).FirstOrDefault();
                        var maxId = 0;

                        if (lastUser != null)
                            maxId = lastUser._id;

                        var listOfUser = new List<User>();
                        var usersToInsertCount = DBSettings.UserCount - collectionCount;
                        var createdDateTime = DateTime.UtcNow;

                        for (int i = 1; i <= usersToInsertCount; i++)
                        {
                            var fakeUser = new Faker<User>()
                                 .RuleFor(u => u._id, f => maxId == 0 ? i : maxId + i)
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
                            if (i % DBSettings.InsertBulcSize == 0)
                            {
                                Collection.Insert(listOfUser);
                                listOfUser.Clear();
                            }
                        }
                        if (listOfUser.Count > 0)
                            Collection.Insert(listOfUser);
                    }
            
                    RecordBiteSize = CalculateRecordSize(Collection);
                
                    return Task.CompletedTask;
                })
                .WithClean(context =>
                {
                    _db.Dispose();
                    return Task.CompletedTask;
                });
        } 
        private int CalculateRecordSize(ILiteCollection<User> collection)
        {
            var randomUserForeSize = Collection.FindById(1);
            var bsonMapper = new BsonMapper();
            var doc = bsonMapper.ToDocument<User>(randomUserForeSize);
            
            return BsonSerializer.Serialize(doc).Length;
        }
    }
}
