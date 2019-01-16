module internal NBomber.Infra.HtmlBuilder

open System
open System.Xml.Linq

open NBomber.Domain.Errors
open NBomber.Domain.DomainTypes

let toTableCell (rowspan: int) (rawData: 'T) =
    String.Format("""<td rowspan="{0}">{1}</td>""", rowspan, rawData)

let toTableRow (rawData: 'T list) =
    let row = rawData
              |> List.map(fun x -> String.Format("<td>{0}</td>", x))
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

let toListGroupItem (failedAssert) =
    match failedAssert with
    | AssertNotFound (_,assertion) -> 
        match assertion with
        | Step s ->
            let labelStr = if s.Label.IsSome then s.Label.Value else String.Empty
            sprintf "<li class=\"list-group-item list-group-item-danger\">Assertion <strong>'%s'</strong> is not found for step <strong>'%s'</strong></li>" labelStr s.StepName
    | AssertionError (_,assertion,_) ->
        match assertion with
        | Step s ->
            let labelStr = if s.Label.IsSome then s.Label.Value else String.Empty
            sprintf "<li class=\"list-group-item list-group-item-danger\">Failed assertion <strong>'%s'</strong> for step <strong>'%s'</strong></li>" labelStr s.StepName
    | _ -> String.Empty

let toListGroup (failedAsserts) =
    let assertionsStr = failedAsserts |> Array.map(toListGroupItem) |> String.concat(String.Empty)

    if String.IsNullOrEmpty(assertionsStr) then String.Empty
    else sprintf "<ul class=\"list-group\">%s</ul><br/>" assertionsStr