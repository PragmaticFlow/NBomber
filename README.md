<p align="center">
  <img src="https://github.com/PragmaticFlow/NBomber/blob/master/assets/nbomber_logo.png" alt="NBomber logo" width="600px"> 
</p>

[![Build status](https://ci.appveyor.com/api/projects/status/kko2ro88xry274do?svg=true)](https://ci.appveyor.com/project/PragmaticFlowOrg/nbomber)
[![NuGet](https://img.shields.io/nuget/v/nbomber.svg)](https://www.nuget.org/packages/nbomber/)
[![Gitter](https://badges.gitter.im/nbomber/community.svg)](https://gitter.im/nbomber/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

Very simple load testing framework for Pull and Push scenarios. It's 100% written in F# and targeting .NET Core and full .NET Framework.

### How to install
To install NBomber via NuGet, run this command in NuGet package manager console:
```code
PM> Install-Package NBomber
```

### Documentation
Documentation is located [here](https://nbomber.com).

### Run test scenario
![how to run a scenario gif](https://raw.githubusercontent.com/PragmaticFlow/NBomber/dev/assets/nbomber_run.gif)

### View report
![view report](https://raw.githubusercontent.com/PragmaticFlow/NBomber/dev/assets/nbomber_report.jpg)

### Analyze trends
![analyze trends](https://github.com/PragmaticFlow/NBomber/blob/dev/assets/influx_trends.png)

### Features
- [x] Pull scenario (Request-response)
- [x] Push scenario (Pub/Sub)
- [x] Sequential flow
- [x] Test runner support: [XUnit; NUnit]
- [x] Cluster support (run scenario from several nodes in parallel)
- [x] Reporting: [Plain text; HTML; Csv; Md]
- [x] Statistics sinks: (analyze and monitor performance trends via sinking statistics to any data storage)

### Supported technologies
- Supported runtimes: .NET Framework (4.6+), .NET Core (2.0+), Mono, CoreRT
- Supported languages: C#, F#, Visual Basic
- Supported OS: Windows, Linux, macOS

### Examples
|Scenario|Language|Example|
|--|--|--|
| HTTP | C# | [Test HTTP (https://github.com)](https://github.com/PragmaticFlow/NBomber/blob/dev/examples/CSharp/CSharp.Examples/Scenarios/Http.cs) |
| MongoDb | C# | [Test MongoDb with 2 READ queries](https://github.com/PragmaticFlow/NBomber/blob/dev/examples/CSharp/CSharp.Examples/Scenarios/MongoDb.cs)|
| NUnit integration | C# | [Simple NUnit test](https://github.com/PragmaticFlow/NBomber/blob/dev/examples/CSharp/CSharp.Examples.NUnit/Tests.cs) |
| WebSockets | C# | [Test ping and pong on WebSockets](https://github.com/PragmaticFlow/NBomber/blob/dev/examples/CSharp/CSharp.Examples/Scenarios/WebSockets.cs) |
| HTTP | F# | [Test HTTP (https://github.com)](https://github.com/PragmaticFlow/NBomber/blob/dev/examples/FSharp/FSharp.Examples/Scenarios/Http.fs) |
| XUnit integration | F# | [Simple XUnit test](https://github.com/PragmaticFlow/NBomber/blob/dev/examples/FSharp/FSharp.Examples.XUnit/Tests.fs) |
| Expecto integration | F# | [Simple Expecto test](https://github.com/PragmaticFlow/NBomber/blob/dev/examples/FSharp/FSharp.Examples.Expecto/Tests.fs) |

### Contributing
Would you like to help make NBomber even better? We keep a list of issues that are approachable for newcomers under the [good-first-issue](https://github.com/PragmaticFlow/NBomber/issues?q=is%3Aopen+is%3Aissue+label%3A%22good+first+issue%22) label.

## Why another {x} framework for load testing?
The main reasons are:
 - **To be technology agnostic** as much as possible (**no dependency on any protocol: HTTP, WebSockets, SSE etc**).
 - To be able to test .NET implementation of specific driver. During testing, it was identified many times that the performance could be slightly different because of the virtual machine(.NET, Java, PHP, Js, Erlang, different settings for GC) or just quality of drivers. For example there were cases that drivers written in C++ and invoked from NodeJs app worked faster than drivers written in C#/.NET. Therefore, it does make sense to load test your app using your concrete driver and runtime.

 ### What makes it very simple? 
NBomber is not really a framework but rather a foundation of building blocks which you can use to describe your test scenario, run it and get reports.
```csharp
var step = Step.Create("step", async context =>
{
    // you can do any logic here: go to http, websocket etc

    await Task.Delay(TimeSpan.FromSeconds(0.1));
    return Response.Ok();
});

var scenario = ScenarioBuilder.CreateScenario("Hello World!", step);

NBomberRunner.RegisterScenarios(scenario)
             .RunInConsole();
```
