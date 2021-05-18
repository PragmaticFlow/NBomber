<p align="center">
  <img src="https://github.com/PragmaticFlow/NBomber/blob/master/assets/nbomber_logo.png" alt="NBomber logo" width="600px">
</p>

![Build status](https://github.com/PragmaticFlow/NBomber/actions/workflows/dotnet.yml/badge.svg?branch=dev)
[![NuGet](https://img.shields.io/nuget/v/nbomber.svg)](https://www.nuget.org/packages/nbomber/)
[![Gitter](https://badges.gitter.im/nbomber/community.svg)](https://gitter.im/nbomber/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

Very simple load testing framework for Pull and Push scenarios. It's 100% written in F# and targeting .NET Core and full .NET Framework.

### How to install
To install NBomber via NuGet, run this command in NuGet package manager console:
```code
PM> Install-Package NBomber
```

### Documentation
Documentation is located [here](https://nbomber.com/docs/overview/).

### Run test scenario
![how to run a scenario gif](https://nbomber.com/assets/images/nbomber_v2_console-6a596abc247223cefefa397c62e620f4.gif)

### View report
![view report](https://raw.githubusercontent.com/PragmaticFlow/NBomber/dev/assets/nbomber_report.jpg)

### Analyze trends
![analyze trends](https://github.com/PragmaticFlow/NBomber/blob/dev/assets/influx_trends.png)

### Why we build NBomber and what you can do with it?

1. The main reason behind NBomber is to provide a **lightweight** framework for writing load tests which you can use to test literally **any** system and simulate **any** production workload. We wanted to provide only a few abstractions so that we could describe any type of load and still have a simple, intuitive API.
2. Another goal is to provide building blocks to validate your POC (proof of concept) projects by applying any complex load distribution.
3. With NBomber you can test any PULL or PUSH system (HTTP, WebSockets, GraphQl, gRPC, SQL Databse, MongoDb, Redis etc).

NBomber as a modern framework provides:
- Zero dependencies on protocol (HTTP/WebSockets/AMQP/SQL)
- Zero dependencies on semantic model (Pull/Push)
- Very flexible configuration and dead simple API
- Cluster support
- Reporting sinks
- CI/CD integration
- Plugins/extensions support
- Data feed support

### What makes it very simple?
NBomber is a foundation of building blocks which you can use to describe your test scenario, run it and get reports.

```fsharp
// FSharp example

let step = Step.create("step", fun context -> task {

    // you can do any logic here: go to http, websocket etc
    do! Task.Delay(seconds 1)
    return Response.Ok()
})

Scenario.create "scenario" [step]
|> NBomberRunner.registerScenario
|> NBomberRunner.run
```

```csharp
// CSharp example

var step = Step.Create("step", async context =>
{
    // you can do any logic here: go to http, websocket etc

    await Task.Delay(TimeSpan.FromSeconds(1));
    return Response.Ok();
});

var scenario = ScenarioBuilder.CreateScenario("scenario", step);

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();
```

### Examples
|Language|Example|
|--|--|
| F# | [link](https://github.com/PragmaticFlow/NBomber/tree/dev/examples/FSharp) |
| C# | [link](https://github.com/PragmaticFlow/NBomber/tree/dev/examples/CSharp) |

### Contributing
Would you like to help make NBomber even better? We keep a list of issues that are approachable for newcomers under the [good-first-issue](https://github.com/PragmaticFlow/NBomber/issues?q=is%3Aopen+is%3Aissue+label%3A%22good+first+issue%22) label.
