module internal NBomber.DomainServices.Reporting.HtmlReport

open System
open System.Text.RegularExpressions

open NBomber.Contracts
open NBomber.DomainServices
open NBomber.DomainServices.Reporting.ViewModels
open NBomber.Extensions

module internal AssetsHtmlReportHelper =

    let tryIncludeAsset (tagName) (regex: Regex) (line: string) =
        let m = line |> regex.Match

        if m.Success then
            m.Groups.[1].Value.Replace("/", ".")
            |> ResourceManager.tryReadResource
            |> Option.bind(fun x ->
                sprintf "<%s>%s%s%s</%s>" tagName Environment.NewLine x Environment.NewLine tagName
                |> Some)
        else None

    let private styleRegex = Regex("<link[/\s\w=\"\d]*href=['\"]([\.\d\w\\/-]*)['\"][\s\w=\"'/]*>")
    let private scriptRegex = Regex("<script[\s\w=\"'/]*src\s*=\s*['\"]([\w/\.\d\s-]*)[\"']>")

    let inline tryIncludeStyle (line: string) =
        line |> tryIncludeAsset "style" styleRegex

    let inline tryIncludeScript (line: string) =
        line |> tryIncludeAsset "script" scriptRegex

let private mapLine (statsData: string) (line: string) =
    let removeLineCommand = "<!-- remove-->"
    let includeStatsDataCommand = "<!-- include stats data -->"
    let includeAssetCommand = "<!-- include asset -->"

    if removeLineCommand |> line.Contains then
        String.Empty
    else if includeStatsDataCommand |> line.Contains then
        sprintf "const statsData = %s;" statsData
    else if includeAssetCommand |> line.Contains then
        AssetsHtmlReportHelper.tryIncludeStyle(line) |? AssetsHtmlReportHelper.tryIncludeScript(line)
        |> Option.map(fun x -> x)
        |> Option.defaultValue line
    else
        line

let inline private removeDescription (html: string) =
    html.Substring(html.IndexOf("<!DOCTYPE"))

let print (stats: NodeStats) =
    let statsData = stats |> HtmlReportViewModel.map |> HtmlReportViewModel.serialize
    let lineMapper = mapLine statsData

    ResourceManager.tryReadResource("index.html")
    |> Option.map removeDescription
    |> Option.map String.splitLines
    |> Option.map(fun lines -> lines |> Array.map lineMapper)
    |> Option.map String.concatLines
    |> Option.defaultValue String.Empty
