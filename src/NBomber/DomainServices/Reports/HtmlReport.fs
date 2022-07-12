module internal NBomber.DomainServices.Reports.HtmlReport

open System
open System.Text.RegularExpressions

open Serilog
open FsToolkit.ErrorHandling

open NBomber.Extensions.Internal
open NBomber.Contracts.Stats
open NBomber.Infra

module AssetsUtils =

    let tryIncludeAsset (tagName) (regex: Regex) (line: string) =
        let m = line |> regex.Match

        if m.Success then
            m.Groups[1].Value.Replace("/", ".")
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

    if line.Contains removeLineCommand then
        String.Empty
    else if line.Contains includeViewModelCommand then
        $"const viewModel = {JsonExt.serialize sessionResult};"
    else if line.Contains includeAssetCommand then
        line
        |> AssetsUtils.tryIncludeStyle
        |> Option.orElse(AssetsUtils.tryIncludeScript line)
        |> Option.defaultValue line
    else
        line

let private removeDescription (html: string) =
    html.Substring(html.IndexOf("<!DOCTYPE"))

let print (logger: ILogger) (sessionResult: NodeSessionResult) =
    try
        logger.Verbose("HtmlReport.print")

        option {
            let! indexHtml = ResourceManager.readResource "index.html"

            return
                indexHtml
                |> removeDescription
                |> String.replace("\r", "")
                |> String.split [| "\n" |]
                |> Seq.map(applyHtmlReplace sessionResult)
                |> String.concatLines
        }
        |> Option.defaultValue String.Empty
    with
    | ex ->
        logger.Error(ex, "HtmlReport.print failed")
        "Could not generate report"
