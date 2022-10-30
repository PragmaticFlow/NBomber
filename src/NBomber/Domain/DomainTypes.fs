module internal NBomber.Domain.DomainTypes

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open Serilog

open NBomber.Contracts

type SessionId = string
type ScenarioName = string
[<Measure>] type scenarioDuration

type StopCommand =
    | StopScenario of scenarioName:string * reason:string
    | StopTest of reason:string

type LoadTimeSegment = {
    StartTime: TimeSpan
    EndTime: TimeSpan
    Duration: TimeSpan
    PrevSegmentCopiesCount: int
    LoadSimulation: LoadSimulation
}

type LoadTimeLine = LoadTimeSegment list

type Scenario = {
    ScenarioName: string
    Init: (IScenarioInitContext -> Task) option
    Clean: (IScenarioInitContext -> Task) option
    Run: (IScenarioContext -> Task<Response<obj>>) option
    LoadTimeLine: LoadTimeLine
    WarmUpDuration: TimeSpan option
    PlanedDuration: TimeSpan
    ExecutedDuration: TimeSpan option
    CustomSettings: string
    IsEnabled: bool // used for stats in the cluster mode
    IsInitialized: bool
    ResetIterationOnFail: bool
    MaxFailCount: int
}
