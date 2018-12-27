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
}

type NBomberConfig = {
    GlobalSettings: GlobalSettings option    
}

type NBomberConfigJson = {
    GlobalSettings: GlobalSettings  
}

module internal NBomberConfig =
    open Newtonsoft.Json

    let parse (json): NBomberConfig option =
        
        let parseGlobalSettings (config) =
            if isNull(config.GlobalSettings :> obj) then None
            else Some(config.GlobalSettings)        

        let config = JsonConvert.DeserializeObject<NBomberConfigJson>(json)
        if isNull(config :> obj) then None
        else
            let globalSettings = parseGlobalSettings(config)
            Some({ GlobalSettings = globalSettings })