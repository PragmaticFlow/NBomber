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
    open Newtonsoft.Json

    let parse (json): NBomberConfig option =
        
        let parseGlobalSettings (config) =
            if isNull(config.GlobalSettings :> obj) then None
            else Some(config.GlobalSettings)        

        let parseClusterSettings (config) =
            if isNull(config.ClusterSettings :> obj) then None
            else                 
                let coordinator = if isNull(config.ClusterSettings.Coordinator :> obj) then None
                                  else Some(config.ClusterSettings.Coordinator)

                let agent = if isNull(config.ClusterSettings.Agent :> obj) then None
                            else Some(config.ClusterSettings.Agent)
                              
                if coordinator.IsSome && agent.IsSome then failwith "" // todo: return Result
                elif coordinator.IsNone && agent.IsNone then failwith ""
                
                if coordinator.IsSome then
                    if isNull(coordinator.Value.ClusterId) then failwith "ClusterId isNull"                                    
                    if isNull(coordinator.Value.TargetScenarios) then failwith "TargetScenarios isNull"
                    if isNull(coordinator.Value.Agents) then failwith "Agents isNull"                                        
                    Some <| Coordinator(coordinator.Value)
                else
                    if isNull(agent.Value.ClusterId) then failwith ""
                    Some <| Agent(agent.Value)

        let config = JsonConvert.DeserializeObject<NBomberConfigJson>(json)
        if isNull(config :> obj) then None
        else
            let globalSettings = parseGlobalSettings(config)
            let clusterSettings = parseClusterSettings(config)
            Some { GlobalSettings = globalSettings
                   ClusterSettings = clusterSettings }