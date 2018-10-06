# Building blocks

The whole API is mainly built around 3 building blocks: Step, TestFlow, Scenario.

```fsharp
// represents single executable Step
// it's a basic element which will be executed and measured
type Step =
    | Request  of RequestStep  // to model Request-response pattern
    | Listener of ListenerStep // to model Pub/Sub pattern
    | Pause    of TimeSpan     // to model pause in your test flow

// represents TestFlow which groups steps and execute them sequentially
// on dedicated System.Threading.Task
type TestFlow = {
    FlowName: string
    Steps: Step[]         // these steps will be executed sequentially, one by one
    ConcurrentCopies: int // specify how many copies of current TestFlow to run in parallel
}

// represents Scenario
type Scenario = {
    ScenarioName: string
    TestInit: Step option // init step will be executed at start of every scenario
    TestFlows: TestFlow[] // each TestFlow will be executed on dedicated System.Threading.Task
    Duration: TimeSpan    // execution time of scenario 
}
```

### Step
Step is a basic element (you can think of Step like a function) which will be executed and measured. 

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
var reqStep = Step.CreateRequest(
    name: "simple step",
    execute: (req) => Task.FromResult(Response.Ok())
);
``` 
```fsharp
// F# example of simple Request step
let reqStep = Step.createRequest("simple step", fun req -> task { return Response.Ok() })
```

### TestFlow
TestFlow is basically a container for steps (you can think of TestFlow like a Job of sequential operations).

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
var testFlow = new TestFlow(
    flowName: "Sequantial flow",
    steps: new [] { authUserStep, buyProductStep, pause_10s, logoutUserStep },
    concurrentCopies: 10
);
```
```fsharp
// F# example of TestFlow with 3 sequential steps
let testFlow = { FlowName = "Sequantial flow"
                 Steps = [authUserStep, buyProductStep, pause_10s, logoutUserStep]
                 ConcurrentCopies = 10 }
```

> **All steps within one TestFlow are executing sequentially**. It helps you model dependently ordered operations. TestFlow allows you to specify how many copies will be run in parallel. For example, if you set ConcurrentCopies = 10, it means that 10 copies of the same TestFlow will be created and started at the same time.

### Scenario
Scenario is a simple abstraction which represent your load test. It contains optional TestInit step which you can define to prepare test environment (load/restore database, clear cache, clean folders and etc). 

```fsharp
type Scenario = {
    ScenarioName: string
    TestInit: Step option // init step will be executed at start of every scenario
    TestFlows: TestFlow[] // each TestFlow will be executed on dedicated System.Threading.Task
    Duration: TimeSpan    // execution time of scenario 
}
```
