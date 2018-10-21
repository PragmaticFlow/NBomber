<p align="center">
  <img src="https://github.com/PragmaticFlow/NBomber/blob/master/assets/nbomber_logo.png" alt="NBomber logo" width="600px"> 
</p>

[![Build status](https://ci.appveyor.com/api/projects/status/sf3ntwjvb4s0xoya?svg=true)](https://ci.appveyor.com/project/PragmaticFlowOrg/nbomber)
[![NuGet](https://img.shields.io/nuget/v/nbomber.svg)](https://www.nuget.org/packages/nbomber/)

Very simple load testing framework for Pull and Push scenarios. It's 100% written in F# and targeting .NET Core and full .NET Framework.

### How to install
To install NBomber via NuGet, run this command in NuGet package manager console:
```code
PM> Install-Package NBomber
```

### Documentation
Documentation is located [here](https://nbomber.com/documentation).

### Features
- [x] Request-response scenario
- [x] Pub/Sub scenario
- [x] Sequential flow
- [x] Test runner support: [XUnit; NUnit]
- [x] Reporting: [Plain text; HTML]

### Supported technologies
- Supported runtimes: .NET Framework (4.6+), .NET Core (2.0+), Mono, CoreRT
- Supported languages: C#, F#, Visual Basic
- Supported OS: Windows, Linux, macOS

### Examples
|Scenario|Language|Example|
|--|--|--|
| HTTP | C# | [Test HTTP (https://github.com) with 100 concurrent users](https://github.com/PragmaticFlow/NBomber/blob/master/examples/CSharp.Example.Http/Program.cs) |
| MongoDb | C# | [Test MongoDb with 2 READ queries and 2000 docs](https://github.com/PragmaticFlow/NBomber/blob/master/examples/CSharp.Example.MongoDb/Program.cs) |
| NUnit integration | C# | [Simple NUnit test](https://github.com/PragmaticFlow/NBomber/blob/master/examples/CSharp.Example.NUnit/Tests.cs) |
| Simple Push | C# | [Test fake push server](https://github.com/PragmaticFlow/NBomber/blob/master/examples/CSharp.Example.SimplePush/Program.cs) |
| HTTP | F# | [Test HTTP (https://github.com) with 100 concurrent users](https://github.com/PragmaticFlow/NBomber/blob/master/examples/FSharp.Example.Http/Program.fs) |
| XUnit integration | F# | [Simple XUnit test](https://github.com/PragmaticFlow/NBomber/blob/master/examples/FSharp.Example.XUnit/Tests.fs) |

### Contributing
Would you like to help make NBomber even better? We keep a list of issues that are approachable for newcomers under the [good-first-issue](https://github.com/PragmaticFlow/NBomber/issues?q=is%3Aopen+is%3Aissue+label%3A%22good+first+issue%22) label.

## Why another {x} framework for load testing?
The main reasons are:
 - **To be technology agnostic** as much as possible (**no dependency on any protocol: HTTP, WebSockets, SSE**).
 - To be able to test .NET implementation of specific driver. During testing, it was identified many times that the performance could be slightly different because of the virtual machine(.NET, Java, PHP, Js, Erlang, different settings for GC) or just quality of drivers. For example there were cases that drivers written in C++ and invoked from NodeJs app worked faster than drivers written in C#/.NET. Therefore, it does make sense to load test your app using your concrete driver and runtime.

### What makes it very simple? 
NBomber is not really a framework but rather a foundation of building blocks which you can use to describe your test scenario, run it and get reports.
```csharp
// simple C# example
var scenario = new ScenarioBuilder(scenarioName: "Test MongoDb")                
                .AddTestFlow("READ Users", steps: new[] { mongoQuery }, concurrentCopies: 10)                
                .Build(duration: TimeSpan.FromSeconds(10));

scenario.RunInConsole();
```
```fsharp
// simple F# example
Scenario.create("Test MongoDb")
|> Scenario.addTestFlow({ FlowName = "READ Users"; Steps = [mongoQuery]; ConcurrentCopies = 10 })
|> Scenario.withDuration(TimeSpan.FromSeconds(10.0))
|> Scenario.runInConsole
```
