module internal NBomber.DomainServices.Reporting.HtmlReport

open System
open System.Text.RegularExpressions

open NBomber.Contracts
open NBomber.Infra.Dependency
open NBomber.DomainServices.Reporting.ViewModels
open NBomber.Extensions

module internal AssetsUtils =

    let tryIncludeAsset (tagName) (regex: Regex) (line: string) =
        let m = line |> regex.Match

        if m.Success then
            m.Groups.[1].Value.Replace("/", ".")
            |> ResourceManager.readResource
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

let private mapLine (statsJsonData: string) (timeLineStatsJsonData: string) (line: string) =
    let removeLineCommand = "<!-- remove-->"
    let includeStatsDataCommand = "<!-- include stats data -->"
    let includeTimeLineStatsDataCommand = "<!-- include time line stats data -->"
    let includeAssetCommand = "<!-- include asset -->"

    if removeLineCommand |> line.Contains then
        String.Empty
    else if includeStatsDataCommand |> line.Contains then
        sprintf "const statsData = %s;" statsJsonData
    else if includeTimeLineStatsDataCommand |> line.Contains then
        sprintf "const timeLineStatsData = %s;" timeLineStatsJsonData
    else if includeAssetCommand |> line.Contains then
        AssetsUtils.tryIncludeStyle(line) |?? AssetsUtils.tryIncludeScript(line)
        |> Option.map(fun x -> x)
        |> Option.defaultValue line
    else
        line

let inline private removeDescription (html: string) =
    html.Substring(html.IndexOf("<!DOCTYPE"))

let print (stats: NodeStats, timeLineStats: (TimeSpan * NodeStats) list) =
    let statsJsonData = stats |> NodeStatsViewModel.create |> NodeStatsViewModel.serializeJson
    let timeLineStatsJsonData = timeLineStats |> TimeLineStatsViewModel.create |> TimeLineStatsViewModel.serializeJson
    let lineMapper = mapLine statsJsonData timeLineStatsJsonData

    ResourceManager.readResource("index.html")
    |> Option.map removeDescription
    |> Option.map String.splitLines
    |> Option.map(fun lines -> lines |> Array.map lineMapper)
    |> Option.map String.concatLines
    |> Option.defaultValue String.Empty
