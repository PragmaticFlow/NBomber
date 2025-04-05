using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Demo.HTTP.SimpleBookstore.Contracts;
using System.Net.Http.Json;

namespace Demo.HTTP.SimpleBookstore
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

                        var request = Http.CreateRequest("POST", "http://localhost:5223/api/users/login")
                            .WithHeader("Accept", "application/json")
                            .WithJsonBody(rundomUser);

                        var response = await Http.Send(_httpClient, request);

                        var jwt = ExtractJwt(response.Payload.Value);
                        return Response.Ok(payload: jwt, sizeBytes: response.SizeBytes);
                    });

                    var jwt = login.Payload.Value;

                    var getAvailableBook = await Step.Run("getAvailableBook", context, async () =>
                    {
                        var request = Http.CreateRequest("GET", "http://localhost:5223/api/books?availableOnly=false")
                            .WithHeader("Accept", "application/json")
                            .WithHeader("Authorization", $"Bearer {jwt}");

                        var response = await Http.Send<BookListResponse>(_httpClient, request);

                        var booksList = response.Payload.Value.Data;

                        return Response.Ok(payload: booksList.Data, sizeBytes: response.SizeBytes);
                    });

                    var books = getAvailableBook.Payload.Value;

                    var createOrder = await Step.Run("createOrder", context, async () =>
                    {
                        var rundomBook = books[random.Next(books.Count - 1)];

                        var order = new Order
                        {
                            BookId = rundomBook.BookId,
                            Quantaty = 1
                        };

                        var request = Http.CreateRequest("POST", "http://localhost:5223/api/orders")
                            .WithHeader("Accept", "application/json")
                            .WithHeader("Authorization", $"Bearer {jwt}")
                            .WithJsonBody(order);

                        var response = await Http.Send(_httpClient, request);

                        return response;
                    });

                    var logout = await Step.Run("logout", context, async () =>
                    {
                        var request = Http.CreateRequest("POST", "http://localhost:5223/api/users/logout")
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
