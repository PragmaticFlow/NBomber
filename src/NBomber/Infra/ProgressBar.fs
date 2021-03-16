module internal NBomber.Infra.ProgressBar

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine
open Spectre.Console
open Spectre.Console.Rendering

type MultilineColumn () =
    inherit ProgressColumn()

    static member val NewLine = "|" with get

    override _.NoWrap = false

    override _.Render(context: RenderContext, task: ProgressTask, deltaTime: TimeSpan) =
        let text = task.Description.Replace(MultilineColumn.NewLine, Environment.NewLine)
        Markup(text).RightAligned() :> IRenderable

let defaultColumns: ProgressColumn[] =
    [| MultilineColumn()
       ProgressBarColumn()
       PercentageColumn()
       ElapsedTimeColumn()
       SpinnerColumn() |]

type ProgressTaskConfig = {
    Description: string
    Ticks: float
}

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

let create (pbHandler: ProgressTask list -> unit) (config: ProgressTaskConfig list) =

    AnsiConsole.Progress()
    |> fun progressBar -> ProgressExtensions.AutoRefresh(progressBar, true)
    |> fun progressBar -> ProgressExtensions.AutoClear(progressBar, false)
    |> fun progressBar -> ProgressExtensions.Columns(progressBar, defaultColumns)
    |> fun progressBar ->
        progressBar.StartAsync(fun ctx ->
            task {
                config |> List.map(createProgressTask ctx) |> pbHandler

                while not ctx.IsFinished do
                    do! Task.Delay(TimeSpan.FromMilliseconds 1000.0)
            }
        )

let setDescription (description: string) (pbTask: ProgressTask) =
    pbTask.Description <- description
    pbTask

let tick (progressTickInterval: float) (pbTask: ProgressTask) =
    pbTask.Increment(progressTickInterval)

let maxTick (pbTask: ProgressTask) =
    pbTask.Increment(Double.MaxValue)

let defaultTick (pbTask: ProgressTask) =
    pbTask.Increment(1.0)

let getRemainTicks (pbTask: ProgressTask) =
    (pbTask.MaxValue - pbTask.Value)

let stop (pbTask: ProgressTask) =
    pbTask.StopTask()
