namespace Demo.WebBrowsers.Playwright;

using Microsoft.Playwright;
using NBomber.CSharp;
using NBomber.WebBrowser.Playwright;

public class PlaywrightExample
{
    public async Task Run()
    {
        var browserPath = "C:/Program Files/Google/Chrome/Application/chrome.exe";

        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions { Headless = true, ExecutablePath = browserPath }
        );

        var scenario = Scenario.Create("playwright_scenario", async context =>
        {
            var page = await browser.NewPageAsync();
            var pageResponse = await page.GotoAsync("https://translate.google.com/");

            var response = await pageResponse.ToNBomberResponse();
            page.CloseAsync();

            return response;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(1, TimeSpan.FromMinutes(1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
