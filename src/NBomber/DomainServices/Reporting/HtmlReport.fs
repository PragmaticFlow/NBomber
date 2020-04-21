module internal NBomber.DomainServices.Reporting.HtmlReport

open System

open System.Text.RegularExpressions
open NBomber.Contracts
open NBomber.DomainServices
open NBomber.Extensions.Extensions

module internal StatsDataHtmlReportHelper =
    let getStatsData (stats: NodeStats) =
        "{}"

module internal AssetsHtmlReportHelper =

    let tryRead (tag) (regex: Regex) (line: string) =
        let m = line |> regex.Match

        if m.Success then
            m.Groups.[1].Value.Replace("/", ".")
            |> ResourceManager.tryReadResource
            |> Option.bind(fun x ->
                sprintf "<%s>%s%s%s</%s>" tag Environment.NewLine x Environment.NewLine tag
                |> Some)
        else None

    let private styleRegex = Regex("<link[/\s\w=\"\d]*href=['\"]([\.\d\w\\/-]*)['\"][\s\w=\"'/]*>")
    let private scriptRegex = Regex("<script[\s\w=\"'/]*src\s*=\s*['\"]([\w/\.\d\s-]*)[\"']>")

    let inline tryReadStyle (line: string) =
        line |> tryRead "style" styleRegex

    let inline tryReadScript (line: string) =
        line |> tryRead "script" scriptRegex

let private executeCommands (statsData: string) (line: string) =
    let removeLineCommand = "<!-- remove-->"
    let includeStatsDataCommand = "<!-- include stats data -->"
    let includeAssetCommand = "<!-- include asset -->"

    if removeLineCommand |> line.Contains then
        String.Empty
    else if includeStatsDataCommand |> line.Contains then
        sprintf "var statsData = %s;" statsData
    else if includeAssetCommand |> line.Contains then
        AssetsHtmlReportHelper.tryReadStyle(line) |? AssetsHtmlReportHelper.tryReadScript(line)
        |> Option.map(fun x -> x)
        |> Option.defaultValue line
    else
        line

let print (stats: NodeStats) =
    let statsData = stats |> StatsDataHtmlReportHelper.getStatsData
    let execCommands = executeCommands statsData

    ResourceManager.tryReadResource("index.html")
    |> Option.map String.splitLines
    |> Option.map(fun x -> x |> Array.map execCommands)
    |> Option.map String.concatLines
    |> Option.defaultValue String.Empty
