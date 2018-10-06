# Introduction

### Step 1. Installation
To install NBomber via NuGet, run this command in NuGet package manager console:
```code
PM> Install-Package PragmaticFlow.NBomber
```

### Step 2. Design and run a load test scenario
```csharp
// C# example
// to create Request step you should use
// Step.CreateRequest(name: string, execute: Func<Request, Task<Response>>)
var myStep = Step.CreateRequest("My Step", execute: async (req) =>
{
    // you can do any logic here: go to http, websocket etc
    await Task.Delay(TimeSpan.FromSeconds(1));
    return Response.Ok();
});

// after creating a step you should add it to TestFlow and then to Scenario.
// for this you can use:
var scenario = new ScenarioBuilder(scenarioName: "My Scenario")
                .AddTestFlow("My TestFlow", steps: new[] { myStep }, concurrentCopies: 10)
                .Build(duration: TimeSpan.FromSeconds(10));

// run scenario
scenario.RunInConsole();
```
The scenario.RunInConsole() call runs your test scenario and print results to console output.

### Step 3. View results
View the results. Here is an example of console output from the above benchmark:
```
Scenario: My Scenario, execution time: 00:00:10
 -----------------------------------------------
 | flow 1: My TestFlow | concurrent copies: 10 |
 -----------------------------------------------
+---------------+----+--------+-----+------+------+------+------+------+------+--------
| request_count | OK | failed | RPS | min  | mean | max  | 50%  | 75%  | 95%  | StdDev 
+---------------+----+--------+-----+------+------+------+------+------+------+--------
| 90            | 90 | 0      | 9   | 1000 | 1010 | 1016 | 1011 | 1014 | 1015 | 5      
+---------------+----+--------+-----+------+------+------+------+------+------+--------
```

### Step 4. Analyze results
In your bin directory, you can find a folder 'results' with *.txt, *.html reports.

### Step 5. Integrate load test in your CI/CD pipline
If you decided to add load test in your CI/CD pipline NBomber provides an integration with:
- XUnit
- NUnit

### Next steps
NBomber provides a lot of features which help to load test any system. If you want to know more about NBomber features, checkout the Overview page. If you want have any questions, checkout the FAQ page. If you didn't find answer for your question on this page, ask it on gitter or create an issue.