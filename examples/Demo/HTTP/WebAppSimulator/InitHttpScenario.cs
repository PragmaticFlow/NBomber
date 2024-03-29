using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Bogus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Demo.HTTP.WebAppSimulator
{
    public class GlobalCustomSettings
    {
        public int RecordsCount { get; set; }
    }

    public class InitHttpScenario
    {
        private HttpClient _httpClient = new HttpClient();
        public ScenarioProps Create()
        {
            return Scenario
                .Empty("init_http_db")
                .WithInit(async context =>
                {
                    // recreate DB
                    var request = Http.CreateRequest("PUT", "http://localhost:5195/api/databases");
                    var response = await Http.Send(_httpClient, request);

                    var settings = context.GlobalCustomSettings.Get<GlobalCustomSettings>();

                    var faker = new Faker();

                    var responses = Enumerable
                        .Range(0, settings.RecordsCount)
                        .Select(i => new User
                        {
                            Id = i,
                            FirstName = faker.Name.FirstName(),
                            LastName = faker.Name.LastName(),
                            Age = faker.Random.Int(1, 100)
                        })
                        .Select(user =>
                        {
                            var data = JsonConvert.SerializeObject(user);
                            var request = Http.CreateRequest("POST", "http://localhost:5195/api/users")
                                .WithHeader("Accept", "application/json")                                
                                .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));
                            
                            return Http.Send(_httpClient, request);
                        });

                    await Task.WhenAll(responses);
                });
        }
    }
}
