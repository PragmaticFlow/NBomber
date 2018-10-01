module internal NBomber.DomainServices.Reporting.HtmlReport

open System

open NBomber.Domain.StatisticsTypes
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.Infra.ResourceManager

module IndicatorsChart =    

    let print (assets: Assets, viewId: string, label: string,
               latencyCount: LatencyCount, failCount: int) =

        let dataArray = 
            HtmlBuilder.toJsArray([latencyCount.Less800
                                   latencyCount.More800Less1200
                                   latencyCount.More1200
                                   failCount])

        let html = assets.IndicatorsChartHtml
                   |> String.replace("%viewId%", viewId)

        let js = assets.IndicatorsChartJs
                 |> String.replace("%dataArray%", dataArray)
                 |> String.replace("%viewId%", viewId)
                 |> String.replace("%label%", label)
                 
        (html, js)

module NumberReqChart =    

    let print (assets: Assets, viewId: string, okCount: int, failCount: int) =
        let dataArray = HtmlBuilder.toJsArray([okCount; failCount])
        
        let html = assets.NumReqChartHtml
                   |> String.replace("%viewId%", viewId)
        
        let js = assets.NumReqChartJs
                 |> String.replace("%dataArray%", dataArray)
                 |> String.replace("%viewId%", viewId)

        (html, js)

module StatisticsTable =    

    let print (assets: Assets, tableTitle: string, flowsStats: TestFlowStats[]) =
        
        let printStepRow (step, flowName, concurrentCopies) =
            let stats = step.LatencyDetails.Value
            [flowName; concurrentCopies.ToString(); step.StepName; step.OkLatencies.Length.ToString()
             step.OkCount.ToString(); step.FailCount.ToString()
             stats.RPS.ToString(); stats.Min.ToString(); stats.Mean.ToString(); stats.Max.ToString()
             stats.Percent50.ToString(); stats.Percent75.ToString()
             stats.Percent95.ToString(); stats.StdDev.ToString()]
            |> HtmlBuilder.toTableRow

        let printFlowRow (flow) =
            flow.StepsStats
            |> Array.map(fun step -> printStepRow(step, flow.FlowName, flow.ConcurrentCopies))
            |> String.concat(String.Empty)

        let tableBody = flowsStats
                        |> Array.map(printFlowRow)
                        |> String.concat(String.Empty)

        assets.StatisticsTableHtml
        |> String.replace("%table_title%", tableTitle)
        |> String.replace("%table_body%", tableBody)

module ScenarioView =    
    
    let getViewId () = "scenario-view"
    let getLabel () = "Scenario"

    let print (assets: Assets, stats: ScenarioStats) = 
        
        let viewId = getViewId()        
        let (iHtml,iJs) = IndicatorsChart.print(assets, viewId, "Scenario", stats.LatencyCount, stats.FailCount)
        let (nHtml,nJs) = NumberReqChart.print(assets, viewId, stats.OkCount, stats.FailCount)

        let title = "Scenario: " + stats.ScenarioName
        let sHtml = StatisticsTable.print(assets, title, stats.TestFlowsStats)
        
        let js = iJs + nJs
        let html = assets.ScenarioViewHtml
                   |> String.replace("%viewId%", "scenario-view")
                   |> String.replace("%indicators_chart%", iHtml)
                   |> String.replace("%num_req_chart%", nHtml)
                   |> String.replace("%statistics_table%", sHtml)        
        (html, js)

module TestFlowView =

    let createViewId (index: int) = String.Format("test-flow-view-{0}", index)
    let createName (index: int) = String.Format("Test flow {0}", index)

    let print (assets: Assets, viewId: string, stats: TestFlowStats) = 
        let (iHtml,iJs) = IndicatorsChart.print(assets, viewId, "TestFlow", stats.LatencyCount, stats.FailCount)
        let (nHtml,nJs) = NumberReqChart.print(assets, viewId, stats.OkCount, stats.FailCount)
        
        let title = "TestFlow: " + stats.FlowName
        let sHtml = StatisticsTable.print(assets, title, [|stats|])
        
        let js = iJs + nJs
        let html = assets.TestFlowViewHtml
                   |> String.replace("%viewId%", viewId)
                   |> String.replace("%indicators_chart%", iHtml)
                   |> String.replace("%num_req_chart%", nHtml)
                   |> String.replace("%statistics_table%", sHtml)        
        (html, js)

module EnvView =
    
    let print (assets: Assets, envInfo: EnvironmentInfo) =            
        let row = [envInfo.OS.VersionString; envInfo.DotNetVersion
                   envInfo.Processor; envInfo.ProcessorArchitecture]
                  |> HtmlBuilder.toTableRow        
        assets.EnvTableHtml.Replace("%env_table%", row)

module ContentView =

    let print (dep: Dependency, stats: ScenarioStats) =        
        let envHtml = EnvView.print(dep.Assets, dep.EnvironmentInfo)
        let (scenarioHtml,scenarioJs) = ScenarioView.print(dep.Assets, stats)
        
        let allFlowsHtmlJs = 
            stats.TestFlowsStats
            |> Array.mapi(fun i x ->                 
                let viewId = TestFlowView.createViewId(i + 1)                
                TestFlowView.print(dep.Assets, viewId, x))

        let flowHtml = if allFlowsHtmlJs.Length > 1 then
                           allFlowsHtmlJs |> Array.map(fst) |> String.concat(String.Empty)
                       else 
                           String.Empty

        let flowJs = if allFlowsHtmlJs.Length > 1 then
                        allFlowsHtmlJs |> Array.map(snd) |> String.concat(String.Empty)
                     else
                        String.Empty

        //let html = envHtml + scenarioHtml + flowHtml
        let html = scenarioHtml + flowHtml
        let js = scenarioJs + flowJs
        (html, js)

module SideBar =

    let print (assets: Assets, stats: ScenarioStats) =

        let printItem (assets, viewId, name, iconCss) =
            assets.SidebarItemHtml
            |> String.replace("%viewId%", viewId)
            |> String.replace("%name%", name)
            |> String.replace("%iconCss%", iconCss)

        let printTestFlowItem (index) =
            let num = index + 1
            let viewId = TestFlowView.createViewId(num)
            let name = TestFlowView.createName(num)
            let css = "sub_icon fas fa-arrow-right"
            printItem(assets, viewId, name, css)

        let envItem = printItem(assets, "env-view", "Environment", "sub_icon fas fa-globe")
        let scenarioItem = printItem(assets, "scenario-view", "Scenario", "sub_icon fas fa-globe")
        
        let flowItems = 
            if stats.TestFlowsStats.Length > 1 then
                stats.TestFlowsStats 
                |> Array.mapi(fun index _ -> printTestFlowItem(index))
                |> String.concat(String.Empty)
            else
                String.Empty

        let sideBarItems = envItem + scenarioItem + flowItems        
        assets.SidebarHtml.Replace("%sideBar_items%", sideBarItems)

let print (dep: Dependency, stats: ScenarioStats) =
    let sideBar = SideBar.print(dep.Assets, stats)

    let (contentView,js) = ContentView.print(dep, stats)    

    dep.Assets.IndexHtml
    |> String.replace("%sidebar%", sideBar)
    |> String.replace("%content_view%", contentView)                              
    |> HtmlBuilder.toPrettyHtml
    |> String.replace("%js%", js)