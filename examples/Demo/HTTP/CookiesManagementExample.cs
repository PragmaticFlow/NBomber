using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Text;

namespace Demo.HTTP
{
    class CookiesManagementExample
    {
        public void Run()
        {
            // For this example, you'll need to start the HttpApiSimulator, which is located in the examples/simulators solution folder.
            // Make sure it’s running before executing the client tests to ensure proper communication.

            var scenario = Scenario.Create("cookies_management_scenario", async context =>
            {
                HttpClient myClient = null;
                context.ScenarioInstanceData.TryGetValue("my_http_client", out var httpClient);

                if (httpClient is null)
                {
                    myClient = Http.CreateDefaultClient();

                    var login = await Step.Run("login", context, async () =>
                    {
                        // WebAppSimulator address
                        var request = Http.CreateRequest("POST", "https://localhost:65385/api/CookiesAuthentication")
                            .WithJsonBody(new StringContent("""{"login": "morpheus","password": "leader"}"""));

                        var response = await Http.Send(myClient, request);

                        return response;
                    });

                    context.ScenarioInstanceData["my_http_client"] = myClient;
                }
                else
                    myClient = (HttpClient)httpClient;

                var getData = await Step.Run("get_data", context, async () =>
                {
                    var request = Http.CreateRequest("GET", "https://localhost:65385/api/CookiesAuthentication");

                    var response = await Http.Send(myClient, request);

                    return response;
                });

                return Response.Ok();
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(3))
            .WithLoadSimulations(Simulation.KeepConstant(copies: 10, during: TimeSpan.FromSeconds(30)));

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}
