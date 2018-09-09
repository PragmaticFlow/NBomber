module internal rec NBomber.Infra.HtmlBuilder

open System

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