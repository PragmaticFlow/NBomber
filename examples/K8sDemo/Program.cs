using NBomber.CSharp;
using NBomber.Http.CSharp;

var httpClient = Http.CreateDefaultClient();

var scenario = Scenario.Create("my_scenario", async context =>
{
    var request = Http.CreateRequest("GET", "https://catfact.ninja/facts");

    var response = await Http.Send(httpClient, request);

    return response;
});

NBomberRunner
    .RegisterScenarios(scenario)
    .Run(args);
