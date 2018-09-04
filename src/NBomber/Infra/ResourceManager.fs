module internal rec NBomber.Infra.ResourceManager

open System.IO
open System.IO.Compression
open System.Reflection

open Constants

type Assets = {
    MainHtml: string    
    ResultsViewHtml: string    
    EnvViewHtml: string
}

let loadAssets () =
    let assembly = typedefof<Assets>.Assembly
    { MainHtml = readResource(assembly, AssetsHtml + MainHtml)            
      ResultsViewHtml = readResource(assembly, AssetsHtml + ResultsViewHtml)
      EnvViewHtml = readResource(assembly, AssetsHtml + EnvViewHtml) }

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
    let AssetsZip = "NBomber.assets.assets.zip"
    
    [<Literal>]
    let MainHtml = "main.html"

    [<Literal>]
    let EnvViewHtml = "env_view.html"

    [<Literal>]
    let ResultsViewHtml = "results_view.html"