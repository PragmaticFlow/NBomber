module internal NBomber.DomainServices.Reporting.MdReport

open System
open NBomber.Domain
open NBomber.Errors

[<AutoOpen>]
module private Impl =
    type Row2 = string * string

    let sep n =
        System.String('-', n)

    let sep2 l1 l2 =
        sprintf "|-%s-|-%s-|" (sep l1) (sep l2)

    let asMdTable (s : StepStats) =
        let dataInfoAvailable = s.DataTransfer.AllMB > 0.0
        let count = sprintf "all = `%i`, OK = `%i`, failed = `%i`" s.ReqeustCount s.OkCount s.FailCount
        let times = sprintf "RPS = `%i`, min = `%i`, mean = `%i`, max = `%i`" s.RPS s.Min s.Mean s.Max
        let percentile =
                sprintf "50%% = `%i`, 75%% = `%i`, 95%% = `%i`, StdDev = `%i`"
                        s.Percent50 s.Percent75 s.Percent95 s.StdDev
        let transfer =
            if dataInfoAvailable
            then
                sprintf "min = `%.3f Kb`, mean = `%.3f Kb`, max = `%.3f Kb`, all = `%.3f MB`"
                    s.DataTransfer.MinKb s.DataTransfer.MeanKb s.DataTransfer.MaxKb s.DataTransfer.AllMB
            else "min = - , mean = - , max = - , all = -"
        [ "name", ("`" + s.StepName + "`")
          "request count", count
          "response time", times
          "response time percentile", percentile
          "data transfer", transfer
        ]

    let concat (rows : Row2 list) =
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
        sprintf "# Scenario: `%s`\n\n- Duration time: `%A`\n- RPS: `%i`\n- Concurrent Copies: `%i`\n"
                (scnStats.ScenarioName.Replace('_', ' '))
                scnStats.Duration
                scnStats.RPS
                scnStats.ConcurrentCopies

    let getAssertNumberAndLabel (failedAssert: DomainError) =
        match failedAssert with
        | AssertionError (assertNumber,assertion,_) ->
            match assertion with
            | Step s ->
                let assertLabel = if s.Label.IsSome then s.Label.Value else String.Empty
                sprintf "- failed assertion nr `%i`, `%s`" assertNumber assertLabel
        | _ -> String.Empty

let print (stats: NodeStats, failedAsserts: DomainError[]) =

    let assertErrors =
        failedAsserts
        |> Array.map getAssertNumberAndLabel
        |> List.ofArray

    stats.AllScenariosStats
    |> Seq.collect (fun x ->
        seq {
            yield scenarioHeader x
            yield! assertErrors
            yield x.StepsStats
                  |> List.ofArray
                  |> List.collect asMdTable
                  |> List.append [ "__step__", "__details__" ]
                  |> concat
        })
    |> String.concat Environment.NewLine
