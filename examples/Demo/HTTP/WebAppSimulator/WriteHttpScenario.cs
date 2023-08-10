using Bogus;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using Newtonsoft.Json;
using NBomber.Http.CSharp;
using System.Text;

namespace Demo.HTTP.WebAppSimulator
{
    public class WriteHttpScenario
    {
        private GlobalCustomSettings _settings;
        private readonly Random _random = new Random();
        private Faker _faker = new Faker();
        private HttpClient _httpClient = new HttpClient();
        public ScenarioProps Create()
        {
            return Scenario
            .Create("write_http_db", async context =>
            {
                var randomId = _random.Next(_settings.RecordsCount);

                var user = new User()
                {
                    Id = randomId,
                    FirstName = _faker.Name.FirstName(),
                    LastName = _faker.Name.LastName(),
                    Age = _faker.Random.Int(1, 100),
                };

                var data = JsonConvert.SerializeObject(user);
                var request = Http.CreateRequest("PUT", $"http://localhost:5195/api/users/{randomId}")
                     .WithHeader("Content-Type", "application/json")
                     .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));

                var response = await Http.Send(_httpClient, request);
                return response;
            })
            .WithInit(context =>
            {
                _settings = context.GlobalCustomSettings.Get<GlobalCustomSettings>();
                return Task.CompletedTask;
            });
        }
    }
}
