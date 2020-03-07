module internal NBomber.DomainServices.Reporting.MdReport

open System
open NBomber.Domain

[<AutoOpen>]
module private Impl =
    type Row2 = string * string

    let sep n =
        System.String('-', n)

    let sep2 l1 l2 =
        sprintf "|-%s-|-%s-|" (sep l1) (sep l2)

    let asMdTable (s: StepStats) =
        let count = sprintf "all = `%i`, OK = `%i`, failed = `%i`" s.RequestCount s.OkCount s.FailCount
        let times = sprintf "RPS = `%i`, min = `%i`, mean = `%i`, max = `%i`" s.RPS s.Min s.Mean s.Max
        let percentile =
            sprintf "50%% = `%i`, 75%% = `%i`, 95%% = `%i`, StdDev = `%i`" s.Percent50 s.Percent75 s.Percent95 s.StdDev

        [ "name", ("`" + s.StepName + "`")
          "request count", count
          "response time", times
          "response time percentile", percentile
          if s.DataTransfer.AllMB > 0.0 then
            "data transfer",
                    sprintf "min = `%.3f Kb`, mean = `%.3f Kb`, max = `%.3f Kb`, all = `%.3f MB`"
                        s.DataTransfer.MinKb s.DataTransfer.MeanKb s.DataTransfer.MaxKb s.DataTransfer.AllMB
          else ()
        ]

    let concatRows (rows: Row2 list) =
        seq {
            let length1 = rows |> List.map (fst >> String.length) |> List.max
            let length2 = rows |> List.map (snd >> String.length) |> List.max
            match rows with
            | (h1, h2) :: tail ->
                yield sprintf "| %-*s | %-*s |" length1 h1 length2 h2
                yield sep2 length1 length2
                for (c1, c2) in tail do
                    yield sprintf "| %-*s | %-*s |" length1 c1 length2 c2
            | _ -> ()
        } |> String.concat Environment.NewLine

    let scenarioHeader (scnStats: ScenarioStats) =
        [ sprintf "# Scenario: `%s`" (scnStats.ScenarioName.Replace('_', ' '))
          ""
          sprintf "- Duration: `%A`" scnStats.Duration
          sprintf "- RPS: `%i`" scnStats.RPS
          ""
        ]
        |> String.concat Environment.NewLine

let print (stats: RawNodeStats) =
    let appendLine s = s + Environment.NewLine

    stats.AllScenariosStats
    |> Seq.collect (fun x ->
        seq {
            scenarioHeader x
            x.StepsStats
            |> List.ofArray
            |> List.collect asMdTable
            |> List.append [ "__step__", "__details__" ]
            |> concatRows
            |> appendLine
        })
    |> String.concat Environment.NewLine
