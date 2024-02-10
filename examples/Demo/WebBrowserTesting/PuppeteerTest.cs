using NBomber.CSharp;
using PuppeteerSharp;
using Response = NBomber.CSharp.Response;

namespace Demo.WebBrowserTesting;

public class PuppeteerTest
{
    public async Task Run()
    {
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            { Headless = true, ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe" });

        var scenario = Scenario.Create("http_scenario", async context =>
            {
                await using var page = await browser.NewPageAsync();
                await page.GoToAsync("https://nbomber.com");

                return Response.Ok();
            })
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.KeepConstant(50, TimeSpan.FromSeconds(60))
            );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
