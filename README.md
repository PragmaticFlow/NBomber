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

## API Usage
The whole API is built around 3 building blocks: Step, TestFlow, Scenario.
```fsharp
// Represents single executable Step
// it's a basic element which will be executed and measured
type Step =
    | Request  of RequestStep  // to model Request-response pattern
    | Listener of ListenerStep // to model Pub/Sub pattern
    | Pause    of TimeSpan     // to model pause in your test flow

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

### Step
Step is a basic element(you can think of Step like a function) which will be executed and measured. 
```fsharp
type Step =
    | Request  of RequestStep  // to model Request-response pattern
    | Listener of ListenerStep // to model Pub/Sub pattern
    | Pause    of TimeSpan     // to model pause in your test flow
```    
NBomber provides 3 type of steps:
- **Request** - to model Request-response pattern. You can use Request step to simulate testing of database, HTTP server and etc.
- **Listener** - to model Pub/Sub pattern. You can use Listener step to simulate testing of WebSockets, SSE, RabbitMq, Kafka and etc. Usually for testing Pub/Sub you need a trigger and listener. The trigger will trigger some action and then listener will listen on correspond updates.
- **Pause** - to model pause. You can use pause to simulate micro-batching update, or just wait some time on operation complete.

This is how simple Request step could be defined:
```csharp
// C# example of simple Request step
var reqStep = Step.CreateRequest(name: "simple step", execute: req => Task.FromResult(Response.Ok()))
``` 
```fsharp
// F# example of simple Request step
let reqStep = Step.createRequest("simple step", fun req -> task { return Response.Ok() })
```

### TestFlow
TestFlow is basically a container for steps(you can think of TestFlow like a Job of sequential operations).
```fsharp
type TestFlow = {
    FlowName: string
    Steps: Step[]         // these steps will be executed sequentially, one by one
    ConcurrentCopies: int // specify how many copies of current TestFlow to run in parallel
}
```
This is how simple TestFlow could be defined:
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
**All steps within one TestFlow are executing sequentially**. It helps you model dependently ordered operations. TestFlow allows you to specify how many copies will be run in parallel. For example, if you set ConcurrentCopies = 10, it means that 10 copies of the same TestFlow will be created and started at the same time.

### Scenario
Scenario is a simple abstraction which represent your load test. It contains optional TestInit step which you can define to prepare test environment(load/restore database, clear cache, clean folders and etc). 
```fsharp
type Scenario = {
    ScenarioName: string
    TestInit: Step option // init step will be executed at start of every scenario
    TestFlows: TestFlow[] // each TestFlow will be executed on dedicated System.Threading.Task
    Duration: TimeSpan    // execution time of scenario 
}
```

### Request-response scenario
To create Request step you should use
```fsharp
// for F#
Step.createRequest(name: string, execute: Request -> Task<Response>)
```
```csharp
// for C#
Step.CreateRequest(name: string, execute: Func<Request, Task<Response>>)
```
