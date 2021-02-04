module internal NBomber.Infra.Console

open System

open Spectre.Console
open Spectre.Console.Rendering

let private console =
    AnsiConsoleSettings(
        ColorSystem = ColorSystemSupport.Detect,
        Interactive = InteractionSupport.No)
    |> AnsiConsole.Create

let render (renderable: IRenderable) =
    console.Render(renderable)

let highlight (text) =
    $"[lime]{text}[/]"

let highlightInfo (text) =
    $"[dodgerblue1]{text}[/]"

let highlightError (text) =
    $"[red]{text}[/]"

let highlightWarning (text) =
    $"[yellow]{text}[/]"

let escapeMarkup (text) =
    Markup.Escape(text)

let addLogo (logo) =
    FigletText(logo) :> IRenderable

let addEmptyLine () =
    Markup(Environment.NewLine) :> IRenderable

let addLine (header) =
    Markup($"{header}{Environment.NewLine}") :> IRenderable

let addHeader (header) =
    let rule = Rule(header)
    rule.Centered() |> ignore
    rule :> IRenderable

let addList (items: string seq seq) =
    items
    |> Seq.mapi(fun i renderables ->
        let listItems =
            renderables
            |> Seq.map(fun renderable -> [Markup(renderable) :> IRenderable; addEmptyLine()])
            |> Seq.concat

        [ if i > 0 then addEmptyLine()
          yield! listItems ]
    )
    |> Seq.concat
    |> List.ofSeq

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
