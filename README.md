<p align="center">
  <img src="https://github.com/PragmaticFlow/NBomber/blob/master/assets/nbomber_logo.png" alt="NBomber logo" width="600px"> 
</p>

[![Build status](https://ci.appveyor.com/api/projects/status/kko2ro88xry274do?svg=true)](https://ci.appveyor.com/project/PragmaticFlowOrg/nbomber)
[![NuGet](https://img.shields.io/nuget/v/nbomber.svg)](https://www.nuget.org/packages/nbomber/)

Very simple load testing framework for Pull and Push scenarios. It's 100% written in F# and targeting .NET Core and full .NET Framework.

### How to install
To install NBomber via NuGet, run this command in NuGet package manager console:
```code
PM> Install-Package NBomber
```

### How to run a simple scenario

![how to run a scenario gif](https://github.com/PragmaticFlow/NBomber/blob/master/assets/howToRunScenario.gif)

### Documentation
Documentation is located [here](https://nbomber.com).

### Features
- [x] Pull scenario (Request-response)
- [x] Push scenario (Pub/Sub)
- [x] Sequential flow
- [x] Test runner support: [XUnit; NUnit]
- [x] Cluster support (run scenario from several nodes in parallel)
- [x] Reporting: [Plain text; HTML]

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

### Contributing
Would you like to help make NBomber even better? We keep a list of issues that are approachable for newcomers under the [good-first-issue](https://github.com/PragmaticFlow/NBomber/issues?q=is%3Aopen+is%3Aissue+label%3A%22good+first+issue%22) label.

## Why another {x} framework for load testing?
The main reasons are:
 - **To be technology agnostic** as much as possible (**no dependency on any protocol: HTTP, WebSockets, SSE etc**).
 - To be able to test .NET implementation of specific driver. During testing, it was identified many times that the performance could be slightly different because of the virtual machine(.NET, Java, PHP, Js, Erlang, different settings for GC) or just quality of drivers. For example there were cases that drivers written in C++ and invoked from NodeJs app worked faster than drivers written in C#/.NET. Therefore, it does make sense to load test your app using your concrete driver and runtime.

 ### What makes it very simple? 
NBomber is not really a framework but rather a foundation of building blocks which you can use to describe your test scenario, run it and get reports.
```csharp
var step1 = Step.CreatePull("simple step", ConnectionPool.None, async context =>
{
    // you can do any logic here: go to http, websocket etc
    // NBomber will measure execution of this lambda function
    await Task.Delay(TimeSpan.FromSeconds(0.1));    
    return Response.Ok();
});

var scenario = ScenarioBuilder.CreateScenario("Hello World from NBomber!", step1);

NBomberRunner.RegisterScenarios(scenario)
             .RunInConsole();
```
