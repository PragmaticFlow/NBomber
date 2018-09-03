module internal rec NBomber.Infra.Dependency

open System
open System.Linq
open System.Resources

open NBomber.Domain
open NBomber.Infra.EnvironmentInfo
open NBomber.Infra.ResourceManager

type Dependency = {
    SessionId: string
    Scenario: Scenario    
    EnvironmentInfo: EnvironmentInfo
    Assets: Assets
}

let create (scenario: Scenario) = 

    let createSessionId () =
        let date = DateTime.UtcNow.ToString("dd.MM.yyyy-HH.mm.ff")
        let guid = Guid.NewGuid().GetHashCode().ToString("x")
        date + "-" + guid
    
    { SessionId = createSessionId()
      Scenario  = scenario      
      EnvironmentInfo = EnvironmentInfo.getEnvironmentInfo()
      Assets = ResourceManager.loadAssets() }