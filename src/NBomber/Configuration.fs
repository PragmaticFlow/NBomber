namespace NBomber.Configuration

open System
open FSharp.Json
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal.Serialization.JsonTransforms

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

    [<JsonField(Transform=typeof<TimeSpanTransform>)>]
    WarmUpDuration: TimeSpan option

    LoadSimulationsSettings: LoadSimulationSettings list option
    ClientFactorySettings: ClientFactorySetting list option
    CustomStepOrder: string[] option

    [<JsonField(AsJson = true)>]
    CustomSettings: string option
}

type GlobalSettings = {
    ScenariosSettings: ScenarioSetting list option
    ReportFileName: string option
    ReportFolder: string option
    ReportFormats: ReportFormat list option

    [<JsonField(Transform=typeof<TimeSpanTransform>)>]
    ReportingInterval: TimeSpan option

    EnableHintsAnalyzer: bool option
    DefaultStepTimeoutMs: int option
}

type NBomberConfig = {
    TestSuite: string option
    TestName: string option
    TargetScenarios: string list option
    GlobalSettings: GlobalSettings option
}
