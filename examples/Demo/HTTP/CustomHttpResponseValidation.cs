using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace Demo.HTTP;

public class CreatedUser
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("job")]
    public string Job { get; set; }
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

public class CustomHttpResponseValidation
{
    public void Run()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("custom_http_response_validation_scenario", async ctx =>
        {
            var failOk = await Step.Run("success_when_fail", ctx, async () =>
            {
                var request = Http.CreateRequest("GET", "https://reqres.in/api/users/23");

                var response = await Http.Send(httpClient, request);

                if (response.StatusCode == HttpStatusCode.NotFound.ToString())
                    return Response.Ok();

                return Response.Fail();
            });

            var successFail = await Step.Run("fail_when_success", ctx, async () =>
            {
                var request = Http.CreateRequest("POST", "https://reqres.in/api/users")
                    .WithBody(new StringContent("""{ "name": "morpheus", "job": "leader" }""", Encoding.UTF8, "application/json"));

                var response = await Http.Send<CreatedUser>(httpClient, request);

                if (response.StatusCode == HttpStatusCode.Created.ToString())
                {
                    var responseData = response.Payload.Value.Data;

                    if (responseData.Name == "morpheus")
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
