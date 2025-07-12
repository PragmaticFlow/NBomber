using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Demo.HTTP;

public class UserPage
{
    [JsonPropertyName("page")]
    public int Page { get; set; }
    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }
    [JsonPropertyName("total")]
    public int Total { get; set; }
    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }
    [JsonIgnore]
    public object Data { get; set; }
}

public class LoginResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}

public class ErrorLoginResponse
{
    [JsonPropertyName("error")]
    public string Error { get; set; }
}

public class CustomHttpResponseValidation
{
    public void Run()
    {
        using var httpClient = Http.CreateDefaultClient();

        var scenario = Scenario.Create("custom_http_response_validation_scenario", async ctx =>
        {
            var failOk = await Step.Run("success_when_fail", ctx, async () =>
            {
                var request = Http.CreateRequest("POST", "https://reqres.in/api/login")
                    .WithBody(new StringContent("""{ "email": "peter@klaven" }""", Encoding.UTF8, "application/json"));

                var response = await Http.Send<LoginResponse>(httpClient, request);

                if (response.StatusCode == HttpStatusCode.BadRequest.ToString())
                {
                    var originalHttpResponse = response.Payload.Value.Response;

                    var errorResponse = await originalHttpResponse.Content.ReadFromJsonAsync<ErrorLoginResponse>();

                    if (errorResponse.Error == "Missing password")
                        return Response.Ok();
                }

                return Response.Fail();
            });

            var successFail = await Step.Run("fail_when_success", ctx, async () =>
            {
                var request = Http.CreateRequest("GET", "https://reqres.in/api/users");

                var response = await Http.Send<UserPage>(httpClient, request);

                if (response.StatusCode == HttpStatusCode.OK.ToString())
                {
                    var responseData = response.Payload.Value.Data;

                    if (responseData.PerPage == 6)
                        return Response.Fail();
                }

                return Response.Ok();
            });

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(10)))
        .WithRestartIterationOnFail(shouldRestart: false);

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
