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
                // Each scenario copy (virtual user) creates its own HttpClient once and keeps it in ScenarioInstanceData.
                // Store it right after it's created: if the login step fails, NBomber ends the iteration,
                // and a client that isn't stored yet would be lost and created again on the next iteration.
                if (!context.ScenarioInstanceData.TryGetValue("http_client", out var client))
                {
                    client = Http.CreateDefaultClient();
                    context.ScenarioInstanceData["http_client"] = client;
                }
                var httpClient = (HttpClient)client;

                // The login state has its own key, so a failed login is repeated on the next iteration with the same client.
                if (!context.ScenarioInstanceData.ContainsKey("logged_in"))
                {
                    await Step.Run("login", context, async () =>
                    {
                        // WebAppSimulator address
                        var request = Http.CreateRequest("POST", "https://localhost:65385/api/CookiesAuthentication")
                            .WithJsonBody(new { Login = "morpheus", Password = "leader" });

                        // the session cookies are stored in this HttpClient
                        return await httpClient.SendRequest(request);
                    });

                    context.ScenarioInstanceData["logged_in"] = true;
                }

                await Step.Run("get_data", context, async () =>
                {
                    var request = Http.CreateRequest("GET", "https://localhost:65385/api/CookiesAuthentication");

                    // sends the cookies of this user session only
                    return await httpClient.SendRequest(request);
                });
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(3))
            .WithLoadSimulations(Simulation.KeepConstant(copies: 10, during: TimeSpan.FromSeconds(30)));

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}
