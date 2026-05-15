using NBomber.CSharp;
using NBomber.Http.CSharp;
using NBomber.Sinks.Timescale;

var httpClient = Http.CreateDefaultClient();

var scenario = Scenario.Create("my_scenario", async context =>
{
    var request = Http.CreateRequest("GET", "https://artifacthub.io/");

    var response = await Http.Send(httpClient, request);

    return response;
});

NBomberRunner
    .RegisterScenarios(scenario)
    .WithReportingSinks(new TimescaleDbSink())
    .Run(args);
