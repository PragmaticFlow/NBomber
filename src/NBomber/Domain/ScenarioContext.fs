module internal NBomber.Domain.ScenarioContext

open System.Collections.Generic
open System.Diagnostics
open System.Threading

open Serilog

open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.ScenarioStatsActor

type ScenarioExecContext = {
    Logger: ILogger
    Scenario: Scenario
    ScenarioCancellationToken: CancellationTokenSource
    ScenarioTimer: Stopwatch
    ScenarioOperation: ScenarioOperation
    ScenarioStatsActor: ScenarioStatsActor
    ExecStopCommand: StopCommand -> unit
    MaxFailCount: int
}

type StepExecContext = {
    ScenarioExecContext: ScenarioExecContext
    ScenarioInfo: ScenarioInfo
    Data: Dictionary<string,obj>
}

type ScenarioContext(scenarioInfo, args: ScenarioExecContext) as this =

    let _logger = args.Logger
    let _scnActor = args.ScenarioStatsActor
    let _timer = args.ScenarioTimer
    let _cancelToken = args.ScenarioCancellationToken.Token

    member val StopIteration = false with get, set
    member _.StatsActor = _scnActor
    member _.Timer = _timer

    interface IFlowContext with
        member this.CancellationToken = _cancelToken
        member this.InvocationNumber = failwith "todo"
        member this.Logger = _logger
        member this.ScenarioInfo = scenarioInfo
        member this.StopCurrentTest(reason) = args.ExecStopCommand(StopTest reason)
        member this.StopScenario(scenarioName, reason) = args.ExecStopCommand(StopScenario(scenarioName, reason))
