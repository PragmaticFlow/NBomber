using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using Newtonsoft.Json;
using System.Text;
using NBomber.Http.CSharp;
using System.Net.Http.Json;
using CSharpProd.HTTP.SimpleBookstore.Contracts;

namespace CSharpProd.HTTP.SimpleBookstore
{
    public class TestSimpleBookstoreScenario
    {
        private GlobalCustomSettings _settings { get; set; }
        private HttpClient _httpClient = new HttpClient();
        public ScenarioProps Create()
        {
            var userLogins = InitSimpleBookstoreScenario.UserLogins;
            var random = new Random();

            return Scenario
                .Create("test_bookstore", async context =>
                {
                    var rundom = random.Next(_settings.RecordsCount - 1);

                    var login = await Step.Run("login", context, async () =>
                    {
                        var rundomUser = userLogins[rundom];
                        var data = JsonConvert.SerializeObject(rundomUser);

                        var request = Http.CreateRequest("POST", "http://localhost:5064/api/users/login")
                            .WithHeader("Accept", "application/json")
                            .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));

                        var response = await Http.Send(_httpClient, request);

                        if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
                        {
                            var jwt = ExtractJwt(response.Payload.Value);
                            return Response.Ok(payload: jwt, sizeBytes: response.SizeBytes);
                        }
                        else
                            return Response.Fail<string>();
                    });

                    var jwt = login.Payload.Value;

                    var getAvailableBook = await Step.Run("getAvailableBook", context, async () =>
                    {
                        var request = Http.CreateRequest("GET", "http://localhost:5064/api/books?availableOnly=true")
                            .WithHeader("Accept", "application/json")
                            .WithHeader("Authorization", $"Bearer {jwt}");

                        var response = await Http.Send(_httpClient, request);

                        if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
                        {
                            var books = response.Payload.Value.Content;
                            var booksList = books.ReadFromJsonAsync<HttpResponse<List<BookResponse>>>().Result.Data;

                            return Response.Ok(payload: booksList, sizeBytes: response.SizeBytes);
                        }
                        else
                            return Response.Fail<List<BookResponse>>();
                    });

                    var books = getAvailableBook.Payload.Value;

                    var createOrder = await Step.Run("createOrder", context, async () =>
                    {
                        var rundomBook = books[random.Next(books.Count)];

                        var order = new Order
                        {
                            BookId = rundomBook.BookId,
                            Quantaty = 1
                        };
                        var data = JsonConvert.SerializeObject(order);
                        var request = Http.CreateRequest("POST", "http://localhost:5064/api/orders")
                            .WithHeader("Accept", "application/json")
                            .WithHeader("Authorization", $"Bearer {jwt}")
                            .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));

                        var response = await Http.Send(_httpClient, request);
                        return response;
                    });

                    var logout = await Step.Run("logout", context, async () =>
                    {
                        var request = Http.CreateRequest("POST", "http://localhost:5064/api/users/logout")
                            .WithHeader("Accept", "application/json")
                            .WithHeader("Authorization", $"Bearer {jwt}");

                        var response = await Http.Send(_httpClient, request);

                        return response;
                    });

                    return Response.Ok();
                })
                .WithInit(context =>
                {
                    _settings = context.GlobalCustomSettings.Get<GlobalCustomSettings>();
                    return Task.CompletedTask;
                });
        }

        private string ExtractJwt(HttpResponseMessage response)
        {
            return response.Content.ReadFromJsonAsync<JwtResponse>().Result.Data;
        }
    }
}
