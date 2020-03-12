namespace NBomber.DomainServices.TestHost.Infra

open System
open System.Threading.Tasks

open FSharp.Control.Reactive
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Extensions
open NBomber.Domain
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Domain.ConnectionPool
open NBomber.Infra
open NBomber.Infra.Dependency

type internal TestSessionArgs = {
    TestInfo: TestInfo
    ScenariosSettings: ScenarioSetting[]
    TargetScenarios: string[]
    CustomSettings: string
    SendStatsInterval: TimeSpan
}

module internal TestSessionArgs =

    let empty = {
        TestInfo = { SessionId = ""; TestSuite = ""; TestName = "" }
        ScenariosSettings = Array.empty
        TargetScenarios = Array.empty
        CustomSettings = ""
        SendStatsInterval = TimeSpan.FromSeconds(Constants.MinSendStatsIntervalSec)
    }

    let getFromContext (testInfo: TestInfo, context: TestContext) =
        let scnSettings = TestContext.getScenariosSettings(context)
        let targetScns = TestContext.getTargetScenarios(context)
        let customSettings = TestContext.getCustomSettings(context)
        let statsInterval = TestContext.getSendStatsInterval(context)
        { TestInfo = testInfo
          ScenariosSettings = scnSettings
          TargetScenarios = targetScns
          CustomSettings = customSettings
          SendStatsInterval = statsInterval }

    let filterTargetScenarios (scns: Scenario[], sessionArgs: TestSessionArgs) =
        scns
        |> Scenario.applySettings(sessionArgs.ScenariosSettings)
        |> Scenario.filterTargetScenarios(sessionArgs.TargetScenarios)

module internal TestHostReporting =

    let saveStats (testInfo: TestInfo, nodeStats: RawNodeStats[], sinks: IReportingSink[]) =
        nodeStats
        |> Array.collect(fun x ->
            let stats = Statistics.create(x)
            sinks |> Array.map(fun snk -> snk.SaveRealtimeStats(testInfo, stats))
        )
        |> Task.WhenAll

    let startReportingTimer (dep: GlobalDependency,
                             sessionArgs: TestSessionArgs,
                             getOperationInfo: unit -> (NodeOperationType * TimeSpan),
                             getNodeStats: TimeSpan -> RawNodeStats[]) =

        if not (Array.isEmpty dep.ReportingSinks) then

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
        |> fun targets -> dep.Logger.Information("target scenarios: {0}", String.concatWithCommaAndQuotes(targets))

    let displayBombingProgress (dep: GlobalDependency, scnSchedulers: ScenarioScheduler[], isWarmUp: bool) =
        match dep.ApplicationType with
        | ApplicationType.Console ->
            scnSchedulers
            |> Array.map(fun x ->

                let tickCount =
                    if isWarmUp then int x.Scenario.WarmUpDuration.TotalMilliseconds / Constants.NotificationTickIntervalMs
                    else int x.Scenario.Duration.TotalMilliseconds / Constants.NotificationTickIntervalMs

                let pb = dep.CreateProgressBar(tickCount)
                x.EventStream
                |> Observable.subscribeWithCompletion
                    (fun x -> let msg = sprintf "keep concurrent scenarios: '%i', inject scenarios per sec: '%i'" x.ConstantActorCount x.OneTimeActorCount
                              pb.Tick(msg))

                    (fun () -> pb.Dispose())
            )
        | _ -> Array.empty

    let displayConnectionPoolProgress (dep: GlobalDependency, pool: ConnectionPool) =
        match dep.ApplicationType with
        | ApplicationType.Console ->
            let pb = dep.CreateProgressBar(pool.ConnectionCount)
            pool.EventStream
            |> Observable.subscribeWithCompletion
                (fun event ->
                    match event with
                    | StartedInit poolName -> pb.Message <- sprintf "opening connections for connection pool: '%s'" poolName
                    | StartedStop poolName -> pb.Message <- sprintf "closing connections for connection pool: '%s'" poolName

                    | ConnectionOpened (poolName,number) ->
                        pb.Tick(sprintf "opened connection '%i' for connection pool: '%s'" number poolName)

                    | ConnectionClosed -> pb.Tick()

                    | CloseConnectionFailed ex ->
                        pb.Tick()
                        dep.Logger.Error(ex.ToString(), "close connection exception")

                    | InitFinished
                    | InitFailed -> pb.Dispose()
                )
                (fun _ -> pb.Dispose())

            |> Some

        | _ -> None

module internal TestHostScenario =

    let rec waitForNotFinishedScenarios (dep: GlobalDependency, tryCount: int, scnSchedulers: ScenarioScheduler[]) = async {
        let waitingTaskCount =
            scnSchedulers
            |> Seq.collect(fun x -> x.AllActors)
            |> Seq.filter(fun x -> x.Working)
            |> Seq.length

        if tryCount >= Constants.ReTryCount then
            dep.Logger.Information("hard stop with '{WaitingTaskCount}' not finished tasks", waitingTaskCount)

        elif waitingTaskCount <> 0 then
            dep.Logger.Information("waiting for not finished tasks '{WaitingTaskCount}'...", waitingTaskCount)
            do! Async.Sleep(Constants.OperationTimeOut)
            return! waitForNotFinishedScenarios(dep, tryCount + 1, scnSchedulers)
    }

    let initConnectionPools (dep: GlobalDependency) (pools: ConnectionPool seq) =
        asyncResult {
            for pool in pools do
                let subscription = TestHostConsole.displayConnectionPoolProgress(dep, pool)
                do! pool.Init()
                if subscription.IsSome then subscription.Value.Dispose()
        }

    let destroyConnectionPools (dep: GlobalDependency) (pools: ConnectionPool seq) =
        for pool in pools do
            let subscription = TestHostConsole.displayConnectionPoolProgress(dep, pool)
            pool.Dispose()
            if subscription.IsSome then subscription.Value.Dispose()

    let initScenarios (dep: GlobalDependency, context: ScenarioContext, scenarios: Scenario list) = asyncResult {

        let tryInitScenario (initFunc: ScenarioContext -> Task) =
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

                    match tryInitScenario(initFunc) with
                    | Ok _     -> yield! init(tail)
                    | Error ex -> Error ex

                | None -> Ok()
            | [] -> Ok()
        }

        do! scenarios |> Scenario.filterDistinctConnectionPools |> initConnectionPools(dep)
        do! scenarios |> init |> Result.toEmptyIO

        return scenarios
    }

    let cleanScenarios (dep: GlobalDependency, context: ScenarioContext, scenarios: Scenario list) =
        scenarios |> Scenario.filterDistinctConnectionPools |> destroyConnectionPools(dep)

        let tryClean (cleanFunc: ScenarioContext -> Task) =
            try
                cleanFunc(context).Wait()
            with
            | ex -> dep.Logger.Error(ex.ToString(), "clean scenario failed")

        for s in scenarios do
            match s.TestClean with
            | Some cleanFunc ->
                dep.Logger.Information("start clean scenario: '{Scenario}'", s.ScenarioName)
                tryClean(cleanFunc)

            | None -> ()
