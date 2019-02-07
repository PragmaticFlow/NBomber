module internal NBomber.DomainServices.ScenarioBuilder

open Serilog

open NBomber
open NBomber.Configuration
open NBomber.Domain
open NBomber.Domain.DomainTypes

let updateScenarioWithSettings (scenario: Scenario, settings: ScenarioSetting) =
    { scenario with ConcurrentCopies = settings.ConcurrentCopies
                    WarmUpDuration = settings.WarmUpDuration
                    Duration = settings.Duration }

let filterTargetScenarios (targetScenarios: string[]) (scenarios: Scenario[]) =
    if Array.isEmpty(targetScenarios) then scenarios
    else
        Log.Information("target scenarios from config: {0}", targetScenarios |> String.concatWithCommaAndQuotes)
        scenarios 
        |> Array.filter(fun x -> targetScenarios |> Array.exists(fun target -> x.ScenarioName = target))
        
let applyScenariosSettings (settings: ScenarioSetting[]) (scenarios: Scenario[]) =        
    scenarios
    |> Array.map(fun scenario -> 
        settings |> Array.tryFind(fun s -> s.ScenarioName = scenario.ScenarioName)
                    |> function | Some setting -> updateScenarioWithSettings(scenario, setting)
                                | None -> scenario)

let build (scenarios: Contracts.Scenario[]) = 
    scenarios |> Array.map(Scenario.create)