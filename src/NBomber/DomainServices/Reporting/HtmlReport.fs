module internal NBomber.DomainServices.Reporting.HtmlReport

open System
open System.Text.RegularExpressions

open NBomber.Extensions.InternalExtensions
open NBomber.Extensions.Operator.Option
open NBomber.Contracts.Stats
open NBomber.Infra.Dependency

module AssetsUtils =

    let tryIncludeAsset (tagName) (regex: Regex) (line: string) =
        let m = line |> regex.Match

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

let private applyHtmlReplace (sessionResult: NodeSessionResult) (line: string) =
    let removeLineCommand = "<!-- remove-->"
    let includeViewModelCommand = "<!-- include view model -->"
    let includeAssetCommand = "<!-- include asset -->"

    if line.Contains(removeLineCommand) then
        String.Empty
    else if line.Contains(includeViewModelCommand) then
        $"const viewModel = {sessionResult |> Json.toJson};"
    else if line.Contains(includeAssetCommand) then
        AssetsUtils.tryIncludeStyle(line) |?? AssetsUtils.tryIncludeScript(line)
        |> Option.defaultValue line
    else
        line

let private removeDescription (html: string) =
    html.Substring(html.IndexOf("<!DOCTYPE"))

let print (sessionResult: NodeSessionResult) =
    let applyHtmlReplace = applyHtmlReplace sessionResult

    ResourceManager.readResource("index.html")
    |> Option.map removeDescription
    |> Option.map(fun html -> html.Replace("\r", ""))
    |> Option.map(fun html -> html.Split([| "\n" |], StringSplitOptions.None))
    |> Option.map(fun lines -> lines |> Seq.map applyHtmlReplace)
    |> Option.map String.concatLines
    |> Option.defaultValue String.Empty
