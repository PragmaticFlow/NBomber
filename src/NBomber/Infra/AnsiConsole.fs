module NBomber.Infra.AnsiConsole

open System

open Spectre.Console
open Spectre.Console.Rendering
open System.IO

let create (output: TextWriter) =
    AnsiConsoleSettings(
        ColorSystem = ColorSystemSupport.Detect,
        Interactive = InteractionSupport.No,
        Out = output
    )
    |> AnsiConsole.Create

let write (text: string) =
    AnsiConsole.Write(text)

let writeException (console: IAnsiConsole) (ex: Exception) =
    console.WriteException(ex)

let render (renderable: IRenderable) =
    AnsiConsole.Render(renderable)

let renderToConsole (console: IAnsiConsole) (renderable: IRenderable) =
    console.Render(renderable)

let highlight (text) =
    $"[lime]{text}[/]"

let highlightMuted (text) =
    $"[grey]{text}[/]"

let highlightVerbose (text) =
    highlightMuted(text)

let highlightDebug (text) =
    $"[silver]{text}[/]"

let highlightInfo (text) =
    $"[deepskyblue1]{text}[/]"

let highlightWarning (text) =
    $"[yellow]{text}[/]"

let highlightError (text) =
    $"[red]{text}[/]"

let highlightFatal (text) =
    $"[maroon]{text}[/]"

let markup (text) =
    Markup(text)

let escapeMarkup (text) =
    Markup.Escape(text)

let addLogo (logo) =
    FigletText(logo) :> IRenderable

let addEmptyLine () =
    Markup(Environment.NewLine) :> IRenderable

let addLine (text) =
    Markup($"{text}{Environment.NewLine}") :> IRenderable

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
