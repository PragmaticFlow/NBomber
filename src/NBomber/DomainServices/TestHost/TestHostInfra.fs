namespace NBomber.DomainServices.TestHost.Infra

open System
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Reactive
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Extensions
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Domain.ConnectionPool
open NBomber.Infra
open NBomber.Infra.Dependency

type internal TestSessionArgs = {
    TestInfo: TestInfo
    ScenariosSettings: ScenarioSetting[]
    TargetScenarios: string[]
    ConnectionPoolSettings: ConnectionPoolSetting list
    SendStatsInterval: TimeSpan
}

module internal TestSessionArgs =

    let empty = {
        TestInfo = { SessionId = ""; TestSuite = ""; TestName = "" }
        ScenariosSettings = Array.empty
        TargetScenarios = Array.empty
        ConnectionPoolSettings = List.empty
        SendStatsInterval = TimeSpan.FromSeconds(Constants.MinSendStatsIntervalSec)
    }

    let getFromContext (testInfo: TestInfo, context: TestContext) =
        { TestInfo = testInfo
          ScenariosSettings = TestContext.getScenariosSettings(context)
          TargetScenarios = TestContext.getTargetScenarios(context)
          ConnectionPoolSettings = TestContext.getConnectionPoolSettings(context)
          SendStatsInterval = TestContext.getSendStatsInterval(context) }

    let filterTargetScenarios (sessionArgs: TestSessionArgs) (scns: Scenario list) =
        scns
        |> Scenario.applySettings(sessionArgs.ScenariosSettings)
        |> Scenario.filterTargetScenarios(sessionArgs.TargetScenarios)

module internal TestHostReporting =

    let saveStats (testInfo: TestInfo, nodeStats: RawNodeStats list, sinks: IReportingSink list) =
        nodeStats
        |> List.collect(fun x ->
            let stats = Statistics.create(x)
            sinks |> List.map(fun snk -> snk.SaveRealtimeStats(testInfo, stats))
        )
        |> Task.WhenAll

    let startReportingTimer (dep: GlobalDependency,
                             sessionArgs: TestSessionArgs,
                             getOperationInfo: unit -> (NodeOperationType * TimeSpan),
                             getNodeStats: TimeSpan -> RawNodeStats list) =

        if not (List.isEmpty dep.ReportingSinks) then

            let timer = new System.Timers.Timer(sessionArgs.SendStatsInterval.TotalMilliseconds)
            timer.Elapsed.Add(fun _ ->

                let (operation,operationTime) = getOperationInfo()

                match operation with
                | NodeOperationType.Bombing ->
                    let rawNodeStats = getNodeStats(operationTime)
                    saveStats(sessionArgs.TestInfo, rawNodeStats, dep.ReportingSinks)
                    |> ignore

                | _ -> ()
            )
            timer.Start()
            timer
        else
            new System.Timers.Timer()

module internal TestHostConsole =

    let printTargetScenarios (dep: GlobalDependency, targetScns: Scenario list) =
        targetScns
        |> List.map(fun x -> x.ScenarioName)
        |> fun targets -> dep.Logger.Information("target scenarios: {0}", String.concatWithCommaAndQuotes targets)

    let displayBombingProgress (dep: GlobalDependency, scnSchedulers: ScenarioScheduler list, isWarmUp: bool) =

        let calcTickCount (scn: Scenario) =
            if isWarmUp then int scn.WarmUpDuration.TotalMilliseconds / Constants.NotificationTickIntervalMs
            else int scn.PlanedDuration.TotalMilliseconds / Constants.NotificationTickIntervalMs

        let getLongestDuration (schedulers: ScenarioScheduler list) =
            if isWarmUp then schedulers |> List.map(fun x -> x.Scenario.WarmUpDuration) |> List.sortDescending |> List.head
            else schedulers |> List.map(fun x -> x.Scenario.PlanedDuration) |> List.sortDescending |> List.head

        let displayProgressForConcurrentScenarios (schedulers: ScenarioScheduler list) =
            let mainPb = schedulers |> getLongestDuration |> dep.CreateAutoProgressBar

            schedulers
            |> List.map(fun scheduler ->
                let tickCount = calcTickCount(scheduler.Scenario)
                let pb = mainPb.Spawn(tickCount, "")

                scheduler.EventStream
                |> Observable.subscribeWithCompletion
                    (fun x -> let msg = sprintf "scenario: '%s', keep concurrent: '%i', inject per sec: '%i'" scheduler.Scenario.ScenarioName x.ConstantActorCount x.OneTimeActorCount
                              pb.Tick(msg))
                    (fun () -> pb.Dispose())
            )
            |> List.append [mainPb :> IDisposable]

        let displayProgressForOneScenario (scheduler: ScenarioScheduler) =
            let pb = scheduler.Scenario |> calcTickCount |> dep.CreateManualProgressBar
            scheduler.EventStream
            |> Observable.subscribeWithCompletion
                (fun x -> let msg = sprintf "keep concurrent: '%i', inject per sec: '%i'" x.ConstantActorCount x.OneTimeActorCount
                          pb.Tick(msg))

                (fun () -> pb.Dispose())

        match dep.ApplicationType with
        | ApplicationType.Console ->
            if scnSchedulers.Length > 1 then
                displayProgressForConcurrentScenarios(scnSchedulers)
            else
                [displayProgressForOneScenario(scnSchedulers.Head)]
        | _ -> List.empty

    let displayConnectionPoolProgress (dep: GlobalDependency, pool: ConnectionPool) =
        match dep.ApplicationType with
        | ApplicationType.Console ->
            let pb = dep.CreateManualProgressBar(pool.ConnectionCount)
            pool.EventStream
            |> Observable.subscribeWithCompletion
                (fun event ->
                    match event with
                    | StartedInit poolName -> pb.Message <- sprintf "opening connections for connection pool: '%s'" poolName
                    | StartedStop poolName -> pb.Message <- sprintf "closing connections for connection pool: '%s'" poolName

                    | ConnectionOpened (poolName,number) ->
                        pb.Tick(sprintf "opened connection '%i' for connection pool: '%s'" number poolName)

                    | ConnectionClosed error ->
                        pb.Tick()
                        if error.IsSome then dep.Logger.Error(error.Value.ToString(), "close connection exception")

                    | InitFinished
                    | InitFailed -> pb.Dispose()
                )
                (fun _ -> pb.Dispose())

            |> Some

        | _ -> None

