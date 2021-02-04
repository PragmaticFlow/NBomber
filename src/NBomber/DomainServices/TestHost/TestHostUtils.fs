namespace NBomber.DomainServices.TestHost

open System
open System.Threading.Tasks

open FSharp.Control.Reactive
open FSharp.Control.Tasks.NonAffine
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Domain.ConnectionPool
open NBomber.Domain.DomainTypes
open NBomber.DomainServices.NBomberContext
open NBomber.Errors
open NBomber.Extensions.InternalExtensions
open NBomber.Infra
open NBomber.Infra.ProgressBar
open NBomber.Infra.Dependency

module internal TestHostReporting =

    let saveRealtimeStats (sinks: IReportingSink list) (nodeStats: NodeStats list) =
        sinks
        |> List.map(fun x -> nodeStats |> List.toArray |> x.SaveStats)
        |> Task.WhenAll

    let saveFinalStats (dep: IGlobalDependency) (stats: NodeStats list) = task {
        for sink in dep.ReportingSinks do
            try
                do! sink.SaveStats(stats |> Seq.toArray)
            with
            | ex -> dep.Logger.Warning(ex, "Reporting sink '{SinkName}' failed to save stats.", sink.SinkName)
    }

    let startReportingTimer (dep: IGlobalDependency,
                             sendStatsInterval: TimeSpan,
                             getData: unit -> (NodeOperationType * NodeStats)) =

            let timer = new System.Timers.Timer(sendStatsInterval.TotalMilliseconds)
            timer.Elapsed.Add(fun _ ->

                let (operation,nodeStats) = getData()

                match operation with
                | NodeOperationType.Bombing ->
                    if not (List.isEmpty dep.ReportingSinks) then
                        nodeStats
                        |> List.singleton
                        |> saveRealtimeStats dep.ReportingSinks
                        |> ignore

                | _ -> ()
            )
            timer.Start()
            timer

    let initReportingSinks (dep: IGlobalDependency) (context: IBaseContext) = taskResult {
        try
            for sink in dep.ReportingSinks do
                dep.Logger.Information("Start init reporting sink: '{SinkName}'.", sink.SinkName)
                do! sink.Init(context, dep.InfraConfig |> Option.defaultValue Constants.EmptyInfraConfig)
        with
        | ex -> return! AppError.createResult(InitScenarioError ex)
    }

    let startReportingSinks (dep: IGlobalDependency) = task {
        for sink in dep.ReportingSinks do
            try
                sink.Start() |> ignore
            with
            | ex -> dep.Logger.Warning(ex, "Failed to start reporting sink '{SinkName}'.", sink.SinkName)
    }

    let stopReportingSinks (dep: IGlobalDependency) = task {
        for sink in dep.ReportingSinks do
            try
                dep.Logger.Information("Stop reporting sink: '{SinkName}'.", sink.SinkName)
                do! sink.Stop()
            with
            | ex -> dep.Logger.Warning(ex, "Stop reporting sink '{SinkName}' failed.", sink.SinkName)
    }

module internal TestHostPlugins =

    let initPlugins (dep: IGlobalDependency) (context: IBaseContext) = taskResult {
        try
            for plugin in dep.WorkerPlugins do
                dep.Logger.Information("Start init plugin: '{PluginName}'.", plugin.PluginName)
                do! plugin.Init(context, dep.InfraConfig |> Option.defaultValue Constants.EmptyInfraConfig)
        with
        | ex -> return! AppError.createResult(InitScenarioError ex)
    }

    let startPlugins (dep: IGlobalDependency) = task {
        for plugin in dep.WorkerPlugins do
            try
                plugin.Start() |> ignore
            with
            | ex -> dep.Logger.Warning(ex, "Failed to start plugin '{PluginName}'.", plugin.PluginName)
    }

    let stopPlugins (dep: IGlobalDependency) = task {
        for plugin in dep.WorkerPlugins do
            try
                dep.Logger.Information("Stop plugin: '{PluginName}'.", plugin.PluginName)
                do! plugin.Stop()
            with
            | ex -> dep.Logger.Warning(ex, "Stop plugin '{PluginName}' failed.", plugin.PluginName)
    }

