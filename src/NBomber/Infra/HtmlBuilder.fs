module internal NBomber.Infra.HtmlBuilder

open System
open System.Xml.Linq

open NBomber.Domain

let htmlEncode (t: 'T) = System.Web.HttpUtility.HtmlEncode t

let toTableCell rowspan rawData =
    sprintf """<td rowspan="%i">%s</td>""" rowspan (htmlEncode rawData)

let toTableRow (rawData: 'T list) =
    let row = rawData
              |> List.map(htmlEncode >> sprintf "<td>%s</td>")
              |> String.concat(String.Empty)
    "<tr>" + row + "</tr>"

let toJsArray (rawData: 'T list) =
    let dataWithCommas = rawData
                         |> List.map(fun x -> String.Format("{0}, ", x))
                         |> String.concat(String.Empty)
    "[" + dataWithCommas + "]"

let toPrettyHtml (html: string) =
    let newHtml = html.Replace("<!DOCTYPE HTML>", String.Empty)
    let rootWrapper = "<root>" + newHtml + "</root>"
    XElement.Parse(rootWrapper)
            .ToString()
            |> String.replace("<root>", "<!DOCTYPE HTML>")
            |> String.replace("</root>", String.Empty)

let formatAssertion assertNumber assertLabel =
    match assertLabel with
    | Some value -> value |> htmlEncode |> sprintf "<strong>%s</strong>"
    | None -> sprintf "<strong>#%i</strong>" assertNumber

let toListGroupItem (failedAssert: DomainError) =
    match failedAssert with
    | AssertNotFound (assertNumber,Step s) ->
        let assertLabel = formatAssertion assertNumber s.Label
        let stepName = htmlEncode s.StepName
        sprintf """<li class="list-group-item list-group-item-danger">Assertion %s is not found for step <strong>%s</strong></li>""" assertLabel stepName
    | AssertionError (assertNumber,Step s, _) ->
        let assertLabel = formatAssertion assertNumber s.Label
        let stepName = htmlEncode s.StepName
        sprintf """<li class="list-group-item list-group-item-danger">Failed assertion %s for step <strong>%s</strong></li>""" assertLabel stepName
    | _ -> String.Empty

let toListGroup (failedAsserts: DomainError[]) =
    failedAsserts
    |> Array.map toListGroupItem
    |> String.concat String.Empty
    |> sprintf """<ul class="list-group">%s</ul><br/>"""