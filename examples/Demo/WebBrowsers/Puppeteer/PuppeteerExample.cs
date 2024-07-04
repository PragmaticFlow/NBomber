namespace Demo.WebBrowsers.Puppeteer;

using PuppeteerSharp;
using NBomber.CSharp;
using NBomber.WebBrowser.Puppeteer;

public class PuppeteerExample
{
    public async Task Run()
    {
        // downloading the Chrome
        var installedBrowser = await new BrowserFetcher(SupportedBrowser.Chrome).DownloadAsync(BrowserTag.Stable);
        var browserPath = installedBrowser.GetExecutablePath();

        await using var browser = await Puppeteer.LaunchAsync(
            new LaunchOptions
            {
                Headless = true,
                ExecutablePath = browserPath
            }
        );

        var scenario = Scenario.Create("puppeteer_scenario", async context =>
        {
            await using var page = await browser.NewPageAsync();
            await page.SetCacheEnabledAsync(false); // disable caching

            await Step.Run("open nbomber", context, async () =>
            {
                var pageResponse = await page.GoToAsync("https://nbomber.com/");

                var html = await page.GetContentAsync();
                var totalSize = await page.GetDataTransferSize();

                return Response.Ok(sizeBytes: totalSize);
            });

            await Step.Run("open bing", context, async () =>
            {
                var pageResponse = await page.GoToAsync("https://www.bing.com/maps");

                await page.WaitForSelectorAsync(".searchbox input");
                await page.FocusAsync(".searchbox input");
                await page.Keyboard.TypeAsync("CN Tower, Toronto, Ontario, Canada");

                await page.Keyboard.PressAsync("Enter");
                await page.WaitForNavigationAsync(new NavigationOptions { WaitUntil = [WaitUntilNavigation.Load]});

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
