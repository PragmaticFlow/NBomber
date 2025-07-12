using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace Demo.HTTP.WebAppSimulator
{
    public class ReadHttpScenario
    {
        public ScenarioProps Create()
        {
            GlobalCustomSettings settings = null;
            using var httpClient = Http.CreateDefaultClient();
            var _random = new Random();

            return Scenario.Create("read_http_db", async context =>
            {
                var randomId = _random.Next(settings.RecordsCount);
                var request = Http.CreateRequest("GET", settings.ServerUrl + $"/api/users/{randomId}");

                var response = await Http.Send(httpClient, request);
                return response;
            })
            .WithInit(context =>
            {
                settings = context.GlobalCustomSettings.Get<GlobalCustomSettings>();
                return Task.CompletedTask;
            });
        }
    }
}
