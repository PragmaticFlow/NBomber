module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Step
open NBomber.Domain.Stats.ScenarioStatsActor

type ActorDep = {
    Logger: ILogger
    CancellationToken: CancellationToken
    ScenarioGlobalTimer: Stopwatch
    Scenario: Scenario
    ScenarioStatsActor: MailboxProcessor<StatsActorMessage>
    ExecStopCommand: StopCommand -> unit
}

type ScenarioActor(dep: ActorDep, scenarioInfo: ScenarioInfo) =

    let _logger = dep.Logger.ForContext<ScenarioActor>()
    let _isAllExecSync = Step.isAllExecSync dep.Scenario.Steps

    let _stepDep = { ScenarioInfo = scenarioInfo
                     Logger = dep.Logger
                     CancellationToken = dep.CancellationToken
                     ScenarioGlobalTimer = dep.ScenarioGlobalTimer
                     ExecStopCommand = dep.ExecStopCommand }

    let _steps = dep.Scenario.Steps
                 |> List.map(RunningStep.create _stepDep)
                 |> List.toArray

    let _responseBuffer = ResizeArray<int * StepResponse>(Constants.ResponseBufferLength) // stepIndex * StepResponse
    let mutable _latestBufferFlushSec = 0
    let _stepDataDict = Dictionary<string,obj>()
    let mutable _working = false

    let flushStats (buffer: ResizeArray<int * StepResponse>) =
        let responses = buffer.ToArray()
        dep.ScenarioStatsActor.Post(AddResponses responses)
        buffer.Clear()
        _latestBufferFlushSec <- int dep.ScenarioGlobalTimer.Elapsed.TotalSeconds

    let checkFlushBuffer (buffer: ResizeArray<int * StepResponse>) =
        if buffer.Count >= Constants.ResponseBufferLength then
            flushStats(buffer)
        else
            let delay = int dep.ScenarioGlobalTimer.Elapsed.TotalSeconds - _latestBufferFlushSec
            if delay >= Constants.ResponseBufferFlushDelaySec then
                flushStats(buffer)

    let execSteps () = task {
        try
            if not _working then
                _working <- true
                do! Task.Yield()

                _stepDataDict.Clear()

                if _isAllExecSync then
                    Step.execSteps(_stepDep, _steps, dep.Scenario.GetStepsOrder(), _responseBuffer, _stepDataDict)
                else
                    do! Step.execStepsAsync(_stepDep, _steps, dep.Scenario.GetStepsOrder(), _responseBuffer, _stepDataDict)

                checkFlushBuffer(_responseBuffer)
            else
                _logger.Error($"ExecSteps was invoked for already working actor with scenario '{dep.Scenario.ScenarioName}'.")
        finally
            _working <- false
    }

    let runInfinite () = task {
        try
            if not _working then
                _working <- true

                do! Task.Yield()
                while _working && not dep.CancellationToken.IsCancellationRequested do

                    _stepDataDict.Clear()

                    if _isAllExecSync then
                        Step.execSteps(_stepDep, _steps, dep.Scenario.GetStepsOrder(), _responseBuffer, _stepDataDict)
                    else
                        do! Step.execStepsAsync(_stepDep, _steps, dep.Scenario.GetStepsOrder(), _responseBuffer, _stepDataDict)

                    checkFlushBuffer(_responseBuffer)
            else
                _logger.Error($"RunInfinite was invoked for already working actor with scenario '{dep.Scenario.ScenarioName}'.")
        finally
            _working <- false
    }

    member _.ScenarioStatsActor = dep.ScenarioStatsActor
    member _.ScenarioInfo = scenarioInfo
    member _.Working = _working

    member _.ExecSteps() = execSteps()
    member _.RunInfinite() = runInfinite()

    member _.Stop() =
        _working <- false
        flushStats(_responseBuffer)
