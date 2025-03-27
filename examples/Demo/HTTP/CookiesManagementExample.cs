using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Text;

namespace Demo.HTTP
{
    class CookiesManagementExample
    {
        public void Run()
        {
            var scenario = Scenario.Create("cookies_management_scenario", async context =>
            {
                context.ScenarioInstanceData.TryGetValue("my_http_client", out var httpClient);

                if (httpClient is null)
                {
                    httpClient = new HttpClient();

                    var login = await Step.Run("login", context, async () =>
                    {
                        // WebAppSimulator address
                        var request = Http.CreateRequest("POST", "https://localhost:65385/api/CookiesAuthentication/Login")
                            .WithBody(new StringContent("""{"login": "morpheus","password": "leader"}""", Encoding.UTF8, "application/json"));

                        var response = await Http.Send((HttpClient)httpClient, request);

                        return response;
                    });

                    context.ScenarioInstanceData["my_http_client"] = httpClient;
                }

                var getData = await Step.Run("get_data", context, async () =>
                {
                    var request = Http.CreateRequest("GET", "https://localhost:65385/api/CookiesAuthentication/GetData");

                    var response = await Http.Send((HttpClient)httpClient, request);

                    return response;
                });

                return Response.Ok();
            })
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30)));

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}
