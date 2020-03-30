namespace rec NBomber.Configuration

open System
open System.Collections.Generic
open FSharp.Json
open Microsoft.Extensions.Configuration

type ReportFormat =
    | Txt = 0
    | Html = 1
    | Csv = 2
    | Md = 3

type LoadSimulationSettings =
    | KeepConcurrentScenarios of copiesCount:int * during:DateTime
    | RampConcurrentScenarios of copiesCount:int * during:DateTime
    | InjectScenariosPerSec   of copiesCount:int * during:DateTime
    | RampScenariosPerSec     of copiesCount:int * during:DateTime

type ScenarioSetting = {
    ScenarioName: string
    WarmUpDuration: DateTime
    LoadSimulationsSettings: LoadSimulationSettings list
    [<JsonField(Transform=typeof<JsonConfig.JsonStringTransform>)>]
    CustomSettings: string option
}

[<CLIMutable>]
type ConnectionPoolSetting = {
    PoolName: string
    ConnectionCount: int
}

type GlobalSettings = {
    ScenariosSettings: ScenarioSetting list option
    TargetScenarios: string list option
    ConnectionPoolSettings: ConnectionPoolSetting list option
    ReportFileName: string option
    ReportFormats: ReportFormat list option
    SendStatsInterval: DateTime option
}

[<CLIMutable>]
type TargetGroupSettings = {
    TargetGroup: string
    TargetScenarios: string list
}

type TestConfig = {
    TestSuite: string option
    TestName: string option
    GlobalSettings: GlobalSettings option
}

module internal JsonConfig =

    type JsonStringTransform() =
        interface ITypeTransform with
            member x.targetType () = typeof<obj>
            member x.toTargetType(value) = value
            member x.fromTargetType(value) =
                let config = JsonConfig.create(allowUntyped = true)
                let str = Json.serializeEx config value
                str :> obj

    let unsafeParse (json) =
        let config = JsonConfig.create(allowUntyped = true)
        Json.deserializeEx<TestConfig> config json

module YamlConfigModels =

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
    type GlobalSettingsYaml = {
        ScenariosSettings: ScenarioSettingYaml[]
        TargetScenarios: string[]
        ConnectionPoolSettings: ConnectionPoolSetting[]
        ReportFileName: string
        ReportFormats: ReportFormat[]
        SendStatsInterval: TimeSpan
    }

    [<CLIMutable>]
    type TestConfigYaml = {
        TestSuite: string
        TestName: string
        GlobalSettings: GlobalSettingsYaml
    }

module internal YamlConfig =

    let private isNotNull (value) =
        obj.ReferenceEquals(value, null) |> not

    let mapToOptionWithMapper (mapper) (nullable) =
        match nullable with
        | null -> None
        | _    -> nullable |> mapper |> Some

    let mapToOption (nullable) =
        nullable
        |> mapToOptionWithMapper (fun x -> x)

    let private mapToListOptionWithMapper (mapper) (array: 'a[]) =
        array
        |> mapToOptionWithMapper (fun x -> x |> Array.map mapper |> List.ofArray)

    let private mapToListOption (array: 'a[]) =
        array
        |> mapToOptionWithMapper List.ofArray

    let private mapToDateTime (timeSpan: TimeSpan) =
        DateTime.MinValue + timeSpan

    let private mapToLoadSimulationsSettings (loadSimulationsSettings: YamlConfigModels.LoadSimulationSettingsYaml) =
        if isNotNull(loadSimulationsSettings.KeepConcurrentScenarios) then
            KeepConcurrentScenarios(loadSimulationsSettings.KeepConcurrentScenarios.CopiesCount,
                                    loadSimulationsSettings.KeepConcurrentScenarios.During
                                    |> mapToDateTime)

        else if isNotNull(loadSimulationsSettings.RampConcurrentScenarios) then
            RampConcurrentScenarios(loadSimulationsSettings.RampConcurrentScenarios.CopiesCount,
                                    loadSimulationsSettings.RampConcurrentScenarios.During
                                    |> mapToDateTime)

        else if isNotNull(loadSimulationsSettings.InjectScenariosPerSec) then
            InjectScenariosPerSec(loadSimulationsSettings.InjectScenariosPerSec.CopiesCount,
                                  loadSimulationsSettings.InjectScenariosPerSec.During
                                  |> mapToDateTime)

        else if isNotNull(loadSimulationsSettings.RampScenariosPerSec) then
            RampScenariosPerSec(loadSimulationsSettings.RampScenariosPerSec.CopiesCount,
                                loadSimulationsSettings.RampScenariosPerSec.During
                                |> mapToDateTime)

        else failwith "LoadSimulationSettings must not be empty"

    let private mapToScenarioSetting (scenarioSettingYaml: YamlConfigModels.ScenarioSettingYaml) =
        let screnarioName = scenarioSettingYaml.ScenarioName
        let warmUpDuration = scenarioSettingYaml.WarmUpDuration |> mapToDateTime
        let loadSimulationsSettings = scenarioSettingYaml.LoadSimulationsSettings
                                      |> Array.map mapToLoadSimulationsSettings
                                      |> List.ofArray
        let serializer = YamlDotNet.Serialization.Serializer()
        let customSettings = scenarioSettingYaml.CustomSettings
                             |> mapToOptionWithMapper (fun x-> x |> serializer.Serialize)

        { ScenarioName = screnarioName
          WarmUpDuration = warmUpDuration
          LoadSimulationsSettings = loadSimulationsSettings
          CustomSettings = customSettings }

    let private mapToGlobalSettings (globalSettingsYaml: YamlConfigModels.GlobalSettingsYaml) =
        let scenariosSettings = globalSettingsYaml.ScenariosSettings |> mapToListOptionWithMapper mapToScenarioSetting
        let targetScenarios = globalSettingsYaml.TargetScenarios |> mapToListOption
        let connectionPoolSettings = globalSettingsYaml.ConnectionPoolSettings |> mapToListOption
        let reportFileName = globalSettingsYaml.ReportFileName |> mapToOption
        let reportFormats = globalSettingsYaml.ReportFormats |> mapToListOption
        let sendStatsInterval = if globalSettingsYaml.SendStatsInterval.Ticks > 0L
                                then globalSettingsYaml.SendStatsInterval |> mapToDateTime |> Some
                                else None

        { ScenariosSettings = scenariosSettings
          TargetScenarios = targetScenarios
          ConnectionPoolSettings = connectionPoolSettings
          ReportFileName = reportFileName
          ReportFormats = reportFormats
          SendStatsInterval = sendStatsInterval }

    let private mapToTestConfig (testConfigYaml: YamlConfigModels.TestConfigYaml) =
        let globalSettings = testConfigYaml.GlobalSettings |> mapToGlobalSettings |> Some

        { TestSuite = Some testConfigYaml.TestSuite
          TestName = Some testConfigYaml.TestName
          GlobalSettings = globalSettings }

    let unsafeParse (yaml: string) =
        let deserializer = YamlDotNet.Serialization.Deserializer()
        let testConfig = yaml
                         |> deserializer.Deserialize<YamlConfigModels.TestConfigYaml>
                         |> mapToTestConfig
        testConfig