module internal TestHostConsole =

    let printTargetScenarios (dep: IGlobalDependency) (targetScns: Scenario list) =
        targetScns
        |> List.map(fun x -> x.ScenarioName)
        |> fun targets -> dep.Logger.Information("Target scenarios: {0}.", String.concatWithCommaAndQuotes targets)

    let createDescription (scenarioIndex: int option) (simulation: LoadSimulation) (simulationValue: int) =
        let scenarioName =
            scenarioIndex
            |> Option.map(fun i -> $"[{i}]" |> Console.escapeMarkup)
            |> Option.defaultValue String.Empty

        let copies = if simulationValue = 1 then "copy" else "copies"

        match simulation with
        | RampConstant _        ->
            $"{scenarioName} ramp {simulationValue |> Console.highlightInfo} {copies}"
        | KeepConstant _        ->
            $"{scenarioName} keep {simulationValue |> Console.highlightInfo} {copies}"
        | RampPerSec _          ->
            $"{scenarioName} ramp {simulationValue |> Console.highlightInfo} {copies}/sec"
        | InjectPerSec _        ->
            $"{scenarioName} inject {simulationValue |> Console.highlightInfo} {copies}/sec"
        | InjectPerSecRandom _  ->
            $"{scenarioName} inject {simulationValue |> Console.highlightInfo} {copies}/sec rand"

    let createDescriptionForProgressInfo (scenarioIndex: int option) (progressInfo: ScenarioProgressInfo) =
        match progressInfo.CurrentSimulation with
        | RampConstant _
        | KeepConstant _ -> progressInfo.ConstantActorCount

        | RampPerSec _
        | InjectPerSec _
        | InjectPerSecRandom _ -> progressInfo.OneTimeActorCount
        |> createDescription scenarioIndex progressInfo.CurrentSimulation

    let createDescriptionForConnectionPool =
        function
        | StartedInit name          -> Some $"{name |> Console.highlight}: opening connection"
        | StartedStop name          -> Some $"{name |> Console.highlight}: closing connection"
        | ConnectionOpened (name,i) -> Some $"{name |> Console.highlight}: opened connection {i |> Console.highlight}"
        | ConnectionClosed _
        | InitFinished _
        | InitFailed                -> None

    let displayBombingProgress (dep: IGlobalDependency, scnSchedulers: ScenarioScheduler list, isWarmUp: bool) =

        let calcTickCount (scn: Scenario) =
            if isWarmUp then int(scn.WarmUpDuration.TotalMilliseconds / Constants.SchedulerNotificationTickInterval.TotalMilliseconds)
            else int(scn.PlanedDuration.TotalMilliseconds / Constants.SchedulerNotificationTickInterval.TotalMilliseconds)

        let calcTotalTickCount (schedulers: ScenarioScheduler list) =
            schedulers |> Seq.map(fun scheduler -> scheduler.Scenario) |> Seq.map calcTickCount |> Seq.sum
            
        let displayProgressForConcurrentScenarios (schedulers: ScenarioScheduler list) =
            schedulers
            |> List.mapi(fun i scheduler ->
                let simulation = scheduler.Scenario.LoadTimeLine |> Seq.head |> fun segment -> segment.LoadSimulation
                let scenarioIndex = Some(i + 1)
                let description = createDescription scenarioIndex simulation 0
                let ticks = scheduler.Scenario |> calcTickCount |> float
                { Description = description; Ticks = ticks }
            )
            |> List.append [{ Description = "All Scenarios"; Ticks = schedulers |> calcTotalTickCount |> float }]
            |> ProgressBar.create ProgressBar.defaultColumns
               (fun tasks ->
                    let totalTask = tasks |> Seq.head

                    tasks
                    |> Seq.iteri(fun i task ->
                        if i > 0 then
                            schedulers.[i - 1].EventStream
                            |> Observable.subscribe(fun progressInfo ->
                                let scenarioIndex = Some i
                                progressInfo |> createDescriptionForProgressInfo scenarioIndex |> Some |> ProgressBar.tick task |> ignore
                                None |> ProgressBar.tick totalTask |> ignore
                            )
                            |> ignore
                    )
               )

        let displayProgressForOneScenario (scheduler: ScenarioScheduler) =
            let simulation = scheduler.Scenario.LoadTimeLine |> Seq.head |> fun segment -> segment.LoadSimulation
            let scenarioIndex = None
            let description = createDescription scenarioIndex simulation 0
            let ticks = scheduler.Scenario |> calcTickCount |> float

            { Description = description; Ticks = ticks }
            |> List.singleton
            |> ProgressBar.create ProgressBar.defaultColumns
               (fun tasks ->
                    let task = tasks |> Seq.head

                    scheduler.EventStream
                    |> Observable.subscribe(fun progressInfo ->
                        progressInfo |> createDescriptionForProgressInfo None |> Some |> ProgressBar.tick task |> ignore
                    )
                    |> ignore
               )

        match dep.ApplicationType with
        | ApplicationType.Console ->
            if scnSchedulers.Length > 1 then
                displayProgressForConcurrentScenarios(scnSchedulers) |> ignore
            else
                displayProgressForOneScenario(scnSchedulers.Head) |> ignore
        | _ -> ()

    let displayConnectionPoolsProgress (dep: IGlobalDependency, pools: ConnectionPool list) =
        match dep.ApplicationType with
        | ApplicationType.Console ->
            pools
            |> List.map(fun pool -> { Description = pool.PoolName; Ticks = pool.ConnectionCount |> float })
            |> ProgressBar.create ProgressBar.defaultColumns
               (fun tasks ->
                    tasks
                    |> Seq.iteri(fun i task ->
                        pools.[i].EventStream
                        |> Observable.subscribe(fun event ->
                            let description = createDescriptionForConnectionPool(event)

                            match event with
                            | StartedInit _
                            | StartedStop _ ->
                                ProgressBar.setDescription task description |> ignore

                            | ConnectionOpened _ ->
                                ProgressBar.tick task description |> ignore
  
                            | ConnectionClosed error ->
                                ProgressBar.tick task description |> ignore
                                if error.IsSome then dep.Logger.Error(error.Value.ToString(), "Close connection exception.")

                            | InitFinished
                            | InitFailed -> ()
                        )
                        |> ignore
                    )
               )

        | _ -> Task.FromResult()

    let printContextInfo (dep: IGlobalDependency) =
        dep.Logger.Verbose("NBomberConfig: {NBomberConfig}", sprintf "%A" dep.NBomberConfig)

        if dep.WorkerPlugins.IsEmpty then
            dep.Logger.Information("Plugins: no plugins were loaded.")
        else
            dep.WorkerPlugins
            |> List.iter(fun plugin -> dep.Logger.Information("Plugins: '{PluginName}' loaded.", plugin.PluginName))

        if dep.ReportingSinks.IsEmpty then
            dep.Logger.Information("Reporting sinks: no reporting sinks were loaded.")
        else
            dep.ReportingSinks
            |> List.iter(fun sink -> dep.Logger.Information("Reporting sinks: '{SinkName}' loaded.", sink.SinkName))

