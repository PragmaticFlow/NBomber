namespace NBomber.Configuration.Yaml

open System.Collections.Generic

open NBomber.Extensions.InternalExtensions
open NBomber.Configuration

[<CLIMutable>]
type KeepConcurrentScenariosYaml = {
    CopiesCount: int
    During: string
}

[<CLIMutable>]
type RampConcurrentScenariosYaml = {
    CopiesCount: int
    During: string
}

[<CLIMutable>]
type InjectScenariosPerSecYaml = {
    CopiesCount: int
    During: string
}

[<CLIMutable>]
type RampScenariosPerSecYaml = {
    CopiesCount: int
    During: string
}

[<CLIMutable>]
type LoadSimulationSettingsYaml = {
    KeepConcurrentScenarios: KeepConcurrentScenariosYaml
    RampConcurrentScenarios: RampConcurrentScenariosYaml
    InjectScenariosPerSec: InjectScenariosPerSecYaml
    RampScenariosPerSec: RampScenariosPerSecYaml
}

[<CLIMutable>]
type ConnectionPoolSettingYaml = {
    PoolName: string
    ConnectionCount: int
}

[<CLIMutable>]
type ScenarioSettingYaml = {
    ScenarioName: string
    WarmUpDuration: string
    LoadSimulationsSettings: LoadSimulationSettingsYaml[]
    ConnectionPoolSettings: ConnectionPoolSettingYaml[]
    CustomSettings: IDictionary<obj, obj>
}

[<CLIMutable>]
type GlobalSettingsYaml = {
    ScenariosSettings: ScenarioSettingYaml[]
    ReportFileName: string
    ReportFormats: ReportFormat[]
    SendStatsInterval: string
}

[<CLIMutable>]
type NBomberConfigYaml = {
    TestSuite: string
    TestName: string
    TargetScenarios: string[]
    GlobalSettings: GlobalSettingsYaml
}

module internal YamlConfig =

    let private toListOption (data: seq<'T>) =
        data |> Option.ofObj |> Option.map(fun x -> Seq.toList x)

    let mapLoadSimulationsSettings (loadSimulations: LoadSimulationSettingsYaml): LoadSimulationSettings =
        if isNotNull(box loadSimulations.KeepConcurrentScenarios) then
            KeepConcurrentScenarios(loadSimulations.KeepConcurrentScenarios.CopiesCount,
                                    loadSimulations.KeepConcurrentScenarios.During)

        else if isNotNull(box loadSimulations.RampConcurrentScenarios) then
            RampConcurrentScenarios(loadSimulations.RampConcurrentScenarios.CopiesCount,
                                    loadSimulations.RampConcurrentScenarios.During)

        else if isNotNull(box loadSimulations.InjectScenariosPerSec) then
            InjectScenariosPerSec(loadSimulations.InjectScenariosPerSec.CopiesCount,
                                  loadSimulations.InjectScenariosPerSec.During)

        else if isNotNull(box loadSimulations.RampScenariosPerSec) then
            RampScenariosPerSec(loadSimulations.RampScenariosPerSec.CopiesCount,
                                loadSimulations.RampScenariosPerSec.During)

        else failwith "LoadSimulationSettings must not be empty"

    let mapScenariosSettings (scenarioSettings: ScenarioSettingYaml): ScenarioSetting =

        let mapConnectionPool (pool: ConnectionPoolSettingYaml): ConnectionPoolSetting =
            { PoolName = pool.PoolName; ConnectionCount = pool.ConnectionCount }

        let mapCustomSettings (customSettings: IDictionary<obj,obj>) =
            if isNull customSettings then None
            else YamlDotNet.Serialization.Serializer().Serialize(customSettings) |> Some

        { ScenarioName   = scenarioSettings.ScenarioName
          WarmUpDuration = scenarioSettings.WarmUpDuration
          LoadSimulationsSettings = scenarioSettings.LoadSimulationsSettings |> Seq.map mapLoadSimulationsSettings |> Seq.toList
          ConnectionPoolSettings = scenarioSettings.ConnectionPoolSettings |> Seq.map(mapConnectionPool) |> toListOption
          CustomSettings = scenarioSettings.CustomSettings |> mapCustomSettings }

    let mapGlobalSettings (globalSettings: GlobalSettingsYaml): GlobalSettings = {
        ScenariosSettings = globalSettings.ScenariosSettings |> Seq.map(mapScenariosSettings) |> toListOption
        ReportFileName    = globalSettings.ReportFileName    |> String.toOption
        ReportFormats     = globalSettings.ReportFormats     |> toListOption
        SendStatsInterval = globalSettings.SendStatsInterval |> Option.ofObj
    }

    let mapNBomberConfig (yamlConfig: NBomberConfigYaml): NBomberConfig = {
        TestSuite = yamlConfig.TestSuite |> Option.ofObj
        TestName = yamlConfig.TestName |> Option.ofObj
        TargetScenarios = yamlConfig.TargetScenarios |> toListOption
        GlobalSettings = yamlConfig.GlobalSettings |> Option.ofRecord |> Option.map(mapGlobalSettings)
    }

    let unsafeParse (yaml: string) =
        yaml
        |> YamlDotNet.Serialization.Deserializer().Deserialize<NBomberConfigYaml>
        |> mapNBomberConfig
