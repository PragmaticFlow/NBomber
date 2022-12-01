module internal NBomber.Domain.DomainTypes

open System
open System.Threading.Tasks
open NBomber
open NBomber.Contracts

type SessionId = string
type ScenarioName = string
[<Measure>] type scenarioDuration

type StopCommand =
    | StopScenario of scenarioName:string * reason:string
    | StopTest of reason:string

type LoadSimulation = {
    Value: Contracts.LoadSimulation
    StartTime: TimeSpan
    EndTime: TimeSpan
    PrevActorCount: int
}

type Scenario = {
    ScenarioName: string
    Init: (IScenarioInitContext -> Task) option
    Clean: (IScenarioInitContext -> Task) option
    Run: (IScenarioContext -> Task<IResponse>) option
    LoadSimulations: LoadSimulation list
    WarmUpDuration: TimeSpan option
    PlanedDuration: TimeSpan
    ExecutedDuration: TimeSpan option
    CustomSettings: string
    IsEnabled: bool // used for stats in the cluster mode
    IsInitialized: bool
    ResetIterationOnFail: bool
    MaxFailCount: int
}
