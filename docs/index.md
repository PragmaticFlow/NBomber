<p align="center">
  <img src="https://raw.githubusercontent.com/PragmaticFlow/NBomber/master/assets/nbomber_logo.png" alt="NBomber logo" width="600px"> 
</p>

Very simple load testing framework for testing Pull/Push-based systems. **You can test any system regardless of protocol and communication model**. In addition, you can run sequential flows to measure how fast one message can go across all system layers(from A => B => C).

### Features
- Pull scenario (Request-response)
- Push scenario (Pub/Sub)
- Sequential flow
- Test runner support: [XUnit; NUnit]
- Reporting: [Plain text; HTML]
- Distibuted cluster (run scenario from several nodes in parallel)

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

## Why another {x} framework for load testing?
The main reasons are:
 - **To be technology agnostic** as much as possible (**no dependency on any protocol: HTTP, WebSockets, SSE etc**).
 - To be able to test .NET implementation of specific driver. During testing, it was identified many times that the performance could be slightly different because of the virtual machine(.NET, Java, PHP, Js, Erlang, different settings for GC) or just quality of drivers. For example there were cases that drivers written in C++ and invoked from NodeJs app worked faster than drivers written in C#/.NET. Therefore, it does make sense to load test your app using your concrete driver and runtime.

 ### What makes it very simple? 
NBomber is not really a framework but rather a foundation of building blocks which you can use to describe your test scenario, run it and get reports.
```csharp
var scenario = new ScenarioBuilder(scenarioName: "Test MongoDb")                
                .AddTestFlow("READ Users", steps: new[] { mongoQuery }, concurrentCopies: 10)
                .Build(duration: TimeSpan.FromSeconds(10));

scenario.RunInConsole(scenario);
```

### Contributing
Would you like to help make NBomber even better? We keep a list of issues that are approachable for newcomers under the [good-first-issue](https://github.com/PragmaticFlow/NBomber/issues?q=is%3Aopen+is%3Aissue+label%3A%22good+first+issue%22) label.