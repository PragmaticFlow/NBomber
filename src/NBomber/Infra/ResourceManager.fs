module internal rec NBomber.Infra.ResourceManager

open System.IO
open System.IO.Compression
open System.Reflection

open Constants

type Assets = {
    IndexHtml: string    
    SidebarHtml: string
    SidebarItemHtml: string
    ScenarioViewHtml: string
    TestFlowViewHtml: string
    StatisticsTableHtml: string    
    EnvTableHtml: string
    NumReqChartHtml: string
    NumReqChartJs: string
    IndicatorsChartHtml: string
    IndicatorsChartJs: string
}

let loadAssets () =
    let assembly = typedefof<Assets>.Assembly
    { IndexHtml = readResource(assembly, AssetsHtml + IndexHtml)            
      SidebarHtml = readResource(assembly, AssetsHtml + SidebarHtml)  
      SidebarItemHtml = readResource(assembly, AssetsHtml + SidebarItemHtml)
      ScenarioViewHtml = readResource(assembly, AssetsHtml + ScenarioViewHtml)
      TestFlowViewHtml = readResource(assembly, AssetsHtml + TestFlowViewHtml)
      StatisticsTableHtml = readResource(assembly, AssetsHtml + StatisticsTableHtml)
      EnvTableHtml = readResource(assembly, AssetsHtml + EnvTableHtml)
      NumReqChartHtml = readResource(assembly, AssetsHtml + NumReqChartHtml)
      NumReqChartJs = readResource(assembly, AssetsJs + NumReqChartJs)
      IndicatorsChartHtml = readResource(assembly, AssetsHtml + IndicatorsChartHtml)
      IndicatorsChartJs = readResource(assembly, AssetsJs + IndicatorsChartJs) }

let saveAssets (outputDir: string) =
    let assetsDir = Path.Combine(outputDir, "assets")
    let assetsZip = Path.Combine(outputDir, "assets.zip")    
    
    if not(Directory.Exists assetsDir) then
        let assembly = typedefof<Assets>.Assembly
        saveResource(assembly, AssetsZip, assetsZip)
        ZipFile.ExtractToDirectory(assetsZip, assetsDir)
        File.Delete(assetsZip)

let private saveResource (assembly: Assembly, resourceName, outputFilePath) =
    use stream = assembly.GetManifestResourceStream(resourceName)
    use file = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write)
    stream.CopyTo(file)    

let private readResource (assembly: Assembly, resourceName) =
    use stream = assembly.GetManifestResourceStream(resourceName)    
    use reader = new StreamReader(stream)
    reader.ReadToEnd()

module Constants =    

    [<Literal>]
    let AssetsHtml = "NBomber.assets.html."

    [<Literal>]
    let AssetsJs = "NBomber.assets.js."

    [<Literal>]
    let AssetsZip = "NBomber.assets.assets.zip"
    
    [<Literal>]
    let IndexHtml = "index.html"

    [<Literal>]
    let SidebarHtml = "sidebar.html"

    [<Literal>]
    let SidebarItemHtml = "sidebar_item.html"

    [<Literal>]
    let ScenarioViewHtml = "scenario_view.html"    

    [<Literal>]
    let TestFlowViewHtml = "test_flow_view.html"        

    [<Literal>]
    let EnvTableHtml = "env_table.html"

    [<Literal>]
    let StatisticsTableHtml = "statistics_table.html"

    [<Literal>]
    let NumReqChartHtml = "num_req_chart.html"

    [<Literal>]
    let NumReqChartJs = "num_req_chart.js"

    [<Literal>]
    let IndicatorsChartHtml = "indicators_chart.html"

    [<Literal>]
    let IndicatorsChartJs = "indicators_chart.js"