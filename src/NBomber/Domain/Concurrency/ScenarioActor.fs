module internal NBomber.Domain.Concurrency.ScenarioActor

open System
open System.Diagnostics
open System.Threading.Tasks
open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.ScenarioContext

type ScenarioActor(scnCtx: ScenarioContextArgs, scenarioInfo: ScenarioInfo) =

    let _logger = scnCtx.Logger.ForContext<ScenarioActor>()
    let _scenario = scnCtx.Scenario
    let _randomizer = Random()
    let _cancelToken = scnCtx.ScenarioCancellationToken.Token
    let sw = Stopwatch()
    let _scenarioCtx = ScenarioContext(scnCtx, sw, scenarioInfo)

    let mutable _working = false
    let mutable _shouldStop = false

    do
        sw.Start()

    let execSteps (startDelay: int) (runInfinite: bool) = backgroundTask {
        try
            if not _working then
                _working <- true
                _shouldStop <- false
                let mutable infiniteRun = true
                let mutable timeBucket = _scenarioCtx.CurrentTimeBucket

                do! Task.Delay startDelay

                while infiniteRun && not _shouldStop && not _cancelToken.IsCancellationRequested do

                    if _scenario.Run.IsSome then
                        _scenarioCtx.PrepareNextIteration()
                        do! Scenario.measure Constants.ScenarioGlobalInfo _scenarioCtx timeBucket _scenario.Run.Value

                    infiniteRun <- runInfinite
                    timeBucket <- _scenarioCtx.CurrentTimeBucket
            else
                _logger.Fatal($"Unhandled exception: ExecSteps was invoked for already working actor with Scenario: {0}", _scenario.ScenarioName)
        finally
            _working <- false
    }

    member _.ScenarioStatsActor = scnCtx.ScenarioStatsActor
    member _.ScenarioInfo = scenarioInfo
    member _.Working = _working

    member _.ExecSteps(injectInterval: TimeSpan) =
        let startDelay = _randomizer.Next(0, int injectInterval.TotalMilliseconds)
        execSteps startDelay false

    member _.RunInfinite(injectInterval: TimeSpan) =
        let startDelay = _randomizer.Next(0, int injectInterval.TotalMilliseconds)
        execSteps startDelay true

    member _.AskToStop() =
        _shouldStop <- true
