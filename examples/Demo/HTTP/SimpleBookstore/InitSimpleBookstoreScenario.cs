using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using Bogus;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
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
                  var request = Http.CreateRequest("PUT", "http://localhost:5223/api/databases")
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
                   
                          var data = JsonSerializer.Serialize(user);
                          var request = Http.CreateRequest("POST", "http://localhost:5223/api/users/singup")
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
                          var data = JsonSerializer.Serialize(book);
                          var request = Http.CreateRequest("POST", "http://localhost:5223/api/books")
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

