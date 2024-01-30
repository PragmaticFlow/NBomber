<p align="center">
  <img src="https://github.com/PragmaticFlow/NBomber/blob/dev/assets/nbomber_logo.png" alt="NBomber logo" width="600px">
</p>

[![NuGet](https://img.shields.io/nuget/v/nbomber.svg)](https://www.nuget.org/packages/nbomber/)
[![Nuget](https://img.shields.io/nuget/dt/nbomber.svg)](https://www.nuget.org/packages/nbomber/)

NBomber is a modern and flexible load-testing framework for Pull and Push scenarios, designed to test any system regardless of a protocol (HTTP/WebSockets/AMQP, etc) or a semantic model (Pull/Push).

NBomber is free for personal use, developer-centric, and extensible.
Using NBomber, you can test the reliability and performance of your systems and catch performance regressions and problems earlier. 
NBomber will help you to build resilient and performant applications that scale.

[![NBomber 5](https://cdn.jsdelivr.net/gh/PragmaticFlow/NBomber@assets/v5.0/assets/NBomber_5_youtube.png)](https://youtu.be/Z51PyZvZNF8)

### Links
- [Website](https://nbomber.com/)
- [Documentation](https://nbomber.com/docs/getting-started/overview/)

### Why we build NBomber and what you can do with it?
The main reason behind NBomber is to provide a lightweight framework for writing load tests which you can use to test literally any system and simulate any production workload. We wanted to provide only a few abstractions so that we could describe any type of load and still have a simple, intuitive API.
Another goal is to provide building blocks to validate your POC (proof of concept) projects by applying any complex load distribution.
With NBomber you can test any PULL or PUSH system (HTTP, WebSockets, GraphQl, gRPC, SQL Databse, MongoDb, Redis etc).
With NBomber you can convert some of your integration tests to load tests easily.

NBomber as a modern framework provides:
- Zero dependencies on protocol (HTTP/WebSockets/AMQP/SQL)
- Zero dependencies on semantic model (Pull/Push)
- Very flexible configuration and dead simple API
- [Distributed cluster support](https://nbomber.com/docs/cluster/overview)
- Real-time reporting
- CI/CD integration (xUnit and NUnit runners are supported)
- Plugins/extensions support - add your own plugins or data sinks
- Data feed support - inject real or fake data into your tests
- **Debuggability of your load test** - debug your tests using your favorite IDE

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

### Examples
|Type|Language
|--|--|
| [NBomber Demo](https://github.com/PragmaticFlow/NBomber/tree/dev/examples/Demo) | C# |

