module internal rec NBomber.Infra.HtmlBuilder

open System

let printTableRow (rowData: string list) =    
    let row = rowData
              |> List.map(fun x -> String.Format("<td>{0}</td>", x)) 
              |> String.concat(String.Empty)
    "<tr>" + row + "</tr>"