using Demo.Cluster.AutoCluster;
using Demo.Cluster.ManualCluster;
using Demo.DB.LiteDB;
using Demo.DB.Redis;
using Demo.DB.SQLiteDB;
using Demo.Features;
using Demo.Features.CliArgs;
using Demo.Features.CustomSettings;
using Demo.Features.DataDemo;
using Demo.Features.DynamicWorkload;
using Demo.Features.ElasticsearchLogger;
using Demo.Features.RealtimeReporting.CustomReportingSink;
using Demo.Features.RealtimeReporting.InfluxDB;
using Demo.Features.Timeouts;
using Demo.HelloWorld;
using Demo.HelloWorld.LoadSimulation;
using Demo.HTTP;
using Demo.MQTT;
using Demo.HTTP.WebAppSimulator;
using Demo.HTTP.SimpleBookstore;
using Demo.MQTT.ConstantRate;
using Demo.WebSockets;

// -------------------------------
// ----- Hello World examples -----
// -------------------------------
new HelloWorldExample().Run();
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

// ------------------
// ---- Features ----
// ------------------
// new CustomSettingsExample().Run();
// new ClientPoolMqttExample().Run();
// new CliArgsExample().Run();

// ---- DataFeed ----
// new InMemoryFeed().Run();
// new JsonFeed().Run();
// new CsvFeed().Run();
// new FakeDataGenExample().Run();

// ---- Real-time reporting ----
// new InfluxDBReportingExample().Run();
// new CustomReportingExample().Run();

// ---- Logs ----
// new ElasticsearchExample().Run();

// ---- Timeouts ----
// new ScenarioCompletionTimeout().Run();
// new HttpWithTimeoutExample().Run();

// ---- Dynamic Workloads ----
// new ThreadIdDistributionExample().Run();
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

// ----------------
// ----- WebSockets -----
// ----------------
// new PingPongWebSocketsTest().Run();

// ----------------
// ----- MQTT -----
// ----------------
// new PingPongMqttTest().Run();
// new ConstantPublishToMqtt().Run();
// new ClientPoolMqttExample().Run();

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
// in order to run this example you should start 2 instances of NBomber,
// for this, you need to run this NBomber application twice

// new AutoClusterExample().Run(); // 1 instance
// new AutoClusterExample().Run(); // 2 instance

// ----- Manual Cluster -------
// in order to run this example you should start 2 instances of NBomber,
// - first instance will act as Coordinator
// - second instance will act as Agent

// new ManualClusterExample().Run(args: new [] { "--cluster-node-type=coordinator" });
// new ManualClusterExample().Run(args: new [] { "--cluster-node-type=agent", "--cluster-agent-group=1" });

