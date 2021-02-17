module internal SpectreConsole

open System.IO

open Spectre.Console

let private create (output: TextWriter) =
    AnsiConsoleSettings(
        ColorSystem = ColorSystemSupport.Detect,
        Interactive = InteractionSupport.No,
        Out = output
    )
    |> AnsiConsole.Create

let render (output: TextWriter) (text) =
    let console = create(output)
    text |> Markup |> console.Render

let writeException (output: TextWriter) (ex) =
    let console = create(output)
    console.WriteException(ex)

let highlightProp (text) =
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
