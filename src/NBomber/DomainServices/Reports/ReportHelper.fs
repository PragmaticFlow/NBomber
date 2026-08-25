module internal NBomber.DomainServices.Reports.ReportHelper

open System
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Contracts.Stats
open NBomber.Extensions.Internal
open NBomber.Domain
open NBomber.Domain.Stats

let printDataKb (highlightTxt: obj -> string) (bytes: int64) =
    $"{bytes |> Converter.fromBytesToKb |> highlightTxt} KB"

let printAllData (highlightTxt: obj -> string) (bytes: int64) =
    $"{bytes |> Converter.fromBytesToMb |> highlightTxt} MB"

let getFullReportsFolderPath (sessionArgs: SessionArgs) =
    let strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location
    let strWorkPath = System.IO.Path.GetDirectoryName strExeFilePath
    let reportFolder = sessionArgs.GetReportFolder()
    System.IO.Path.Combine(strWorkPath, reportFolder)

module StepStats =

    let printStepStatsRow (isOkStats: bool)
                          (okColor: obj -> string)
                          (errorColor: obj -> string)
                          (blueColor: obj -> string)
                          (stepIndex: int)
                          (stats: StepStats) =

        let highlightTxt = if isOkStats then okColor else errorColor
        let printDataKb = printDataKb highlightTxt
        let printAllData = printAllData highlightTxt

        let data = if isOkStats then stats.Ok else stats.Fail

        let rq = data.Request
        let lt = data.Latency
        let dt = data.DataTransfer
        let allReqCount = Statistics.StepStats.getAllRequestCount stats

        let reqCount =
            if isOkStats then $"all = {okColor allReqCount}, ok = {okColor rq.Count}, RPS = {okColor rq.RPS}"
            else $"all = {okColor allReqCount}, fail = {errorColor rq.Count}, RPS = {errorColor rq.RPS}"

        let latencies =
            $"min = {highlightTxt lt.MinMs}, mean = {highlightTxt lt.MeanMs}, max = {highlightTxt lt.MaxMs}, StdDev = {highlightTxt lt.StdDev}"

        let percentiles =
            $"p50 = {highlightTxt lt.Percent50}, p75 = {highlightTxt lt.Percent75}, p95 = {highlightTxt lt.Percent95}, p99 = {highlightTxt lt.Percent99}"

        let dataTransfer =
            $"min = {printDataKb dt.MinBytes}, mean = {printDataKb dt.MeanBytes}, max = {printDataKb dt.MaxBytes}, all = {printAllData dt.AllBytes}"

        [ if stepIndex > 0 then [String.Empty; String.Empty]
          ["name"; blueColor stats.StepName]
          ["request count"; reqCount]
          ["latency, ms"; latencies]
          ["latency percentile, ms"; percentiles]
          if data.DataTransfer.AllBytes > 0 then ["data transfer"; dataTransfer] ]

module LoadSimulation =

    let private printLine (okColor: obj -> string) (name: string) (values: (string * obj) list) =
        values
        |> List.map(fun (key, value) -> $"{key}: {okColor value}")
        |> String.concatWithComma
        |> fun argsStr -> $"  - {okColor name}, {argsStr}"

    let print (okColor: obj -> string) (simulation: LoadSimulation) =

        let name = LoadSimulation.getSimulationName simulation

        match simulation with
        | RampingConstant (copies, during)
        | KeepConstant (copies, during) ->
            printLine okColor name ["copies", copies; "during", during]

        | RampingInject (rate, interval, during)
        | Inject (rate, interval, during) ->
            printLine okColor name ["rate", rate; "interval", interval; "during", during]

        | InjectRandom (minRate, maxRate, interval, during) ->
            printLine okColor name ["minRate", minRate; "maxRate", maxRate; "interval", interval; "during", during]

        | Pause during ->
            printLine okColor name ["during", during]

module StatusCodesStats =

    let createTableRows (okColor: obj -> string)
                        (errorColor: obj -> string)
                        (scnStats: ScenarioStats) =

        let okStatusCodes =
            scnStats.Ok.StatusCodes
            |> Seq.map(fun x -> [okColor x.StatusCode; string x.Count; x.Message])
            |> Seq.toList

        let failStatusCodes =
            scnStats.Fail.StatusCodes
            |> Seq.map(fun x -> [errorColor x.StatusCode; string x.Count; errorColor x.Message])
            |> Seq.toList
        
        let okStatusCount = scnStats.Ok.StatusCodes |> Seq.sumBy(fun x -> x.Count)
        let failStatusCount = scnStats.Fail.StatusCodes |> Seq.sumBy(fun x -> x.Count)
        let noStatusCount = scnStats.AllRequestCount - (okStatusCount + failStatusCount)  
        
        if noStatusCount > 0 then            
            okStatusCodes @ failStatusCodes @ [ [okColor "no status"; string noStatusCount; ""] ]
        else
            okStatusCodes @ failStatusCodes
