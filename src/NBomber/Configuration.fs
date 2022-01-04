namespace NBomber.Configuration

open FSharp.Json
open NBomber.Contracts.Stats

type LoadSimulationSettings =
    | RampConstant of copies:int * during:string
    | KeepConstant of copies:int * during:string
    | RampPerSec   of rate:int   * during:string
    | InjectPerSec of rate:int   * during:string

type ClientFactorySetting = {
    FactoryName: string
    ClientCount: int
}

type ScenarioSetting = {
    ScenarioName: string
    WarmUpDuration: string option
    LoadSimulationsSettings: LoadSimulationSettings list option
    ClientFactorySettings: ClientFactorySetting list option
    CustomStepOrder: string[] option
    [<JsonField(AsJson = true)>] CustomSettings: string option
}

type GlobalSettings = {
    ScenariosSettings: ScenarioSetting list option
    ReportFileName: string option
    ReportFolder: string option
    ReportFormats: ReportFormat list option
    ReportingInterval: string option
    UseHintsAnalyzer: bool option
}

type NBomberConfig = {
    TestSuite: string option
    TestName: string option
    TargetScenarios: string list option
    GlobalSettings: GlobalSettings option
}

module internal JsonConfig =

    let unsafeParse (json) =
        let parseSettings = JsonConfig.create(allowUntyped = true)
        Json.deserializeEx<NBomberConfig> parseSettings json
