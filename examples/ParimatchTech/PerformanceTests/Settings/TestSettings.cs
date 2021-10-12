using System;
using Microsoft.Extensions.Configuration;
using PerformanceTests.Settings;

namespace PerformanceTests
{
    public static class TestSettings
    {
        public static ConfigModel Instance;

        static TestSettings()
        {
            Instance = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables()
                .Build().Get<ConfigModel>();
        }
    }
}
