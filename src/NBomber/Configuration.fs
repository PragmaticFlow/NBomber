namespace NBomber.Configuration

open System
open FSharp.Json
open NBomber.Contracts
open NBomber.Contracts.Stats

type ScenarioSetting = {
    ScenarioName: string
    WarmUpDuration: TimeSpan option
    LoadSimulationsSettings: LoadSimulation list option
    [<JsonField(AsJson = true)>] CustomSettings: string option
}

type GlobalSettings = {
    ScenariosSettings: ScenarioSetting list option
    ReportFileName: string option
    ReportFolder: string option
    ReportFormats: ReportFormat list option
    ReportingInterval: TimeSpan option
    EnableHintsAnalyzer: bool option
    MaxFailCount: int option
}

type NBomberConfig = {
    TestSuite: string option
    TestName: string option
    TargetScenarios: string list option
    GlobalSettings: GlobalSettings option
}
