namespace Demo.WebBrowsers.Playwright;

using PuppeteerSharp;
using Microsoft.Playwright;
using NBomber.CSharp;
using NBomber.WebBrowser.Playwright;

public class PlaywrightExample
{
    public async Task Run()
    {
        // downloading the Chrome
        var installedBrowser = await new BrowserFetcher(SupportedBrowser.Chrome).DownloadAsync(BrowserTag.Stable);
        var browserPath = installedBrowser.GetExecutablePath();

        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = true,
                ExecutablePath = browserPath
            }
        );

        var scenario = Scenario.Create("playwright_scenario", async context =>
        {
            var page = await browser.NewPageAsync();

            await Step.Run("open nbomber", context, async () =>
            {
                var pageResponse = await page.GotoAsync("https://nbomber.com/");

                var html = await page.ContentAsync();
                var totalSize = await page.GetDataTransferSize();

                return Response.Ok(sizeBytes: totalSize);
            });

            await Step.Run("open bing", context, async () =>
            {
                var pageResponse = await page.GotoAsync("https://www.bing.com/maps");

                await page.WaitForSelectorAsync(".searchbox input");
                await page.FocusAsync(".searchbox input");
                await page.Keyboard.TypeAsync("CN Tower, Toronto, Ontario, Canada");

                await page.Keyboard.PressAsync("Enter");
                await page.WaitForLoadStateAsync(LoadState.Load);

                var totalSize = await page.GetDataTransferSize();
                return Response.Ok(sizeBytes: totalSize);
            });

            await page.CloseAsync();

            return Response.Ok();
        })
        .WithWarmUpDuration(TimeSpan.FromSeconds(3))
        .WithLoadSimulations(
            Simulation.KeepConstant(1, TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
