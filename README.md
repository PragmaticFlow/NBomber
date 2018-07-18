# NBomber
[![Build status](https://ci.appveyor.com/api/projects/status/ukphl1c0s9cuf4jl?svg=true)](https://ci.appveyor.com/api/projects/status/github/VIP-Logic/NBomber?branch=master&svg=true)

Very simple load testing framework for Request-response and Pub/Sub scenarios. It's 100% written in F# and targeting .NET Core and full .NET Framework.

### How to install
To install NBomber via NuGet, run this command in NuGet package manager console:
```code
PM> Install-Package VIP-Logic.NBomber
```

### Features
- [x] Request-response scenario
- [x] Pub/Sub scenario
- [x] Sequential flow
- [ ] Distibuted cluster (run scenario from several nodes in parallel)

### Supported technologies
- Supported runtimes: .NET Framework (4.6+), .NET Core (2.0+), Mono, CoreRT
- Supported languages: C#, F#, Visual Basic
- Supported OS: Windows, Linux, macOS

### Examples
|Scenario|Language|Example|
|--|--|--|
| Request-response | C# | [Test MongoDb with 2 READ queries and 2000 docs](https://github.com/VIP-Logic/NBomber/blob/master/examples/NBomber.Examples.CSharp/Scenarios/MongoScenario.cs) |
| Request-response | C# | [Test HTTP (https://github.com) with 100 concurrent users](https://github.com/VIP-Logic/NBomber/blob/master/examples/NBomber.Examples.CSharp/Scenarios/HttpScenario.cs) |
| Request-response | F# | [Test HTTP (https://github.com) with 100 concurrent users](https://github.com/VIP-Logic/NBomber/blob/master/examples/NBomber.Examples.FSharp/Scenarios/HttpScenario.fs) |

### Contributing
Would you like to help make NBomber even better? We keep a list of issues that are approachable for newcomers under the [good-first-issue](https://github.com/VIP-Logic/NBomber/issues?q=is%3Aopen+is%3Aissue+label%3A%22good+first+issue%22) label.

## Why another {x} framework for load testing?
The main reasons are:
 - **To be technology agnostic** as much as it possible (**no dependency on any protocol: HTTP, WebSockets, SSE**).
 - To be able to test .NET implementation of specific driver. During testing, it was identified many times that the performance could be slightly different because of the virtual machine(.NET, Java, PHP, Js, Erlang, different settings for GC) or just quality of drivers. For example there ware cases that drivers written in C++ and invoked from NodeJs app worked faster than drivers written in C#/.NET. Therafore it does make sense to load test your app using your concrete driver and runtime.

### What makes it very simple? 
NBomber is not really a framework but rather a foundation of building blocks which you can use to describe your test scenario, run it and get reports.
```csharp
// simple C# example
var scenario = new ScenarioBuilder(scenarioName: "Test MongoDb")                
                .AddTestFlow("READ Users", steps: new[] { mongoQuery }, concurrentCopies: 10)                
                .Build(duration: TimeSpan.FromSeconds(10));

ScenarioRunner.Run(scenario)
```
```fsharp
// simple F# example
scenario("Test MongoDb")
|> addTestFlow({ FlowName = "READ Users"; Steps = [|mongoQuery|]; ConcurrentCopies = 10 })
|> build(TimeSpan.FromSeconds(10))
|> run
```

## API Documentation
### Request-response scenario
The whole API is built around 3 building blocks:
```fsharp
// Represents single executable Step
type Step =
    | Request of RequestStep
    | Listen  of ListenStep
    | Pause   of TimeSpan  

// Represents TestFlow which groups steps and execute them sequentially on dedicated System.Threading.Task
type TestFlow = {
    FlowName: string
    Steps: Step[]         // these steps will be executed sequentially, one by one
    ConcurrentCopies: int // specify how many copies of current TestFlow to run in parallel
}

// Represents Scenario
type Scenario = {
    ScenarioName: string
    InitStep: Step option // init step will be executed at start of every scenario
    Flows: TestFlow[]     // each TestFlow will be executed on dedicated System.Threading.Task
    Duration: TimeSpan    // execution time of scenario 
}
```
