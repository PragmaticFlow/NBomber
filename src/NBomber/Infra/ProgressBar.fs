module internal NBomber.Infra.ProgressBar

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine
open Spectre.Console

type ProgressTaskConfig = {
    Description: string
    Ticks: float
}

let defaultColumns: ProgressColumn[] =
        [| TaskDescriptionColumn()
           ProgressBarColumn()
           PercentageColumn()
           RemainingTimeColumn()
           SpinnerColumn() |]

let private createProgressTask (ctx: ProgressContext) (config: ProgressTaskConfig) =
    let task = ctx.AddTask(config.Description)

    if config.Ticks > 0.0 then
        task.MaxValue <- config.Ticks
        task.Increment(0.0)
    else
        // set 100% if number of ticks equal to 0
        task.MaxValue <- 1.0
        task.Increment(1.0)

    task

let create (columns) (created: ProgressTask list -> unit) (config: ProgressTaskConfig list) =
    AnsiConsole.Progress()
    |> fun progress -> ProgressExtensions.AutoRefresh(progress, true)
    |> fun progress -> ProgressExtensions.AutoClear(progress, false)
    |> fun progress -> ProgressExtensions.Columns(progress, columns)
    |> fun progress ->
        progress.StartAsync(fun ctx ->
            task {
                config |> Seq.map(createProgressTask ctx) |> List.ofSeq |> created

                while not ctx.IsFinished do
                    do! Task.Delay(TimeSpan.FromMilliseconds(1000.0))
            }
        )

let setDescription (task: ProgressTask) (description: string option) =
    match description with
    | Some description -> task.Description <- description
    | None             -> ()
    task

let tick (task: ProgressTask) (description: string option) =
    task.Increment(1.0)
    setDescription task description 
