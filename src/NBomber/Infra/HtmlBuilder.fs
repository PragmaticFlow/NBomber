module internal NBomber.Infra.HtmlBuilder

open System
open System.Xml.Linq

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