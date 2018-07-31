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
Scenario.create("Test MongoDb")
|> Scenario.addTestFlow({ FlowName = "READ Users"; Steps = [mongoQuery]; ConcurrentCopies = 10 })
|> Scenario.build(TimeSpan.FromSeconds(10.0))
|> Scenario.run
```

## API Documentation
The whole API is built around 3 building blocks:
```fsharp
// Represents single executable Step
type Step =
    | Request  of RequestStep
    | Listener of ListenerStep
    | Pause    of TimeSpan  

// Represents TestFlow which groups steps and execute them sequentially on dedicated System.Threading.Task
type TestFlow = {
    FlowName: string
    Steps: Step[]         // these steps will be executed sequentially, one by one
    ConcurrentCopies: int // specify how many copies of current TestFlow to run in parallel
}

// Represents Scenario
type Scenario = {
    ScenarioName: string
    TestInit: Step option // init step will be executed at start of every scenario
    TestFlows: TestFlow[] // each TestFlow will be executed on dedicated System.Threading.Task
    Duration: TimeSpan    // execution time of scenario 
}
```

**Step** is a basic element which will be executed and measured. You can use:
 - Request - to model Request-response pattern.
 - Listener - to model Pub/Sub pattern. 
 - Pause - to model pause in your test flow.

```csharp
// C# example of simple Request step
var reqStep = Step.CreateRequest(name: "simple step", execute: req => Task.FromResult(Response.Ok()))
``` 
```fsharp
// F# example of simple Request step
let reqStep = Step.createRequest("simple step", fun req -> task { return Response.Ok() })
```

**TestFlow** is basically a container for steps(you can think of TestFlow like a Job of sequential operations). **All steps within one TestFlow are executing sequentially**. It helps you model dependently ordered operations like: 
```csharp
// C# example of TestFlow with 3 sequential steps
var testFlow = new TestFlow(flowName: "Sequantial flow",
                            steps: new [] { authenticateUserStep, buyProductStep, logoutUserStep },
                            concurrentCopies: 10)
```
```fsharp
// F# example of TestFlow with 3 sequential steps
let testFlow = { FlowName = "Sequantial flow"
                 Steps = [authenticateUserStep, buyProductStep, logoutUserStep]
                 ConcurrentCopies = 10 }
```
### Request-response scenario
The Request-response is represented via RequestStep.
```fsharp
type RequestStep = {
    StepName: StepName
    Execute: Request -> Task<Response>
}
```
To create RequestStep you should use
```fsharp
// for F#
Step.createRequest(name: string, execute: Request -> Task<Response>)
```
```csharp
// for C#
Step.CreateRequest(name: string, execute: Func<Request, Task<Response>>)
```
