module internal NBomber.Domain.Concurrency.ScenarioActor

open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Step
open NBomber.Domain.Stats.ScenarioStatsActor

type ActorDep = {
    Logger: ILogger
    CancellationToken: CancellationToken
    GlobalTimer: Stopwatch
    Scenario: Scenario
    ScenarioStatsActor: MailboxProcessor<ActorMessage>
    ExecStopCommand: StopCommand -> unit
}

type ScenarioActor(dep: ActorDep, scenarioInfo: ScenarioInfo) =

    let _logger = dep.Logger.ForContext<ScenarioActor>()
    let _isAllExecSync = Step.isAllExecSync dep.Scenario.Steps

    let _stepDep = { ScenarioInfo = scenarioInfo
                     Logger = dep.Logger
                     CancellationToken = dep.CancellationToken
                     GlobalTimer = dep.GlobalTimer
                     ExecStopCommand = dep.ExecStopCommand }

    let _steps = dep.Scenario.Steps
                 |> List.map(RunningStep.create _stepDep)
                 |> List.toArray

    let _responseBuffer = ResizeArray<int * StepResponse>(Constants.ScenarioResponseBufferLength) // stepIndex * StepResponse
    let _stepDataDict = Dictionary<string,obj>()
    let mutable _working = false

    let flushStats () =
        let responses = _responseBuffer.ToArray()
        dep.ScenarioStatsActor.Post(AddResponses responses)
        _responseBuffer.Clear()

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

                if _responseBuffer.Count >= Constants.ScenarioResponseBufferLength then
                    flushStats()
            else
                _logger.Fatal($"ExecSteps was invoked for already working actor with scenario '{dep.Scenario.ScenarioName}'.")
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

                    if _responseBuffer.Count >= Constants.ScenarioResponseBufferLength then
                        flushStats()
            else
                _logger.Fatal($"RunInfinite was invoked for already working actor with scenario '{dep.Scenario.ScenarioName}'.")
        finally
            _working <- false
    }

    member _.ScenarioInfo = scenarioInfo
    member _.Working = _working

    member _.ExecSteps() = execSteps()
    member _.RunInfinite() = runInfinite()

    member _.Stop() =
        _working <- false
        flushStats()
