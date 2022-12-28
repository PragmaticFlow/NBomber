﻿module internal NBomber.Domain.ScenarioContext

open System.Collections.Generic
open System.Diagnostics
open System.Threading
open Serilog
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.ScenarioStatsActor

type ScenarioContextArgs = {
    Logger: ILogger
    Scenario: Scenario
    ScenarioCancellationToken: CancellationTokenSource
    ScenarioTimer: Stopwatch
    ScenarioOperation: ScenarioOperation
    ScenarioStatsActor: ScenarioStatsActor
    ExecStopCommand: StopCommand -> unit
    TestInfo: TestInfo
    GetNodeInfo: unit -> NodeInfo
}

type ScenarioContext(args: ScenarioContextArgs, scenarioInfo) =

    let _logger = args.Logger
    let _scnActor = args.ScenarioStatsActor
    let _timer = args.ScenarioTimer
    let _resetIteration = args.Scenario.ResetIterationOnFail
    let _testInfo = args.TestInfo
    let _data = Dictionary<string,obj>()
    let mutable _invocationNumber = 0

    member _.ResetIterationOnFail = _resetIteration
    member _.InvocationNumber = _invocationNumber
    member _.StatsActor = _scnActor
    member _.Timer = _timer

    member inline _.PrepareNextIteration() =
        _invocationNumber <- _invocationNumber + 1
        _data.Clear()

    interface IScenarioContext with
        member this.TestInfo = _testInfo
        member this.ScenarioInfo = scenarioInfo
        member this.NodeInfo = args.GetNodeInfo()
        member this.Logger = _logger
        member this.InvocationNumber = _invocationNumber
        member this.Data = _data
        member this.StopCurrentTest(reason) = args.ExecStopCommand(StopTest reason)
        member this.StopScenario(scenarioName, reason) = args.ExecStopCommand(StopScenario(scenarioName, reason))
