module internal NBomber.Infra.Console

open System

open Spectre.Console
open Spectre.Console.Rendering

let escapeMarkup (text: string) =
    Markup.Escape(text)

let render (renderable: IRenderable) =
    AnsiConsole.Render(renderable)

let okColor (text: 'T) =
    $"[lime]{text}[/]"

let warningColor (text: 'T) =
    $"[yellow]{text}[/]"

let errorColor (text: 'T) =
    $"[red]{text}[/]"

let blueColor (text: 'T) =
    $"[deepskyblue1]{text}[/]"

let bold (text: 'T) =
    $"[bold]{text}[/]"

let addLine (text: string) =
    Markup($"{text}{Environment.NewLine}") :> IRenderable

let addLogo (logo: string) =
    FigletText(logo) :> IRenderable

let addHeader (header: string) =
    let rule = Rule(header)
    rule.Centered() |> ignore
    rule :> IRenderable

let addList (items: string seq seq) =
    items
    |> Seq.mapi(fun i renderables ->
        let listItems =
            renderables
            |> Seq.map(fun renderable -> [Markup(renderable) :> IRenderable; addLine String.Empty])
            |> Seq.concat

        [ if i > 0 then addLine String.Empty
          yield! listItems ]
    )
    |> List.concat

let addTable (headers: string list) (rows: string list list) =
    let table = Table()
    table.Border <- TableBorder.Square

    headers
    |> Seq.iteri(fun i header ->
        let col = TableColumn(header)

        if i = 0 then
            col.RightAligned() |> ignore
        elif i = headers.Length - 1 then
            col.LeftAligned() |> ignore
        else
            col.Centered() |> ignore

        table.AddColumn(col) |> ignore
    )

    rows
    |> Seq.iter(fun row ->
        row
        |> Seq.map(fun col -> Markup(col) :> IRenderable)
        |> table.AddRow
        |> ignore
    )

    table :> IRenderable
