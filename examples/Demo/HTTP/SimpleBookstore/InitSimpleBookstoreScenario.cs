using Bogus;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;
using NBomber.Http.CSharp;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Demo.HTTP.SimpleBookstore.Contracts;

namespace Demo.HTTP.SimpleBookstore
{
    public class GlobalCustomSettings
    {
        public int RecordsCount { get; set; }
        public int BooksCount { get; set; }
    }

    public class InitSimpleBookstoreScenario
    {
        private HttpClient _httpClient = new HttpClient();
        public static List<UserLogin> UserLogins = new List<UserLogin>();
        public ScenarioProps Create()
        {
            return Scenario
              .Empty("init_bookstore_db")
              .WithInit(async context =>
              {
                  // recreate DB
                  var request = Http.CreateRequest("PUT", "http://localhost:5064/api/databases")
                                .WithHeader("Accept", "application/json");

                  var response = await Http.Send(_httpClient, request);

                  var settings = context.GlobalCustomSettings.Get<GlobalCustomSettings>();

                  var faker = new Faker();

                  var usersSingup = Enumerable
                      .Range(0, settings.RecordsCount)
                      .Select(i => new UserSingup
                      {
                          FirstName = faker.Name.FirstName(),
                          LastName = faker.Name.LastName(),
                          Email = faker.Internet.Email(),
                          Password = PasswordGenerator.GeneratePassword(15),
                      })
                      .Select(user =>
                      {
                          UserLogins.Add(new UserLogin
                          {
                              Email = user.Email,
                              Password = user.Password,
                          });
                   
                          var data = JsonConvert.SerializeObject(user);
                          var request = Http.CreateRequest("POST", "http://localhost:5064/api/users/singup")
                              .WithHeader("Accept", "application/json")
                              .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));

                          return Http.Send(_httpClient, request);
                      });

                  var bookInsert = Enumerable
                      .Range(0, settings.BooksCount)
                      .Select(i => new Book
                      {
                          Title = faker.Lorem.Sentence(),
                          Author = faker.Name.FullName(),
                          PublicationDate = faker.Date.Past(200, DateTime.UtcNow),
                          Quantaty = faker.Random.Number(10, 50)
                      })
                      .Select(book =>
                      {
                          var data = JsonConvert.SerializeObject(book);
                          var request = Http.CreateRequest("POST", "http://localhost:5064/api/books")
                                  .WithHeader("Accept", "application/json")
                                  .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));

                          return Http.Send(_httpClient, request);
                      });

                  var allTasks = bookInsert.Concat(usersSingup);
                  await Task.WhenAll(allTasks);
              });
        }
    }
}