module internal TestHostScenario =

    let initConnectionPools (dep: GlobalDependency) (token: CancellationToken) (pools: ConnectionPool list) =
        asyncResult {
            for pool in pools do
                let subscription = TestHostConsole.displayConnectionPoolProgress(dep, pool)
                do! pool.Init(token)
                if subscription.IsSome then subscription.Value.Dispose()

            return pools
        }

    let destroyConnectionPools (dep: GlobalDependency) (token: CancellationToken) (pools: ConnectionPool list) =
        for pool in pools do
            let subscription = TestHostConsole.displayConnectionPoolProgress(dep, pool)
            pool.Destroy(token)
            if subscription.IsSome then subscription.Value.Dispose()

    let initScenarios (dep: GlobalDependency,
                       defaultScnContext: ScenarioContext,
                       registeredScenarios: Scenario list,
                       sessionArgs: TestSessionArgs) = asyncResult {

        let tryInitScenario (context: ScenarioContext, initFunc: ScenarioContext -> Task) =
            try
                initFunc(context).Wait()
                Ok()
            with
            | ex -> Error ex

        let rec init (scenarios) = seq {
            match scenarios with
            | scn :: tail ->
                match scn.TestInit with
                | Some initFunc ->
                    dep.Logger.Information("start init scenario: '{Scenario}'", scn.ScenarioName)

                    let context = { defaultScnContext with CustomSettings = scn.CustomSettings }
                    match tryInitScenario(context, initFunc) with
                    | Ok _     -> yield! init(tail)
                    | Error ex -> Error ex

                | None -> Ok()
            | [] -> Ok()
        }

        let targetScns = registeredScenarios |> TestSessionArgs.filterTargetScenarios sessionArgs
        TestHostConsole.printTargetScenarios(dep, targetScns)
        do! targetScns |> init |> Result.toEmptyIO

        let! pools = targetScns
                     |> Scenario.filterDistinctConnectionPoolsArgs
                     |> Scenario.applyConnectionPoolSettings(sessionArgs.ConnectionPoolSettings)
                     |> List.map(fun poolArgs -> new ConnectionPool(poolArgs))
                     |> initConnectionPools dep defaultScnContext.CancellationToken

        let scenariosWithPools = targetScns |> Scenario.insertConnectionPools(pools)
        return scenariosWithPools
    }

    let cleanScenarios (dep: GlobalDependency, defaultScnContext: ScenarioContext, scenarios: Scenario list) =
        scenarios
        |> Scenario.filterDistinctConnectionPools
        |> destroyConnectionPools dep defaultScnContext.CancellationToken

        let tryClean (context: ScenarioContext, cleanFunc: ScenarioContext -> Task) =
            try
                cleanFunc(context).Wait()
            with
            | ex -> dep.Logger.Error(ex.ToString(), "clean scenario failed")

        for s in scenarios do
            match s.TestClean with
            | Some cleanFunc ->
                dep.Logger.Information("start clean scenario: '{Scenario}'", s.ScenarioName)
                let context = { defaultScnContext with CustomSettings = s.CustomSettings }
                tryClean(context, cleanFunc)

            | None -> ()
