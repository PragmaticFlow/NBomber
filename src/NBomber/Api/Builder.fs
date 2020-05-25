[<AutoOpen>]
module NBomber.FSharp.Builder

open System
open System.Threading.Tasks
open FSharp.Control.Tasks.V2.ContextInsensitive
open NBomber.Contracts
open NBomber.FSharp


/// scenario builder
type ScenarioBuilder(name : string) =
    let empty = Scenario.create name []
    let addTo scenario step =
        { scenario with Steps = [ step ] |> List.append scenario.Steps }
    let unitTask (init : ScenarioContext -> Task) ctx =
        task {
            do! init ctx
            return ()
        }
    member _.Zero() = empty
    member _.Yield _ = empty
    member _.Run f = f

    /// create step to scenario
    [<CustomOperation "stepSimple">]
    member _.StepSimple(scenario : Scenario, stepName : string, action : IStepContext<_,_> -> Task<Response>) =
        Step.create(stepName, action) |> addTo scenario

    member _.StepSimple(scenario : Scenario, stepName : string, action : IStepContext<_,_> -> Task) =
        let withResponse ctx =
            task {
                do! action ctx
                return Response.Ok()
            }
        Step.create(stepName, withResponse) |> addTo scenario

    member _.StepSimple(scenario : Scenario, stepName : string, action : IStepContext<_,_> -> Task<unit>) =
        let withResponse ctx =
            task {
                do! action ctx
                return Response.Ok()
            }
        Step.create(stepName, withResponse) |> addTo scenario

    member _.StepSimple(scenario : Scenario, stepName : string, action : IStepContext<_,_> -> Async<Response>) =
        Step.create(stepName, action >> Async.StartAsTask) |> addTo scenario

    member _.StepSimple(scenario : Scenario, stepName : string, action : IStepContext<_,_> -> Async<unit>) =
        let withResponse ctx =
            async {
                do! action ctx
                return Response.Ok()
            }
        Step.create(stepName, withResponse >> Async.StartAsTask) |> addTo scenario

    /// create step with connection pool
    [<CustomOperation "step">]
    member _.Step(scenario : Scenario, stepName : string, connectionPool : IConnectionPoolArgs<_>, action : IStepContext<_,_> -> Task<Response>) =
        Step.create(stepName, connectionPool, action) |> addTo scenario

    member _.Step(scenario : Scenario, stepName : string, connectionPool : IConnectionPoolArgs<_>, action : IStepContext<_,_> -> Task<unit>) =
        let withResponse ctx =
            task {
                do! action ctx
                return Response.Ok()
            }
        Step.create(stepName, connectionPool, withResponse) |> addTo scenario

    member _.Step(scenario : Scenario, stepName : string, connectionPool : IConnectionPoolArgs<_>, action : IStepContext<_,_> -> Task) =
        let withResponse ctx =
            task {
                do! action ctx
                return Response.Ok()
            }
        Step.create(stepName, connectionPool, withResponse) |> addTo scenario

    member _.Step(scenario : Scenario, stepName : string, connectionPool : IConnectionPoolArgs<_>, action : IStepContext<_,_> -> Async<Response>) =
        Step.create(stepName, connectionPool, action >> Async.StartAsTask) |> addTo scenario

    member _.Step(scenario : Scenario, stepName : string, connectionPool : IConnectionPoolArgs<_>, action : IStepContext<_,_> -> Async<unit>) =
        let withResponse ctx =
            async {
                do! action ctx
                return Response.Ok()
            }
        Step.create(stepName, connectionPool, withResponse >> Async.StartAsTask) |> addTo scenario

    /// create not tracked pause step
    [<CustomOperation "pause">]
    member _.Pause(scenario: Scenario, millis : int) =
        Step.createPause millis |> addTo scenario

    member _.Pause(scenario: Scenario, getMillis : unit -> int) =
        Step.createPause getMillis |> addTo scenario

    member _.Pause(scenario: Scenario, timeSpan : TimeSpan) =
        Step.createPause timeSpan |> addTo scenario

    member _.Pause(scenario: Scenario, getTimeSpan : unit -> TimeSpan) =
        Step.createPause getTimeSpan |> addTo scenario

    /// set warmup duration
    [<CustomOperation "warmUp">]
    member _.WarmUp(scenario: Scenario, time) =
        Scenario.withWarmUpDuration time scenario

    /// run without warmup
    [<CustomOperation "noWarmUp">]
    member _.NoWarmUp(scenario: Scenario) =
        Scenario.withWarmUpDuration TimeSpan.Zero scenario

    /// setup load simulation
    [<CustomOperation "load">]
    member _.Load(scenario: Scenario, simulations) =
        Scenario.withLoadSimulations simulations scenario

    /// run an action before test
    [<CustomOperation "testInit">]
    member _.TestInit(scenario: Scenario, init) =
        Scenario.withTestInit init scenario
    member _.TestInit(scenario: Scenario, init : ScenarioContext -> Task) =
        Scenario.withTestInit (unitTask init) scenario

    /// run an ation after test
    [<CustomOperation "testClean">]
    member _.TestClean(scenario: Scenario, clean) =
        Scenario.withTestClean clean scenario
    member _.TestClean(scenario: Scenario, clean : ScenarioContext -> Task) =
        Scenario.withTestClean (unitTask clean) scenario

