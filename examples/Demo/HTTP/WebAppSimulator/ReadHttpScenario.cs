using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Net.Http;

namespace Demo.HTTP.WebAppSimulator
{
    public class ReadHttpScenario
    {
        private GlobalCustomSettings _settings;
        private HttpClient _httpClient = new HttpClient();
        private readonly Random _random = new Random();
        public ScenarioProps Create()
        {
            return Scenario
            .Create("read_http_db", async context =>
            {
                var randomId = _random.Next(_settings.RecordsCount);
                var request = Http.CreateRequest("GET", $"http://localhost:5195/api/users/{randomId}");

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
