using Microsoft.Extensions.Configuration;
using Bogus;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Demo.HTTP.SimpleBookstore.Contracts;

namespace Demo.HTTP.SimpleBookstore
{
    public class GlobalCustomSettings
    {
        public string ServerUrl { get; set; }
        public int ClientsCount { get; set; }
        public int BooksCount { get; set; }
    }

    public class InitSimpleBookstoreScenario
    {
        private HttpClient _httpClient = Http.CreateDefaultClient();
        public static List<UserLogin> UserLogins = new List<UserLogin>();
        public ScenarioProps Create()
        {
            return Scenario
              .Empty("init_bookstore_db")
              .WithInit(async context =>
              {
                  var settings = context.GlobalCustomSettings.Get<GlobalCustomSettings>();

                  // recreate DB
                  var request = Http.CreateRequest("PUT", settings.ServerUrl + "/api/databases")
                    .WithHeader("Accept", "application/json");

                  var response = await Http.Send(_httpClient, request);

                  var faker = new Faker();

                  var usersSingup = Enumerable
                      .Range(0, settings.ClientsCount)
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

                          var request = Http.CreateRequest("POST", settings.ServerUrl + "/api/users/singup")
                              .WithHeader("Accept", "application/json")
                              .WithJsonBody(user);

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
                          var request = Http.CreateRequest("POST", settings.ServerUrl + "/api/books")
                                  .WithHeader("Accept", "application/json")
                                  .WithJsonBody(book);

                          return Http.Send(_httpClient, request);
                      });

                  var allTasks = bookInsert.Concat(usersSingup);
                  await Task.WhenAll(allTasks);
              });
        }
    }
}

