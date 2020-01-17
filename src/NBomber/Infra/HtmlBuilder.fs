module internal NBomber.Infra.HtmlBuilder

open System
open System.Xml.Linq

open NBomber.Extensions

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
    XElement.Parse(rootWrapper).ToString()
        |> String.replace("<root>", "<!DOCTYPE HTML>")
        |> String.replace("</root>", String.Empty)