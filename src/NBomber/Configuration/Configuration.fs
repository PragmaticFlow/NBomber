namespace NBomber.Configuration

open FSharp.Json

type ReportFormat =
    | Txt = 0
    | Html = 1
    | Csv = 2
    | Md = 3

type LoadSimulationSettings =
    | RampConstant of copies:int * during:string
    | KeepConstant of copies:int * during:string
    | RampPerSec   of rate:int   * during:string
    | InjectPerSec of rate:int   * during:string

type ConnectionPoolSetting = {
    PoolName: string
    ConnectionCount: int
}

type ScenarioSetting = {
    ScenarioName: string
    WarmUpDuration: string
    LoadSimulationsSettings: LoadSimulationSettings list
    ConnectionPoolSettings: ConnectionPoolSetting list option
    [<JsonField(AsJson = true)>] CustomSettings: string option
}

type GlobalSettings = {
    ScenariosSettings: ScenarioSetting list option
    ReportFileName: string option
    ReportFormats: ReportFormat list option
    SendStatsInterval: string option
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
