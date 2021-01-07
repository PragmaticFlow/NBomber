module internal NBomber.DomainServices.Reporting.HtmlReport

open System
open System.Text.RegularExpressions

open NBomber.Domain.HintsAnalyzer
open NBomber.DomainServices
open NBomber.Extensions.InternalExtensions
open NBomber.Extensions.Operator.Option
open NBomber.Contracts
open NBomber.DomainServices.Reporting.ViewModels
open NBomber.Infra.Dependency

module AssetsUtils =

    let tryIncludeAsset (tagName) (regex: Regex) (line: string) =
        let m = regex.Match(line)

        if m.Success then
            m.Groups.[1].Value.Replace("/", ".")
            |> ResourceManager.readResource
            |> Option.map(fun resource -> $"<{tagName}>{resource}</{tagName}>")
        else None

    let private styleRegex = Regex("<link[/\s\w=\"\d]*href=['\"]([\.\d\w\\/-]*)['\"][\s\w=\"'/]*>")
    let private scriptRegex = Regex("<script[\s\w=\"'/]*src\s*=\s*['\"]([\w/\.\d\s-]*)[\"']>")

    let tryIncludeStyle (line: string) =
        line |> tryIncludeAsset "style" styleRegex

    let tryIncludeScript (line: string) =
        line |> tryIncludeAsset "script" scriptRegex

let private applyHtmlReplace (viewModel: HtmlReportViewModel) (customHead: string) (customBody: string) (line: string) =
    let removeLineCommand = "<!-- remove -->"
    let includeViewModelCommand = "<!-- include view model -->"
    let includeAssetCommand = "<!-- include asset -->"
    let includeCustomHeadCommand = "<!-- include custom head -->"
    let includeCustomBodyCommand = "<!-- include custom body -->"

    if line.Contains(removeLineCommand) then
        String.Empty
    else if line.Contains(includeViewModelCommand) then
        $"const viewModel = {viewModel |> Json.toJson};"
    else if line.Contains(includeAssetCommand) then
        AssetsUtils.tryIncludeStyle(line) |?? AssetsUtils.tryIncludeScript(line)
        |> Option.defaultValue line
    else if line.Contains(includeCustomHeadCommand) then
        customHead
    else if line.Contains(includeCustomBodyCommand) then
        customBody
    else
        line

let inline private removeDescription (html: string) =
    html.Substring(html.IndexOf("<!DOCTYPE"))

let print (stats: NodeStats) (timeLineStats: (TimeSpan * NodeStats) list) (hints: HintResult list) =
    //todo: use PascalCase
    let viewModel = {
        nBomberInfo = stats.NodeInfo |> NBomberInfoViewModel.create
        testInfo = stats.TestInfo |> TestInfoViewModel.create
        statsData = stats |> NodeStatsViewModel.create
        timeLineStatsData = timeLineStats |> TimeLineStatsViewModel.create
        hints = hints |> HintsViewModel.create
    }

    let customHead = PluginReport.getHtmlReportHead(stats.PluginStats)
    let customBody = PluginReport.getHtmlReportBody(stats.PluginStats)
    let applyHtmlReplace = applyHtmlReplace viewModel customHead customBody

    ResourceManager.readResource("index.html")
    |> Option.map removeDescription
    |> Option.map(fun html -> html.Replace("\r", String.Empty))
    |> Option.map(fun html -> html.Split([| "\n" |], StringSplitOptions.None))
    |> Option.map(Seq.map applyHtmlReplace)
    |> Option.map String.concatLines
    |> Option.defaultValue String.Empty
