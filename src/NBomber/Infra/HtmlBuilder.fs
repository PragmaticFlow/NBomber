module internal rec NBomber.Infra.HtmlBuilder

open System

let printTableRow (rowData: string[]) =
    
    let row = rowData
              |> Array.map(fun x -> String.Format("<td>{0}</td>", x)) 
              |> String.concat(String.Empty)
    
    "<tr>" + row + "</tr>"