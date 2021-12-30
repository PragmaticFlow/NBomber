module internal NBomber.DomainServices.Reports.ReportHelper

open System

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Extensions.InternalExtensions
open NBomber.Domain

let tryApplyColor (colorize: (string -> string) option) (value: string) =
    colorize |> Option.map(fun color -> color value) |> Option.defaultValue value

module LoadSimulation =

    let private printLine (okColor: (string -> string) option) (name: string) (values: (string * obj) list) =
        values
        |> List.map(fun (key,value) -> $"{key}: {value |> string |> tryApplyColor(okColor)}")
        |> String.concatWithComma
        |> fun argsStr -> $"  - {name}, {argsStr}"

    let print (okColor: (string -> string) option) (simulation: LoadSimulation) =

        let name = LoadTimeLine.getSimulationName simulation

        match simulation with
        | RampConstant (copies, during)
        | KeepConstant (copies, during) ->
            printLine okColor name ["copies", copies; "during", during]

        | RampPerSec (rate, during)
        | InjectPerSec (rate, during) ->
            printLine okColor name ["rate", rate; "during", during]

        | InjectPerSecRandom (minRate, maxRate, during) ->
            printLine okColor name ["minRate", minRate; "maxRate", maxRate; "during", during]

module StatusCodesStats =

    let createTableRows (okColor: (string -> string) option)
                        (errorColor: (string -> string) option)
                        (scnStats: ScenarioStats) =

        let okStatusCodes =
            scnStats.StatusCodes
            |> Seq.choose(fun x ->
                if not x.IsError then
                    let code = x.StatusCode |> string |> tryApplyColor okColor
                    Some [code; x.Count.ToString(); x.Message]
                else
                    None
            )
            |> Seq.toList

        let failStatusCodes =
            scnStats.StatusCodes
            |> Seq.choose(fun x ->
                if x.IsError then
                    let code = x.StatusCode |> string |> tryApplyColor errorColor
                    let msg = x.Message |> tryApplyColor errorColor
                    Some [code; x.Count.ToString(); msg]
                else
                    None
            )
            |> Seq.toList

        let okCodesCount   = scnStats.StatusCodes |> Seq.filter(fun x -> not x.IsError) |> Seq.sumBy(fun x -> x.Count)
        let failCodesCount = scnStats.StatusCodes |> Seq.filter(fun x -> x.IsError)     |> Seq.sumBy(fun x -> x.Count)

        let okNotAvailableStatusCodes =
            if okCodesCount < scnStats.OkCount then
                ["ok (no status)" |> tryApplyColor(okColor); string(scnStats.OkCount - okCodesCount); String.Empty]
                |> List.singleton
            else
                List.Empty

        let failNotAvailableStatusCodes =
            if failCodesCount < scnStats.FailCount then
                ["fail (no status)" |> tryApplyColor(errorColor); string(scnStats.FailCount - failCodesCount); String.Empty]
                |> List.singleton
            else
                List.Empty

        // all status codes
        okNotAvailableStatusCodes @ okStatusCodes @ failNotAvailableStatusCodes @ failStatusCodes