let scenario name = ScenarioBuilder name


type ReportBuilder() =
    member _.Yield _ = ReporterConfig.Default

    [<CustomOperation "fileName">]
    member _.ReportFileName(ctx, fileName) =
        { ctx with FileName = Some fileName }

    [<CustomOperation "interval">]
    member _.Interval(ctx : ReporterConfig, interval) =
        { ctx with SendStatsInterval = interval }

    [<CustomOperation "formats">]
    member _.Formats(ctx, formats) =
        { ctx with Formats = formats }

    [<CustomOperation "sink">]
    member _.Report(ctx, reporter) =
        { ctx with Sinks = reporter::ctx.Sinks }

let report = ReportBuilder()


/// performance test builder
type RunnerBuilder() =
    let empty = NBomberRunner.registerScenarios []

    member _.Zero() = empty

    member _.Yield _ = empty

    member _.Run f = f

    [<CustomOperation "scenarios">]
    member _.Scenarios(ctx, scenarios) =
        { ctx with RegisteredScenarios = scenarios }


    [<CustomOperation "report">]
    member _.Report(ctx, report) =
        { ctx with Report = Some report }

    [<CustomOperation "noReport">]
    member _.NoReports(ctx) =
        { ctx with Report = None }

    [<CustomOperation "testSuite">]
    member _.TestSuite(ctx : NBomberContext, name) =
        { ctx with TestSuite = name }

    [<CustomOperation "name">]
    member _.Name(ctx : NBomberContext, name) =
        printfn "runner name %s" name
        { ctx with TestName = name }

    [<CustomOperation "config">]
    member _.Config(ctx, path) =
        NBomberRunner.loadConfig path ctx

    [<CustomOperation "infraConfig">]
    member _.ConfigInfrastructure(ctx, path) =
        NBomberRunner.loadInfraConfig path ctx

    [<CustomOperation "plugins">]
    member _.Reports(ctx, plugins) =
        { ctx with Plugins = plugins }

    [<CustomOperation "runProcess">]
    member _.ApplicationTypeProcess(ctx) =
        { ctx with ApplicationType = Some ApplicationType.Process }

    [<CustomOperation "runConsole">]
    member _.ApplicationTypeConsole(ctx) =
        { ctx with ApplicationType = Some ApplicationType.Console }

    [<CustomOperation "applicationType">]
    member _.ApplicationType(ctx, application) =
        { ctx with ApplicationType = Some application }

let perftest = RunnerBuilder()
