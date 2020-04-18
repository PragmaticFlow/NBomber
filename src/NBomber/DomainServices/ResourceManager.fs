module internal NBomber.DomainServices.ResourceManager

open System
open System.IO
open System.Reflection

open NBomber.Contracts

let readResource (assembly: Assembly, resourceName) =
    use stream = assembly.GetManifestResourceStream(resourceName)
    use reader = new StreamReader(stream)
    reader.ReadToEnd()

let saveResource (assembly: Assembly, resourceName, fileName) =
    use stream = assembly.GetManifestResourceStream(resourceName)
    use file = new FileStream(fileName, FileMode.Create, FileAccess.Write)
    stream.CopyTo(file)

module HtmlReportResourceManager =

    let private getAssetFileName (assetType) (resourceName: string) =
        let subName = sprintf "assets.%s." assetType
        let index = resourceName.IndexOf(subName) + subName.Length
        let fileName = resourceName.Substring(index)
        fileName

    let private saveAsset (assetType) (outputDir) (assembly) (resourceName) =
        let assetFileName = resourceName |> getAssetFileName assetType
        let fileName = Path.Combine(outputDir, assetFileName)
        saveResource(assembly, resourceName, fileName)

    let private filterAndSaveAssets (assetType) (outputDir) (assembly) (resourceNames: string[]) =
        let assetsDir = Path.Combine(outputDir, assetType)

        if Directory.Exists assetsDir |> not then
            Directory.CreateDirectory assetsDir |> ignore

        resourceNames
        |> Array.filter(fun x -> x.Contains(sprintf "assets.%s" assetType))
        |> Array.iter(fun x -> x |> saveAsset assetType assetsDir assembly)

    let saveAssets (outputDir: string) =
        let assembly = typedefof<TestContext>.Assembly
        let resourceNames = assembly.GetManifestResourceNames()
        let assetsDir = Path.Combine(outputDir, "assets")

        if Directory.Exists assetsDir then
            Directory.Delete(assetsDir, true)

        Directory.CreateDirectory(assetsDir) |> ignore

        resourceNames |> filterAndSaveAssets "js" assetsDir assembly
        resourceNames |> filterAndSaveAssets "css" assetsDir assembly
        resourceNames |> filterAndSaveAssets "img" assetsDir assembly

    let readIndexHtml () =
        let assembly = typedefof<TestContext>.Assembly

        assembly.GetManifestResourceNames()
        |> Array.tryFind(fun x -> x.Contains("index.html"))
        |> Option.map(fun x -> readResource(assembly, x))
        |> Option.defaultValue String.Empty
