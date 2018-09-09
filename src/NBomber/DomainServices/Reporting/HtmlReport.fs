module internal rec NBomber.Reporting.HtmlReport

open System

open NBomber.Statistics
open NBomber.Infra
open NBomber.Infra.EnvironmentInfo
open NBomber.Infra.Dependency
open NBomber.Infra.ResourceManager

let print (dep: Dependency, scResult: ScenarioStats) =
    let (nHtml,nJs) = NumberReqChart.print(dep.Assets, scResult)
    let (iHtml,iJs) = IndicatorsChart.print(dep.Assets, scResult)
    let stHtml = StatisticsTable.print(dep.Assets, scResult)
    //let envView = EnvView.print(dep.Assets.EnvViewHtml, dep.EnvironmentInfo)
    //let resultsView = StatisticsTable.print(dep.Assets, scResult)
    dep.Assets.MainHtml.Replace("%num_req_chart%", nHtml)
                       .Replace("%indicators_chart%", iHtml)
                       .Replace("%statistics_table%", stHtml)
                       .Replace("%js%", nJs + Environment.NewLine + iJs)
                       //.Replace("{results_view}", resultsView)

module NumberReqChart =
    
    let print (assets: Assets, scResult: ScenarioStats) =
        let dataArray = HtmlBuilder.toJsArray([scResult.AllOkCount; scResult.AllFailedCount])
        let js = assets.NumReqChartJs.Replace("%dataArray%", dataArray)        
        (assets.NumReqChartHtml, js)

module IndicatorsChart =

    let print (assets: Assets, scResult: ScenarioStats) =        
        let dataArray = 
            HtmlBuilder.toJsArray([scResult.LatencyCount.Less800
                                   scResult.LatencyCount.More800Less1200
                                   scResult.LatencyCount.More1200
                                   scResult.AllFailedCount])

        let js = assets.IndicatorsChartJs.Replace("%label%", "Scenario")
                                         .Replace("%dataArray%", dataArray)
        (assets.IndicatorsChartHtml, js)

module StatisticsTable =    

    let print (assets: Assets, scResult: ScenarioStats) =
        let scnTable = printScenarioTable(scResult)
        assets.ResultsViewHtml.Replace("%statistics_table_body%", scnTable)

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
        [flowName; concurrentCopies.ToString(); step.StepName; step.OkLatencies.Length.ToString()
         step.OkCount.ToString(); step.FailCount.ToString()
         stats.RPS.ToString(); stats.Min.ToString(); stats.Mean.ToString(); stats.Max.ToString()
         stats.Percent50.ToString(); stats.Percent75.ToString()
         stats.Percent95.ToString(); stats.StdDev.ToString()]
        |> HtmlBuilder.toTableRow

module EnvView =
    
    let print (envViewHtml: string, envInfo: EnvironmentInfo) =            
        let row = [envInfo.OS.VersionString; envInfo.DotNetVersion
                   envInfo.Processor; envInfo.ProcessorArchitecture]
                  |> HtmlBuilder.toTableRow        
        envViewHtml.Replace("%env_table%", row)
