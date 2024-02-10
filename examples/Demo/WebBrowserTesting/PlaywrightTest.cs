using Microsoft.Playwright;
using NBomber.CSharp;

namespace Demo.WebBrowserTesting;

public class PlaywrightTest
{
    public async Task Run()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new  BrowserTypeLaunchOptions {ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"});

        var scenario = Scenario.Create("http_scenario", async context =>
            {
                var page = await browser.NewPageAsync();
                await page.GotoAsync("https://nbomber.com");

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
