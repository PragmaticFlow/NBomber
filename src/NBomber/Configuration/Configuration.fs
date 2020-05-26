namespace NBomber.Configuration

open System
open FSharp.Json

type ReportFormat =
    | Txt = 0
    | Html = 1
    | Csv = 2

type LoadSimulationSettings =
    | RampConcurrentScenarios of copiesCount:int * during:DateTime
    | KeepConcurrentScenarios of copiesCount:int * during:DateTime
    | RampScenariosPerSec     of copiesCount:int * during:DateTime
    | InjectScenariosPerSec   of copiesCount:int * during:DateTime

type ConnectionPoolSetting = {
    PoolName: string
    ConnectionCount: int
}

type ScenarioSetting = {
    ScenarioName: string
    WarmUpDuration: DateTime
    LoadSimulationsSettings: LoadSimulationSettings list
    ConnectionPoolSettings: ConnectionPoolSetting list option
    [<JsonField(AsJson = true)>] CustomSettings: string option
}

type GlobalSettings = {
    ScenariosSettings: ScenarioSetting list option
    ReportFileName: string option
    ReportFormats: ReportFormat list option
    SendStatsInterval: DateTime option
}

type NBomberConfig = {
    TestSuite: string option
    TestName: string option
    TargetScenarios: string list option
    GlobalSettings: GlobalSettings option
}

module internal JsonConfig =

    let unsafeParse (json) =
        let config = JsonConfig.create(allowUntyped = true)
        Json.deserializeEx<NBomberConfig> config json
