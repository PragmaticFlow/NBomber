module internal rec NBomber.Reporting.HtmlReport

open System

open NBomber.Statistics
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.Infra.ResourceManager

let print (dep: Dependency, scResult: ScenarioStats) =
    let scenarioView = ScenarioView.print(dep.Assets.ScenarioViewHtml, scResult)
    dep.Assets.MainHtml.Replace("{scenario_view}", scenarioView)

module ScenarioView =    

    let print (scnViewHtml: string, scResult: ScenarioStats) =
        let scnTable = printScenarioTable(scResult)
        scnViewHtml.Replace("{scenario_table}", scnTable)        

    let private printScenarioTable (scResult: ScenarioStats) =
        scResult.FlowsStats
        |> Array.map(printFlowRow)
        |> String.concat("")

    let private printFlowRow (flow: FlowStats) =
        flow.StepsStats
        |> Array.map(fun step -> printStepRow(step, flow.FlowName, flow.ConcurrentCopies))
        |> String.concat("")

    let private printStepRow (step: StepStats, flowName: string, concurrentCopies: int) =
        let stats = step.Details.Value
        [| flowName; concurrentCopies.ToString(); step.StepName; step.Latencies.Length.ToString();
           step.OkCount.ToString(); step.FailCount.ToString();
           stats.RPS.ToString(); stats.Min.ToString(); stats.Mean.ToString(); stats.Max.ToString();
           stats.Percent50.ToString(); stats.Percent75.ToString() |]        
        |> HtmlBuilder.printTableRow