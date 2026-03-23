<p align="center">
  <img src="https://github.com/PragmaticFlow/NBomber/blob/dev/assets/nbomber_logo.png" alt="NBomber logo" width="600px">
</p>

[![NuGet](https://img.shields.io/nuget/v/nbomber.svg)](https://www.nuget.org/packages/nbomber/)
[![Nuget](https://img.shields.io/nuget/dt/nbomber.svg)](https://www.nuget.org/packages/nbomber/)

**NBomber** - Distributed load-testing framework for .NET.
It is designed to test any system regardless of the protocol (HTTP/WebSockets/AMQP) or a semantic model (Pull/Push).
It allows to write distributed load test scenarios in plain C# or F#.

[![NBomber 5](https://cdn.jsdelivr.net/gh/PragmaticFlow/NBomber@assets/v5.0/assets/NBomber_5_youtube.png)](https://youtu.be/Z51PyZvZNF8)

NBomber as a modern framework provides:
- Zero dependencies on protocol (HTTP/WebSockets/AMQP/SQL)
- Zero dependencies on semantic model (Pull/Push)
- Very flexible configuration and dead simple API
- **Native Debug** - debug your tests using your favorite IDE (Visual Studio, VS Code, Rider)
- [Informative Reports](https://nbomber.com/docs/reporting/reports): [HTML](https://nbomber.com/assets/reports/html_report.html)
- Realtime reporting:
    - [NBomber Studio](https://nbomber.com/docs/nbomber-studio/overview) - is a powerful management tool designed by NBomber. Monitor tests in real time with NBomber Studio and conduct detailed result analysis to identify trends. Check out our [blog posts about NBomber Studio](https://nbomber.com/blog/tags/nbomber-studio-release).
    - [Reporting Sinks](https://nbomber.com/docs/nbomber/reporting-sinks) for integration with popular systems for real-time monitoring
        - [InfluxDB](https://nbomber.com/docs/reporting/realtime/influx-db)
        - [TimescaleDB](https://nbomber.com/docs/reporting/realtime/timescale)
        - [Grafana](https://nbomber.com/docs/reporting/realtime/grafana)
- Protocols/extensions support:
  - [HTTP](https://nbomber.com/docs/protocols/http),
  - [WebSockets](https://nbomber.com/docs/protocols/websockets),
  - [MQTT](https://nbomber.com/docs/protocols/mqtt),
  - [AMQP](https://nbomber.com/docs/protocols/amqp)
  - [WebBrowser](https://nbomber.com/docs/protocols/webbrowser)
  - [Redis](https://github.com/PragmaticFlow/NBomber/tree/dev/examples/Demo/DB/Redis),
  - [SQLite](https://github.com/PragmaticFlow/NBomber/tree/dev/examples/Demo/DB/SQLiteDB),
  - MongoDB
  - gRPC
- [Data Feed](https://nbomber.com/docs/nbomber/data) - inject real or fake data into your tests
- [Distributed Cluster](https://nbomber.com/docs/cluster/overview) - run your load tests in distributed mode
- [Load Simulation](https://nbomber.com/docs/nbomber/load-simulation) - simulate realistic workloads
- [Dynamic Workloads](https://nbomber.com/docs/nbomber/dynamic-workloads) - simulate dynamic workloads
- [JSON Configuration](https://nbomber.com/docs/nbomber/json-config) - configure your load tests via JSON Config
- [Asserts and Thresholds](https://nbomber.com/docs/nbomber/asserts_and_thresholds)
- CI/CD integration ([xUnit](https://github.com/PragmaticFlow/NBomber/blob/dev/examples/xUnitExample/LoadTestExample.cs#L11) and NUnit runners are supported)

### Why we build NBomber and what you can do with it?
The main reason behind NBomber is to provide a lightweight framework for writing load tests which you can use to test literally any system and simulate any production workload. We wanted to provide only a few abstractions so that we could describe any type of load and still have a simple, intuitive API.
Another goal is to provide building blocks to validate your POC (proof of concept) projects by applying any complex load distribution.
With NBomber you can test any PULL or PUSH system (HTTP, WebSockets, GraphQl, gRPC, SQL Databse, MongoDb, Redis etc).
With NBomber you can convert some of your integration tests to load tests easily.

### What makes it very simple?
One of the design goals of NBomber is to keep API as minimal as possible.
Because of this, NBomber focuses on fully utilizing programming language(C#/F#) constructs instead of reinventing a new DSL that should be learned.
In other words, if you want to write a for loop, you don't need to learn a DSL for this.

```csharp
var scenario = Scenario.Create("hello_world_scenario", async context =>
{
    // you can define and execute any logic here,
    // for example: send http request, SQL query etc
    // NBomber will measure how much time it takes to execute your logic
    await Task.Delay(1_000);

    return Response.Ok();
})
.WithLoadSimulations(
    Simulation.Inject(rate: 10,
                      interval: TimeSpan.FromSeconds(1),
                      during: TimeSpan.FromSeconds(30))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();
```

### Videos

[Load Testing with C# and NBomber (Part 1)](https://youtu.be/XnK5sLhqXms)

### Blog Posts

- Anton Martyniuk - [Load Testing Microservices with C# and NBomber](https://antondevtips.com/blog/load-testing-microservices-with-csharp-and-nbomber)
- Anton Martyniuk - [Load Testing Kafka Pipelines with C# and NBomber](https://antondevtips.com/blog/load-testing-kafka-pipelines-with-charp-and-nbomber)
- Abdul Rahman - [Using NBomber for Performance, Load and Stress testing in ASP.NET WEB API](https://ilovedotnet.org/blogs/using-nbomber-for-performance-load-and-stress-testing-in-asp-net-webapi/)
- Olena Kostash - [Load Testing HTTP API on C# with NBomber](https://medium.com/@OlenaKostash/load-testing-http-api-on-c-with-nbomber-96939511bdab)
- ExecuteAutomation - [HTTP Performance Testing with NBomber in C# .NET](https://medium.com/executeautomation/http-performance-testing-with-nbomber-in-c-net-c858b887da1d)
- Stijn Moreels - [NBomber Load Tests in the xUnit Testing Framework for Invictus](https://www.codit.eu/blog/nbomber-load-tests-in-the-xunit-testing-framework-for-invictus)
- Stijn Moreels - [Integrating NBomber Load Testing with F# Expecto](https://www.codit.eu/blog/integrating-nbomber-load-testing-with-f-expecto/)
- Anh Nguyen Viet - [NBomber – Performance Testing Framework](https://blog.nashtechglobal.com/nbomber-performance-testing-framework/)

### Links
- [Website](https://nbomber.com/)
- [Documentation](https://nbomber.com/docs/getting-started/overview/)
- [Blog](https://nbomber.com/blog/)
- [Examples](https://github.com/PragmaticFlow/NBomber/tree/dev/examples/Demo)

### Frequently asked questions

| Question                                                                        | Answer                                                                                                                       |
|---------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------|
| How NBomber can be installed?                                                   | NBomber is shipped as .NET library and can be installed via [NuGet package manager](https://www.nuget.org/packages/NBomber/) |
| How can I run NBomber load test scenario?                                       | You can run it as Console application or as Unit test (xUnit/NUnit)                                                          |
| Can I use NBomber for free?                                                     | You can use it for free, only for personal use. For the organization usage, you should have a license.                       |
| Can multiple teams use the same license within one organization?                | Yes, a single license can be shared for the whole organization                                                               |
| How many users from the same organization can use the license at the same time? | Unlimited                                                                                                                   |
| Can I run NBomber Cluster without purchasing a license?                         | Yes, you can try [Local Dev Cluster](https://nbomber.com/docs/cluster/local-dev-cluster) mode                                |
| How many instances can be installed with one NBomber license?                   | Unlimited                                                                                                                    |
| How many NBomber executions can be run in parallel?                             | Unlimited                                                                                                                    |

### About Us

We are US based company [NBomber LLC](https://www.linkedin.com/company/nbomber) (8 The Green, Dover, Delaware 19901, USA).
