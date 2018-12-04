module internal NBomber.Infra.ResourceManager

open System.IO
open System.IO.Compression
open System.Reflection

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
    let GlobalViewHtml = "global_view.html"    

    [<Literal>]
    let ScenarioViewHtml = "scenario_view.html"        

    [<Literal>]
    let EnvTableHtml = "env_table.html"

    [<Literal>]
    let EnvViewHtml = "env_view.html"    

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

type Assets = {
    IndexHtml: string    
    SidebarHtml: string
    SidebarItemHtml: string
    GlobalViewHtml: string
    ScenarioViewHtml: string
    StatisticsTableHtml: string    
    EnvTableHtml: string
    EnvViewHtml: string
    NumReqChartHtml: string
    NumReqChartJs: string
    IndicatorsChartHtml: string
    IndicatorsChartJs: string
}

let loadAssets () =

    let readResource (assembly: Assembly, resourceName) =
        use stream = assembly.GetManifestResourceStream(resourceName)    
        use reader = new StreamReader(stream)
        reader.ReadToEnd()

    let assembly = typedefof<Assets>.Assembly

    { IndexHtml = readResource(assembly, Constants.AssetsHtml + Constants.IndexHtml)            
      SidebarHtml = readResource(assembly, Constants.AssetsHtml + Constants.SidebarHtml)  
      SidebarItemHtml = readResource(assembly, Constants.AssetsHtml + Constants.SidebarItemHtml)
      GlobalViewHtml = readResource(assembly, Constants.AssetsHtml + Constants.GlobalViewHtml)
      ScenarioViewHtml = readResource(assembly, Constants.AssetsHtml + Constants.ScenarioViewHtml)
      StatisticsTableHtml = readResource(assembly, Constants.AssetsHtml + Constants.StatisticsTableHtml)
      EnvTableHtml = readResource(assembly, Constants.AssetsHtml + Constants.EnvTableHtml)
      EnvViewHtml = readResource(assembly, Constants.AssetsHtml + Constants.EnvViewHtml)
      NumReqChartHtml = readResource(assembly, Constants.AssetsHtml + Constants.NumReqChartHtml)
      NumReqChartJs = readResource(assembly, Constants.AssetsJs + Constants.NumReqChartJs)
      IndicatorsChartHtml = readResource(assembly, Constants.AssetsHtml + Constants.IndicatorsChartHtml)
      IndicatorsChartJs = readResource(assembly, Constants.AssetsJs + Constants.IndicatorsChartJs) }

let saveAssets (outputDir: string) =

    let assetsDir = Path.Combine(outputDir, "assets")
    let assetsZip = Path.Combine(outputDir, "assets.zip")    
    
    let saveResource (assembly: Assembly, resourceName, outputFilePath) =
        use stream = assembly.GetManifestResourceStream(resourceName)
        use file = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write)
        stream.CopyTo(file)
        stream.Close()
        file.Close()        

    if not(Directory.Exists assetsDir) then
        let assembly = typedefof<Assets>.Assembly
        saveResource(assembly, Constants.AssetsZip, assetsZip)
        ZipFile.ExtractToDirectory(assetsZip, assetsDir)
        File.Delete(assetsZip)