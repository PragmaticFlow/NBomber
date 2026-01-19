using System.Text;
using Bogus;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Newtonsoft.Json;

namespace Demo.HTTP.HttpApiSimulator;

public class GlobalCustomSettings
{
    public string ServerUrl { get; set; }
    public int RecordsCount { get; set; }
}

public class InitHttpScenario
{
    public ScenarioProps Create()
    {
        var httpClient = Http.CreateDefaultClient();

        return Scenario
            .Empty("init_http_db")
            .WithInit(async context =>
            {
                var settings = context.GlobalCustomSettings.Get<GlobalCustomSettings>();

                // recreate DB
                var request = Http.CreateRequest("PUT", settings.ServerUrl + "/api/databases");
                var response = await Http.Send(httpClient, request);

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
                        var request = Http.CreateRequest("POST", settings.ServerUrl + "/api/users")
                            .WithHeader("Accept", "application/json")
                            .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));

                        return Http.Send(httpClient, request);
                    });

                await Task.WhenAll(responses);
            });
    }
}
