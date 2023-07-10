using CSharpProd.DB.LiteDB;
using CSharpProd.DB.Redis;
using CSharpProd.DB.SQLiteDB;
using CSharpProd.Features;
using CSharpProd.Features.CliArgs;
using CSharpProd.Features.CustomSettings;
using CSharpProd.Features.ElasticsearchLogger;
using CSharpProd.Features.RealtimeReporting.CustomReportingSink;
using CSharpProd.Features.RealtimeReporting.InfluxDB;
using CSharpProd.HelloWorld;
using CSharpProd.HelloWorld.LoadSimulation;
using CSharpProd.HTTP;
using CSharpProd.MQTT;
using CSharpProd.HTTP.WebAppSimulator;


// -------------------------------
// -----Hello World examples -----
// -------------------------------
new HelloWorldExample().Run();
// new ScenarioWithInit().Run();
// new ScenarioWithSteps().Run();
// new StepsShareData().Run();
// new LoggerExample().Run();

// new ParallelScenarios().Run();
// new ScenarioInjectRate().Run();
// new ScenarioKeepConstant().Run();
// new DelayedScenarioStart().Run();

// new ScenarioWithTimeout().Run();
// new ScenarioWithStepRetry().Run();
// new EmptyScenario().Run();

// ------------------
// ---- Features ----
// ------------------
// new DataFeedExample().Run();
// new CustomSettingsExample().Run();
// new ClientPoolMqttExample().Run();
// new CliArgsExample().Run();
// new InfluxDBReportingExample().Run();
// new CustomReportingExample().Run();
// new ElasticsearchExample().Run();

// ----------------
// ----- HTTP -----
// ----------------
// new SimpleHttpExample().Run();
// new SequentialHttpSteps().Run();
// new HttpResponseValidation().Run();
// new HttpClientArgsExample().Run();
// new WebAppSimulatorExample().RunHttpUserExample();

// ----------------
// ----- MQTT -----
// ----------------
// new PingPongMqttTest().Run();
// new ClientPoolMqttExample().Run();

// ----------------
// ----- Db -------
// ----------------
// new LiteDBExample().Run();
// new SQLiteDBExample().Run();
// new RedisExample().Run();