module internal TestHostScenario =

    let getTargetScenarios (sessionArgs: SessionArgs) (registeredScenarios: Scenario list) =
        registeredScenarios
        |> Scenario.filterTargetScenarios sessionArgs.TargetScenarios
        |> Scenario.applySettings sessionArgs.ScenariosSettings

    let initConnectionPools (dep: IGlobalDependency) (context: IBaseContext) (pools: ConnectionPool list) = taskResult {
        try
            pools
            |> Seq.iter(fun pool ->
                dep.Logger.Information("Start opening {ConnectionCount} connections for connection pool: '{PoolName}'.", pool.ConnectionCount, pool.PoolName)
            )

            let progressTask = TestHostConsole.displayConnectionPoolsProgress(dep, pools)

            for pool in pools do
                do! pool.Init(context) |> TaskResult.mapError(InitScenarioError >> AppError.create)

            progressTask.Wait()

            return pools
        with
        | ex -> return! AppError.createResult(InitScenarioError ex)
    }

    let initDataFeeds (dep: IGlobalDependency) (context: IBaseContext) (feeds: IFeed<obj> list) = taskResult {
        try
            for feed in feeds do
                do! feed.Init(context)
                dep.Logger.Information("Initialized data feed: '{FeedName}'.", feed.FeedName)

            return feeds
        with
        | ex -> return! AppError.createResult(InitScenarioError ex)
    }

    let initScenarios (dep: IGlobalDependency)
                      (baseContext: IBaseContext)
                      (defaultScnContext: IScenarioContext)
                      (sessionArgs: SessionArgs)
                      (targetScenarios: Scenario list) = taskResult {
        try
            TestHostConsole.printTargetScenarios dep targetScenarios

            // scenario init
            for scn in targetScenarios do
                match scn.Init with
                | Some initFunc ->

                    dep.Logger.Information("Start init scenario: '{Scenario}'.", scn.ScenarioName)
                    let scnContext = Scenario.ScenarioContext.setCustomSettings defaultScnContext scn.CustomSettings
                    do! initFunc scnContext

                | None -> ()

            // connection pools init
            let! pools =
                targetScenarios
                |> Scenario.ConnectionPool.createConnectionPools sessionArgs.ConnectionPoolSettings
                |> initConnectionPools dep baseContext

            // data feed init
            do! targetScenarios
                |> Scenario.Feed.filterDistinctAndEmptyFeeds
                |> initDataFeeds dep baseContext
                |> TaskResult.ignore

            return targetScenarios
                   |> Scenario.ConnectionPool.setConnectionPools pools
        with
        | ex -> return! AppError.createResult(InitScenarioError ex)
    }

    let cleanScenarios (dep: IGlobalDependency)
                       (baseContext: IBaseContext)
                       (defaultScnContext: IScenarioContext)
                       (scenarios: Scenario list) = task {

        let destroyConnectionPools (dep: IGlobalDependency) (context: IBaseContext) (pools: ConnectionPool list) =
            pools
            |> Seq.iter(fun pool ->
                dep.Logger.Information("Start closing {ConnectionCount} connections for connection pool: '{PoolName}'.", pool.ConnectionCount, pool.PoolName)
            )

            let progressTask = TestHostConsole.displayConnectionPoolsProgress(dep, pools)

            for pool in pools do
                pool.Destroy(context).Wait()

            progressTask.Wait()

        scenarios
        |> Scenario.ConnectionPool.filterDistinctConnectionPools
        |> destroyConnectionPools dep baseContext

        for scn in scenarios do
            match scn.Clean with
            | Some cleanFunc ->
                dep.Logger.Information("Start clean scenario: '{Scenario}'.", scn.ScenarioName)

                let context = Scenario.ScenarioContext.setCustomSettings defaultScnContext scn.CustomSettings
                try
                    do! cleanFunc context
                with
                | ex -> dep.Logger.Warning(ex, "Clean scenario: '{Scenario}' failed.", scn.ScenarioName)

            | None -> ()
    }
