namespace NBomber.Configuration

open System

type ScenarioSetting = {
    ScenarioName: string
    ConcurrentCopies: int
    WarmUpDuration: TimeSpan
    Duration: TimeSpan
}

type GlobalSettings = {
    ScenariosSettings: ScenarioSetting[]
    TargetScenarios: string[]
    ReportFileName: string
    ReportFormats: string[]
}

type AgentSettings = {
    ClusterId: string
    Port: int
}

type AgentInfoSettings = {
    Host: string
    Port: int
    TargetScenarios: string[]
}

type CoordinatorSettings = {
    ClusterId: string
    TargetScenarios: string[]
    Agents: AgentInfoSettings[]
}

type ClusterSettings =
    | Coordinator of CoordinatorSettings
    | Agent       of AgentSettings

type ClusterSettingsJson = {
    Coordinator: CoordinatorSettings
    Agent: AgentSettings
}

type NBomberConfig = {
    GlobalSettings: GlobalSettings option
    ClusterSettings: ClusterSettings option
}

type NBomberConfigJson = {
    GlobalSettings: GlobalSettings
    ClusterSettings: ClusterSettingsJson
}

module internal NBomberConfig =
    open Newtonsoft.Json // TODO: replace with utf8json
    let ofNull o =
        if isNull (o :> obj) then None
        else Some o
    let parse (json) : NBomberConfig option =
        let parseGlobalSettings config = ofNull config.GlobalSettings
        let parseClusterSettings config =
            config.ClusterSettings
            |> ofNull
            |> Option.map (fun clusterSettings ->
                let coordinator' = ofNull clusterSettings.Coordinator
                let agent' = ofNull clusterSettings.Agent
                match coordinator', agent' with
                | Some _, Some _ -> failwith "" // todo: return Result
                | None, None -> failwith ""
                | Some coordinator, None ->
                    if isNull coordinator.ClusterId then failwith "ClusterId isNull"
                    if isNull coordinator.TargetScenarios then failwith "TargetScenarios isNull"
                    if isNull coordinator.Agents then failwith "Agents isNull"
                    Coordinator coordinator
                | None, Some agent ->
                    if isNull agent.ClusterId then failwith ""
                    Agent agent
            )

        JsonConvert.DeserializeObject<NBomberConfigJson> json
        |> ofNull
        |> Option.map (fun config ->
            let globalSettings = parseGlobalSettings config
            let clusterSettings = parseClusterSettings config
            { GlobalSettings = globalSettings
              ClusterSettings = clusterSettings }
        )
