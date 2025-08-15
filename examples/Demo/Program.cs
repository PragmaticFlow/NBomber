using Bogus;
using Demo.Cluster.AutoCluster;
using Demo.Cluster.ManualCluster;
using Demo.DB.LiteDB;
using Demo.DB.Redis;
using Demo.DB.SQLiteDB;
using Demo.Features;
using Demo.Features.CliArgs;
using Demo.Features.CustomMetrics;
using Demo.Features.CustomSettings;
using Demo.Features.DataDemo;
using Demo.Features.DynamicWorkload;
using Demo.Features.Logger.Elasticsearch;
using Demo.Features.Logger.GrafanaLoki;
using Demo.Features.Logger.TextFile;
using Demo.Features.RealtimeReporting.CustomReportingSink;
using Demo.Features.RealtimeReporting.InfluxDB;
using Demo.Features.Thresholds;
using Demo.HelloWorld;
using Demo.HelloWorld.LoadSimulation;
using Demo.HTTP;
using Demo.HTTP.WebAppSimulator;
using Demo.HTTP.SimpleBookstore;
using Demo.MQTT;
using Demo.MQTT.ClientPool;
using Demo.MQTT.IndependentActors;
using Demo.NBomber_Studio;
using Demo.WebBrowsers.Playwright;
using Demo.WebBrowsers.Puppeteer;
using Demo.WebSockets;
using Demo.WebSockets.ClientPool;
using Demo.gRPC;
using Demo.AMQP;
using Demo.AMQP.ClientPool;
using Demo.AMQP.IndependentActors;
using Demo.Features.RealtimeReporting.Datadog;
using Demo.Features.Timeouts;
using Demo.HTTP.RestSharpDemo;

// -------------------------------
// ----- Hello World examples -----
// -------------------------------
// new HelloWorldExample().Run();
// new ScenarioWithInit().Run();
// new ScenarioWithSteps().Run();
// new StepsShareData().Run();
// new LoggerExample().Run();

// ----- Load Simulations -----
// new ParallelScenarios().Run();
// new ScenarioInjectRate().Run();
// new ScenarioKeepConstant().Run();
// new DelayedScenarioStart().Run();
// new ScenarioTotalIterations().Run();

// new ScenarioWithTimeout().Run();
// new ScenarioWithStepRetry().Run();
// new EmptyScenario().Run();
// new ScenarioTimerTimeExample().Run();

// ------------------
// ---- Features ----
// ------------------
// new CustomSettingsExample().Run();
// new ClientPoolMqttExample().Run();
// new CliArgsExample().Run();
// new CustomMetricsExample().Run();
// new ScenarioCompletionTimeout().Run();

// ---- DataFeed ----
// new InMemoryFeed().Run();
// new JsonFeed().Run();
// new CsvFeed().Run();
// new FakeDataGenExample().Run();

// ---- Real-time reporting ----
// new InfluxDBReportingExample().Run();
new TimescaleDBReportingExample().Run();
// new DatadogReportingExample().Run();
// new CustomReportingExample().Run();
// new NBomberStudioReportingExample().Run();

// ---- Logs ----
// new TextFileLogger().Run();
// new ElasticsearchLogger().Run();
// new GrafanaLokiLogger().Run();

// ---- Thresholds ----
// new ThresholdsExample().Run();
// new ThresholdsConfigExample().Run();

// ---- Dynamic Workloads ----
// new ScenarioWeightExample().Run();
// new InstanceNumberDistributionExample().Run();
// new UniformDistributionExample().Run();
// new ZipfianDistributionExample().Run();
// new MultinomialDistributionExample().Run();

// ----------------
// ----- HTTP -----
// ----------------
// new SimpleHttpExample().Run();
// new SequentialHttpSteps().Run();
// new HttpResponseValidation().Run();
// new HttpSendJsonExample().Run();
// new HttpClientArgsExample().Run();
// new WebAppSimulatorExample().RunHttpUserExample();
// new ExampleSimpleBookstore().Run();
// new HttpRequestTracing().Run();
// new CookiesManagementExample().Run();
// new CustomHttpResponseValidation().Run();
// new HttpWithTimeoutExample().Run();

// new RestSharpSimpleHttpExample().Run();

// ----------------
// -- WebSockets --
// ----------------
// new PingPongWebSocketsTest().Run();
// new ClientPoolWebSocketsExample().Run();

// ----------------
// ----- MQTT -----
// ----------------
// new PingPongMqttTest().Run();
// new ClientPoolMqttExample().Run();
// new IndependentActorsMqttExample().Run();

// ----------------
// ----- AMQP -----
// ----------------
// new PingPongAmqpTest().Run();
// new ClientPoolAmqpExample().Run();
// new IndependentActorsAmqpExample().Run();

// ----------------
// ----- gRPC -----
// ----------------
// new GrpcExample().Run();

// ----------------
// ----- Db -------
// ----------------
// new LiteDBExample().Run();
// new SQLiteDBExample().Run();
// new RedisExample().Run();

// ---------------------
// ----- Cluster -------
// ---------------------

// ----- Auto Cluster -------
// to run this example you should start 2 instances of NBomber,
// for this, you need to run this NBomber application twice

// new AutoClusterExample().Run(); // 1 instance
// new AutoClusterExample().Run(); // 2 instance

// ----- Manual Cluster -------
// to run this example, you should start 2 instances of NBomber,
// - first instance will act as Coordinator
// - second instance will act as Agent

// new ManualClusterExample().Run(args: new [] { "--cluster-node-type=coordinator" });
// new ManualClusterExample().Run(args: new [] { "--cluster-node-type=agent", "--cluster-agent-group=1" });

// --------------------------
// ----- Web Browsers -------
// --------------------------
// await new PuppeteerExample().Run();
// await new PlaywrightExample().Run();

