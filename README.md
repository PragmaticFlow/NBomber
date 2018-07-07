# NBomber
Very very simple load testing framework for Request-response and Pub/Sub scenarios. It's 100% written in F# and targeting .NET Core and full .NET Framework.

### Supports:
- [x] Request-response scenario
- [ ] Pub/Sub scenario
- [ ] Distibuted scenario (run scenario from several nodes in parallel)

### Why another {x} framework for load testing?
The main reasons are:
 - To be technology agnostic as much as it possible (no dependency on any protocol: HTTP, WebSockets, SSE).
 - To be able to test .NET implementation of specific driver. During testing, it was identified many times that the performance could be slightly different because of the virtual machine(.NET, Java, PHP, Js, Erlang, different settings for GC) or just quality of drivers. For example there ware cases that drivers written in C++ and invoked from NodeJs app worked faster than drivers written in C#/.NET. Therafore it does make sense to load test your app using your runtime.

### What makes it very very simple? 
You need to know only 2 building blocks. The whole API is built around them.
```fsharp
type Step = {
    Name: string
    Execute: unit -> Task
}

type TestFlow = {
    Name: string
    Steps: Step[]
    ConcurrentCopies: int
}

// simple example
let scenario = new ScenarioBuilder(scenarioName: "Test MongoDb")                
                .AddTestFlow("READ Users", steps: new[] { mongoQuery }, concurrentCopies: 10)                
                .Build(interval: TimeSpan.FromSeconds(10));

ScenarioRunner.Run(scenario)

```
