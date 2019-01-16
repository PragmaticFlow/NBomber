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

let formatAssertion (assertNumber: int, assertLabel: string option) =
    if assertLabel.IsSome then sprintf "<strong>%s</strong>" assertLabel.Value
    else sprintf "<strong>#%i</strong>" assertNumber

let toListGroupItem (failedAssert) =
    match failedAssert with
    | AssertNotFound (assertNumber,assertion) -> 
        match assertion with
        | Step s ->
            let labelStr = formatAssertion(assertNumber, s.Label)
            sprintf "<li class=\"list-group-item list-group-item-danger\">Assertion %s is not found for step <strong>%s</strong></li>" labelStr s.StepName
    | AssertionError (assertNumber,assertion,_) ->
        match assertion with
        | Step s ->
            let labelStr = formatAssertion(assertNumber, s.Label)
            sprintf "<li class=\"list-group-item list-group-item-danger\">Failed assertion %s for step <strong>%s</strong></li>" labelStr s.StepName
    | _ -> String.Empty

let toListGroup (failedAsserts) =
    let assertionsStr = failedAsserts |> Array.map(toListGroupItem) |> String.concat(String.Empty)

    if String.IsNullOrEmpty(assertionsStr) then String.Empty
    else sprintf "<ul class=\"list-group\">%s</ul><br/>" assertionsStr