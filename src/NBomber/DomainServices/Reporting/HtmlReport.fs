module internal NBomber.DomainServices.Reporting.HtmlReport

open System
open System.Text.RegularExpressions

open NBomber.Extensions.InternalExtensions
open NBomber.Extensions.Operator.Option
open NBomber.Contracts
open NBomber.Infra.Dependency
open NBomber.DomainServices.Reporting
open Newtonsoft.Json

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

let private applyHtmlReplace (nBomberInfoJsonData: string) (testInfoJsonData: string) (statsJsonData: string) (timeLineStatsJsonData: string) (line: string) =
    let removeLineCommand = "<!-- remove-->"
    let includeViewModelCommand = "<!-- include view model -->"
    let includeAssetCommand = "<!-- include asset -->"

    if line.Contains(removeLineCommand) then
        String.Empty
    else if line.Contains(includeViewModelCommand) then
        sprintf "const viewModel = { nBomberInfo: %s, testInfo: %s, statsData: %s, timeLineStatsData: %s };"
                nBomberInfoJsonData testInfoJsonData statsJsonData timeLineStatsJsonData
    else if line.Contains(includeAssetCommand) then
        AssetsUtils.tryIncludeStyle(line) |?? AssetsUtils.tryIncludeScript(line)
        |> Option.map(fun x -> x)
        |> Option.defaultValue line
    else
        line

let inline private removeDescription (html: string) =
    html.Substring(html.IndexOf("<!DOCTYPE"))

let print (dep: IGlobalDependency, testInfo: TestInfo, stats: NodeStats, timeLineStats: (TimeSpan * NodeStats) list) =
    let nBomberInfoJsonData = dep |> AppInfoViewModel.create |> AppInfoViewModel.serializeJson
    let testInfoJsonData = testInfo |> TestInfoViewModel.create |> TestInfoViewModel.serializeJson
    let statsJsonData = stats |> NodeStatsViewModel.create |> NodeStatsViewModel.serializeJson
    let timeLineStatsJsonData = timeLineStats |> TimeLineStatsViewModel.create |> TimeLineStatsViewModel.serializeJson
    let applyHtmlReplace = applyHtmlReplace nBomberInfoJsonData testInfoJsonData statsJsonData timeLineStatsJsonData

    ResourceManager.readResource("index.html")
    |> Option.map removeDescription
    |> Option.map(fun html -> html.Split([| "\n" |], StringSplitOptions.None))
    |> Option.map(fun lines -> lines |> Seq.map applyHtmlReplace)
    |> Option.map String.concatLines
    |> Option.defaultValue String.Empty
