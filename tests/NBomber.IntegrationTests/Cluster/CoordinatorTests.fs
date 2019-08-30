module Tests.CoordinatorTests

open Xunit

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Cluster

let private dep = Dependency.create(ApplicationType.Process, NodeType.Coordinator)

let settings = {
    ClusterId = "1"
    TargetScenarios = List.empty
    MqttServer = "localhost"
    Agents = List.empty
}

let customSettings = ""

[<Fact>]
let ``Coordinator should `` () =
    
    let state = Coordinator.init(dep, Array.empty, settings, Array.empty, customSettings)
    
    ()


