namespace NBomber.DomainServices.TestHost

open System
open System.Threading.Tasks

open FSharp.Control.Reactive
open FsToolkit.ErrorHandling
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Errors
open NBomber.Contracts
open NBomber.Extensions.InternalExtensions
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Domain.ConnectionPool
open NBomber.Infra.Dependency
open NBomber.DomainServices.NBomberContext

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

    let createReportingTimer (dep: IGlobalDependency,
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

    let displayBombingProgress (dep: IGlobalDependency, scnSchedulers: ScenarioScheduler list, isWarmUp: bool) =

        let calcTickCount (scn: Scenario) =
            if isWarmUp then int(scn.WarmUpDuration.TotalMilliseconds / Constants.SchedulerNotificationTickInterval.TotalMilliseconds)
            else int(scn.PlanedDuration.TotalMilliseconds / Constants.SchedulerNotificationTickInterval.TotalMilliseconds)

        let getLongestDuration (schedulers: ScenarioScheduler list) =
            if isWarmUp then schedulers |> List.map(fun x -> x.Scenario.WarmUpDuration) |> List.max
            else schedulers |> List.map(fun x -> x.Scenario.PlanedDuration) |> List.max

        let displayProgressForConcurrentScenarios (schedulers: ScenarioScheduler list) =
            let mainPb = schedulers |> getLongestDuration |> dep.ProgressBarEnv.CreateAutoProgressBar

            schedulers
            |> List.map(fun scheduler ->
                let tickCount = calcTickCount(scheduler.Scenario)
                let pb = mainPb.Spawn(tickCount, "")

                scheduler.EventStream
                |> Observable.subscribeWithCompletion
                    (fun x -> let simulationName = LoadTimeLine.getSimulationName(x.CurrentSimulation)
                              let msg = String.Format("Scenario: '{0}', simulation: '{1}', keep concurrent: '{2}', inject per sec: '{3}'.",
                                                      scheduler.Scenario.ScenarioName, simulationName, x.ConstantActorCount, x.OneTimeActorCount)
                              pb.Tick(msg))
                    (fun () -> pb.Dispose())
            )
            |> List.append [mainPb :> IDisposable]

        let displayProgressForOneScenario (scheduler: ScenarioScheduler) =
            let pb = scheduler.Scenario |> calcTickCount |> dep.ProgressBarEnv.CreateManualProgressBar
            scheduler.EventStream
            |> Observable.subscribeWithCompletion
                (fun x -> let simulationName = LoadTimeLine.getSimulationName(x.CurrentSimulation)
                          let msg =
                              match x.CurrentSimulation with
                              | RampConstant _
                              | KeepConstant _ -> String.Format("Simulation: '{0}', copies: '{1}'.", simulationName, x.ConstantActorCount)
                              | RampPerSec _
                              | InjectPerSec _ -> String.Format("Simulation: '{0}', rate: '{1}'.", simulationName, x.OneTimeActorCount)
                              | InjectPerSecRandom _ -> String.Format("Simulation: '{0}', rate: '{1}'.", simulationName, x.OneTimeActorCount)

                          pb.Tick(msg) )

                (fun () -> pb.Dispose())

        match dep.ApplicationType with
        | ApplicationType.Console ->
            if scnSchedulers.Length > 1 then
                displayProgressForConcurrentScenarios(scnSchedulers)
            else
                [displayProgressForOneScenario(scnSchedulers.Head)]
        | _ -> List.empty

    let displayConnectionPoolProgress (dep: IGlobalDependency, pool: ConnectionPool) =
        match dep.ApplicationType with
        | ApplicationType.Console ->
            dep.Logger.Information("Start open '{ConnectionCount}' connections for connection pool: '{PoolName}'.", pool.ConnectionCount, pool.PoolName)
            let pb = dep.ProgressBarEnv.CreateManualProgressBar(pool.ConnectionCount)
            pool.EventStream
            |> Observable.subscribeWithCompletion
                (fun event ->
                    match event with
                    | StartedInit poolName -> pb.Message <- sprintf "Opening connections for connection pool: '%s'." poolName
                    | StartedStop poolName -> pb.Message <- sprintf "Closing connections for connection pool: '%s'." poolName

                    | ConnectionOpened (poolName,number) ->
                        pb.Tick(sprintf "Opened connection '%i' for connection pool: '%s'." number poolName)

                    | ConnectionClosed error ->
                        pb.Tick()
                        if error.IsSome then dep.Logger.Error(error.Value.ToString(), "Close connection exception.")

                    | InitFinished
                    | InitFailed -> pb.Dispose()
                )
                (fun _ -> pb.Dispose())

            |> Some

        | _ -> None

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
            for pool in pools do
                let subscription = TestHostConsole.displayConnectionPoolProgress(dep, pool)
                do! pool.Init(context) |> TaskResult.mapError(InitScenarioError >> AppError.create)
                if subscription.IsSome then subscription.Value.Dispose()

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
            for pool in pools do
                let subscription = TestHostConsole.displayConnectionPoolProgress(dep, pool)
                pool.Destroy(context).Wait()
                if subscription.IsSome then subscription.Value.Dispose()

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
