module internal rec NBomber.Reporting.HtmlReport

open System

open NBomber.Statistics
open NBomber.Infra
open NBomber.Infra.EnvironmentInfo
open NBomber.Infra.Dependency
open NBomber.Infra.ResourceManager

let print (dep: Dependency, scResult: ScenarioStats) =
    let envView = EnvView.print(dep.Assets.EnvViewHtml, dep.EnvironmentInfo)
    let resultsView = ResultsView.print(dep.Assets.ResultsViewHtml, scResult)
    dep.Assets.MainHtml.Replace("{env_view}", envView)
                       .Replace("{results_view}", resultsView)

module EnvView =
    
    let print (envViewHtml: string, envInfo: EnvironmentInfo) =            
        let row = [envInfo.OS.VersionString; envInfo.DotNetVersion
                   envInfo.Processor; envInfo.ProcessorArchitecture]
                  |> HtmlBuilder.printTableRow        
        envViewHtml.Replace("{env_table}", row)

module ResultsView =    

    let print (scnViewHtml: string, scResult: ScenarioStats) =
        let scnTable = printScenarioTable(scResult)
        scnViewHtml.Replace("{results_table}", scnTable)        

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
        [flowName; concurrentCopies.ToString(); step.StepName; step.Latencies.Length.ToString();
         step.OkCount.ToString(); step.FailCount.ToString();
         stats.RPS.ToString(); stats.Min.ToString(); stats.Mean.ToString(); stats.Max.ToString();
         stats.Percent50.ToString(); stats.Percent75.ToString()]
        |> HtmlBuilder.printTableRow