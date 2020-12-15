module internal NBomber.DomainServices.Reporting.HtmlReport

open System
open System.Text.RegularExpressions

open NBomber.Extensions.InternalExtensions
open NBomber.Extensions.Operator.Option
open NBomber.Contracts
open NBomber.DomainServices
open NBomber.DomainServices.Reporting.ViewModels
open NBomber.Infra.Dependency

type ViewModelJson = {
    NBomberInfoJson: string
    TestInfoJson: string
    StatsJson: string
    TimeLineStatsData: string
}

type CustomPluginData = {
    CustomHeaders: string
    CustomJs: string
    CustomHtmlTemplates: string
}

module AssetsUtils =

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

    let tryIncludeStyle (line: string) =
        line |> tryIncludeAsset "style" styleRegex

    let tryIncludeScript (line: string) =
        line |> tryIncludeAsset "script" scriptRegex

let private applyHtmlReplace (viewModelJson: ViewModelJson) (customPluginData: CustomPluginData) (line: string) =
    let removeLineCommand = "<!-- remove-->"
    let includeViewModelCommand = "<!-- include view model -->"
    let includeAssetCommand = "<!-- include asset -->"
    let includeCustomHeaderCommand = "<!-- include custom header -->"
    let includeCustomJsCommand = "<!-- include custom js -->"
    let includeCustomHtmlCommand = "<!-- include custom html template -->"

    if line.Contains(removeLineCommand) then
        String.Empty
    else if line.Contains(includeViewModelCommand) then
        sprintf "const viewModel = { nBomberInfo: %s, testInfo: %s, statsData: %s, timeLineStatsData: %s };"
            viewModelJson.NBomberInfoJson viewModelJson.TestInfoJson viewModelJson.StatsJson viewModelJson.TimeLineStatsData
    else if line.Contains(includeAssetCommand) then
        AssetsUtils.tryIncludeStyle(line) |?? AssetsUtils.tryIncludeScript(line)
        |> Option.map(fun x -> x)
        |> Option.defaultValue line
    else if line.Contains(includeCustomHeaderCommand) then
        customPluginData.CustomHeaders
    else if line.Contains(includeCustomJsCommand) then
        customPluginData.CustomJs
    else if line.Contains(includeCustomHtmlCommand) then
         customPluginData.CustomHtmlTemplates
    else
        line

let inline private removeDescription (html: string) =
    html.Substring(html.IndexOf("<!DOCTYPE"))

let private getCustomHtmlTemplates (pluginStats) =
    pluginStats
    |> PluginStats.getCustomHtmlTemplates
    |> Seq.map(fun (htmlTemplate, pluginName) ->
        sprintf "<div v-if=\"pluginName === '%s'\">%s</div>" pluginName htmlTemplate
    )
    |> String.concatLines

let print (stats: NodeStats) (timeLineStats: (TimeSpan * NodeStats) list) =
    let viewModelJson = {
        NBomberInfoJson = stats.NodeInfo |> NBomberInfoViewModel.create |> Json.toJson
        TestInfoJson = stats.TestInfo |> TestInfoViewModel.create |> Json.toJson
        StatsJson = stats |> NodeStatsViewModel.create |> Json.toJson
        TimeLineStatsData = timeLineStats |> TimeLineStatsViewModel.create |> Json.toJson
    }

    let customPluginData = {
        CustomHeaders = PluginStats.getCustomHeaders(stats.PluginStats)
        CustomJs = PluginStats.getCustomJs(stats.PluginStats)
        CustomHtmlTemplates = getCustomHtmlTemplates(stats.PluginStats)
    }

    let applyHtmlReplace = applyHtmlReplace viewModelJson customPluginData

    ResourceManager.readResource("index.html")
    |> Option.map removeDescription
    |> Option.map(fun html -> html.Replace("\r", ""))
    |> Option.map(fun html -> html.Split([| "\n" |], StringSplitOptions.None))
    |> Option.map(fun lines -> lines |> Seq.map applyHtmlReplace)
    |> Option.map String.concatLines
    |> Option.defaultValue String.Empty
