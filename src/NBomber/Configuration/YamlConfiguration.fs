namespace NBomber.Configuration.Yaml

open System
open System.Collections.Generic

open NBomber.Configuration
open NBomber.Extensions

[<CLIMutable>]
type KeepConcurrentScenariosYaml = {
    CopiesCount: int
    During: TimeSpan
}

[<CLIMutable>]
type RampConcurrentScenariosYaml = {
    CopiesCount: int
    During: TimeSpan
}

[<CLIMutable>]
type InjectScenariosPerSecYaml = {
    CopiesCount: int
    During: TimeSpan
}

[<CLIMutable>]
type RampScenariosPerSecYaml = {
    CopiesCount: int
    During: TimeSpan
}

[<CLIMutable>]
type LoadSimulationSettingsYaml = {
    KeepConcurrentScenarios: KeepConcurrentScenariosYaml
    RampConcurrentScenarios: RampConcurrentScenariosYaml
    InjectScenariosPerSec: InjectScenariosPerSecYaml
    RampScenariosPerSec: RampScenariosPerSecYaml
}

[<CLIMutable>]
type ScenarioSettingYaml = {
    ScenarioName: string
    WarmUpDuration: TimeSpan
    LoadSimulationsSettings: LoadSimulationSettingsYaml[]
    CustomSettings: IDictionary<obj, obj>
}

[<CLIMutable>]
type ConnectionPoolSettingYaml = {
    PoolName: string
    ConnectionCount: int
}

[<CLIMutable>]
type GlobalSettingsYaml = {
    ScenariosSettings: ScenarioSettingYaml[]
    ConnectionPoolSettings: ConnectionPoolSettingYaml[]
    ReportFileName: string
    ReportFormats: ReportFormat[]
    SendStatsInterval: TimeSpan
}

[<CLIMutable>]
type NBomberConfigYaml = {
    TestSuite: string
    TestName: string
    GlobalSettings: GlobalSettingsYaml
    TargetScenarios: string[]
}

module internal YamlConfig =

    let private mapDateTime (timeSpan: TimeSpan) =
        DateTime.MinValue + timeSpan

    let private toListOption (data: seq<'T>) =
        data |> Option.ofObj |> Option.map(fun x -> Seq.toList x)

    let mapLoadSimulationsSettings (loadSimulations: LoadSimulationSettingsYaml): LoadSimulationSettings =
        if isNotNull(box loadSimulations.KeepConcurrentScenarios) then
            KeepConcurrentScenarios(loadSimulations.KeepConcurrentScenarios.CopiesCount,
                                    loadSimulations.KeepConcurrentScenarios.During |> mapDateTime)

        else if isNotNull(box loadSimulations.RampConcurrentScenarios) then
            RampConcurrentScenarios(loadSimulations.RampConcurrentScenarios.CopiesCount,
                                    loadSimulations.RampConcurrentScenarios.During |> mapDateTime)

        else if isNotNull(box loadSimulations.InjectScenariosPerSec) then
            InjectScenariosPerSec(loadSimulations.InjectScenariosPerSec.CopiesCount,
                                  loadSimulations.InjectScenariosPerSec.During |> mapDateTime)

        else if isNotNull(box loadSimulations.RampScenariosPerSec) then
            RampScenariosPerSec(loadSimulations.RampScenariosPerSec.CopiesCount,
                                loadSimulations.RampScenariosPerSec.During |> mapDateTime)

        else failwith "LoadSimulationSettings must not be empty"

    let mapScenariosSettings (scenarioSettings: ScenarioSettingYaml): ScenarioSetting =

        let mapCustomSettings (customSettings: IDictionary<obj,obj>) =
            if isNull customSettings then None
            else YamlDotNet.Serialization.Serializer().Serialize(customSettings) |> Some

        { ScenarioName   = scenarioSettings.ScenarioName
          WarmUpDuration = scenarioSettings.WarmUpDuration |> mapDateTime
          LoadSimulationsSettings = scenarioSettings.LoadSimulationsSettings |> Seq.map mapLoadSimulationsSettings |> Seq.toList
          CustomSettings = scenarioSettings.CustomSettings |> mapCustomSettings }

    let mapGlobalSettings (globalSettings: GlobalSettingsYaml): GlobalSettings =

        let mapConnectionPool (pool: ConnectionPoolSettingYaml): ConnectionPoolSetting =
            { PoolName = pool.PoolName; ConnectionCount = pool.ConnectionCount }

        let mapSendStatsInterval (interval: TimeSpan) =
            if interval.Ticks > 0L then interval |> mapDateTime |> Some
            else None

        { ScenariosSettings = globalSettings.ScenariosSettings |> Seq.map(mapScenariosSettings) |> toListOption
          ConnectionPoolSettings = globalSettings.ConnectionPoolSettings |> Seq.map(mapConnectionPool) |> toListOption
          ReportFileName    = globalSettings.ReportFileName    |> String.toOption
          ReportFormats     = globalSettings.ReportFormats     |> toListOption
          SendStatsInterval = globalSettings.SendStatsInterval |> mapSendStatsInterval }

    let mapNBomberConfig (yamlConfig: NBomberConfigYaml): NBomberConfig =
        { TestSuite = yamlConfig.TestSuite |> Option.ofObj
          TestName = yamlConfig.TestName |> Option.ofObj
          GlobalSettings = yamlConfig.GlobalSettings |> Option.ofRecord |> Option.map(mapGlobalSettings)
          TargetScenarios = yamlConfig.TargetScenarios |> toListOption }

    let unsafeParse (yaml: string) =
        yaml
        |> YamlDotNet.Serialization.Deserializer().Deserialize<NBomberConfigYaml>
        |> mapNBomberConfig
