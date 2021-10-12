using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Newtonsoft.Json;
using PerformanceTests.Helpers;
using PerformanceTests.Settings;

namespace PerformanceTests.Examples
{
    public static class RealTimeReportingExample
    {
        private static ConfigModel _configuration = TestSettings.Instance;

        public static async Task Run()
        {
            // Common tags and annotation tags

            var commonTags = new Dictionary<string, string>()
            {
                { "pipeline", TestSettings.Instance.TestRunSettings.ReportingSettings.Pipeline },
                { "build_simpleWebApp", TestSettings.Instance.TestRunSettings.ReportingSettings.Build_SimpleWebApp },
                { "test_suite", TestSettings.Instance.TestRunSettings.ReportingSettings.Test_Suite },
                { "test_name", TestSettings.Instance.TestRunSettings.ReportingSettings.Test_Name }
            };

            var annotationTags = new Dictionary<string, string>()
            {
                { "pipeline", TestSettings.Instance.TestRunSettings.ReportingSettings.Pipeline },
                { "build_SimpleWebApp", TestSettings.Instance.TestRunSettings.ReportingSettings.Build_SimpleWebApp },
                { "config_SimpleWebApp", TestSettings.Instance.TestRunSettings.ReportingSettings.Config_SimpleWebApp },
                { "TestDurationSeconds", TestSettings.Instance.TestRunSettings.TestDurationSeconds.ToString() }
            };

            // =========> Reporting 

            var influxDbCustomSink = new InfluxDBCustomSink(
                _configuration.InfluxDB.Url,
                _configuration.InfluxDB.User,
                _configuration.InfluxDB.Password,
                _configuration.InfluxDB.DataBaseName);

            influxDbCustomSink.GlobalMetricTags = commonTags;

            // =========> Client Factories

            var httpFactory = HttpClientFactory.Create("httpFactory");

            var kafkaFactory = ClientFactory.Create(
                name: "kafkaFactory",
                clientCount: 1,
                initClient: (number, context) => Task.FromResult(new ProducerBuilder<string, string>(
                    new ProducerConfig
                    {
                        BootstrapServers = _configuration.Kafka.Broker,
                        Acks = Acks.Leader,
                        MessageTimeoutMs = 10000
                    }).Build()));

            // =========> Steps
            var httpStepGenerateGuidIdsAndSaveToCache = Step.Create("HTTP_GenerateGuidIdsAndSaveToCache",
                httpFactory, async context =>
                {
                    var request = Http.CreateRequest("POST",
                            _configuration.TestRunSettings.SimpleAppUrl +
                            $"/api/Awesome/GenerateGuidIdsAndSaveToCache?howManyIds={_configuration.TestRunSettings.GuidCountsToCreate}")
                        .WithHeader("Accept", "text/plain");

                    return await Http.Send(request, context);
                }, TimeSpan.FromSeconds(10));


            var kafkaStep = Step.Create("KAFKA_SendSomething",
                kafkaFactory, async context =>
                {
                    await context.Client.ProduceAsync(_configuration.Kafka.Awesome_Topic,
                        new Message<string, string>()
                        {
                            Key = "\"" + Guid.NewGuid() + "\"",
                            Value = JsonConvert.SerializeObject(_configuration)
                        });

                    return Response.Ok();
                });

            var pause = Step.CreatePause(TimeSpan.FromMilliseconds(_configuration.TestRunSettings.PauseMs));

            // =========> Scenarios

            var rampUpSeconds = _configuration.TestRunSettings.RampUpSeconds;
            var durationAfterRampUpSeconds = _configuration.TestRunSettings.TestDurationSeconds - rampUpSeconds;

            var scenarioHttp = ScenarioBuilder
                .CreateScenario("CheckHttp", httpStepGenerateGuidIdsAndSaveToCache)
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.RampPerSec(_configuration.TestRunSettings.HttpUsers,
                        TimeSpan.FromSeconds(rampUpSeconds)),
                    Simulation.InjectPerSec(
                        _configuration.TestRunSettings.HttpUsers,
                        TimeSpan.FromSeconds(durationAfterRampUpSeconds))
                );

            var scenarioKafka = ScenarioBuilder
                .CreateScenario("CheckKafka", kafkaStep, pause)
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.RampConstant(_configuration.TestRunSettings.KafkaUsers,
                        TimeSpan.FromSeconds(rampUpSeconds)),
                    Simulation.KeepConstant(_configuration.TestRunSettings.KafkaUsers,
                        TimeSpan.FromSeconds(durationAfterRampUpSeconds))
                );

            // =========> Run

            await influxDbCustomSink.SaveAnnotationToInfluxDBAsync("---> Test started", annotationTags);

            NBomberRunner
                .RegisterScenarios(scenarioHttp, scenarioKafka)
                .WithTestSuite(TestSettings.Instance.TestRunSettings.ReportingSettings.Test_Suite)
                .WithTestName(TestSettings.Instance.TestRunSettings.ReportingSettings.Test_Name)
                .WithReportingSinks(influxDbCustomSink)
                .WithReportingInterval(TimeSpan.FromSeconds(5))
                .Run();

            await influxDbCustomSink.SaveAnnotationToInfluxDBAsync("<--- Test ended", annotationTags);
        }
    }
}