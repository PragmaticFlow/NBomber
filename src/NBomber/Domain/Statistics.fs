module internal NBomber.Domain.Statistics

open System
open System.Data

open HdrHistogram
open Nessos.Streams
open FSharp.UMX

open NBomber.Extensions.InternalExtensions
open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes

let buildHistogram (latencies: Stream<int>) =
    let histogram = LongHistogram(TimeStamp.Hours(24), 3)
    latencies |> Stream.filter(fun x -> x > 0)
              |> Stream.iter(fun x -> x |> int64 |> histogram.RecordValue)
    histogram

let calcRPS (executionTime: TimeSpan) (latencies: Stream<Latency>) =

    let allLatenciesIn1SecCount = latencies |> Stream.filter(fun x -> x <= 1_000) |> Stream.length

    let totalSec = if executionTime.TotalSeconds < 1.0 then 1.0
                   else executionTime.TotalSeconds

    allLatenciesIn1SecCount / int(totalSec)

let calcMin (latencies: Stream<Latency>) =
    latencies |> Stream.minOrDefault 0

let calcMean (latencies: Stream<Latency>) =
    latencies
    |> Stream.map float
    |> Stream.averageOrDefault 0.0 |> int

let calcMax (latencies: Stream<Latency>) =
    latencies |> Stream.maxOrDefault 0

let calcPercentile (percentile: float) (histogram: LongHistogram) =
    if histogram.TotalCount > 0L then
        percentile |> histogram.GetValueAtPercentile |> int
    else 0

let calcStdDev (histogram: LongHistogram) =
    if histogram.TotalCount > 0L then
        histogram.GetStdDeviation() |> Math.Round |> int
    else
        0

let fromBytesToKb (sizeBytes: int) =
    if sizeBytes > 0 then float(sizeBytes) / 1024.0
    else 0.0
    |> UMX.tag<kb>

let fromKbToMb (sizeKb: float<kb>) =
    let size = % sizeKb
    if size > 0.0 then size / 1024.0
    else 0.0
    |> UMX.tag<mb>

let toMB = fromBytesToKb >> fromKbToMb

let calcAllMB (sizesBytes: Stream<int>) =
    sizesBytes
    |> Stream.fold(fun sizeMb sizeByte -> sizeMb + toMB sizeByte) (UMX.tag<mb> 0.0)

let roundResult (value: float) =
    let result = Math.Round(value, 2)
    if result > 0.01 then result
    else Math.Round(value, 4)
    |> UMX.tag

module ErrorStats =

    let create (stepResults: RawStepResults) =
        stepResults.Responses
        |> Stream.filter(fun x -> x.Response.Exception.IsSome)
        |> Stream.groupBy(fun x -> x.Response.ErrorCode)
        |> Stream.map(fun (code,errorResponses) ->
            let ex = errorResponses |> Seq.head |> fun x -> x.Response.Exception.Value
            { ErrorCode = code
              ShortMessage = ex.Message
              Message = ex.ToString()
              Count = errorResponses |> Seq.length }
        )

    let merge (stepStats: Stream<RawStepStats>) =
        stepStats
        |> Stream.collect(fun x -> x.ErrorStats)
        |> Stream.groupBy(fun x -> x.ErrorCode)
        |> Stream.map(fun (code,errorStats) ->
            { ErrorCode = code
              ShortMessage = errorStats |> Seq.head |> fun x -> x.ShortMessage
              Message = errorStats |> Seq.head |> fun x -> x.Message
              Count = errorStats |> Seq.sumBy(fun x -> x.Count) }
        )

module StepResults =

    let calcDataTransfer (responses: Stream<StepResponse>) =
        let allSizesBytes =
            responses
            |> Stream.choose(fun x ->
                match x.Response.SizeBytes > 0 with
                | true  -> Some x.Response.SizeBytes
                | false -> None)

        { MinKb  = allSizesBytes |> calcMin |> fromBytesToKb
          MeanKb = allSizesBytes |> calcMean |> fromBytesToKb
          MaxKb  = allSizesBytes |> calcMax |> fromBytesToKb
          AllMB  = allSizesBytes |> calcAllMB }

    let mergeTraffic (counts: Stream<DataTransferCount>) =

        { MinKb  = counts |> Stream.map(fun x -> % x.MinKb) |> Stream.minOrDefault 0.0 |> roundResult
          MeanKb = counts |> Stream.map(fun x -> % x.MeanKb) |> Stream.averageOrDefault 0.0 |> roundResult
          MaxKb  = counts |> Stream.map(fun x -> % x.MaxKb) |> Stream.maxOrDefault 0.0 |> roundResult
          AllMB  = counts |> Stream.sumBy(fun x -> % x.AllMB) |> roundResult }

    let merge (stepsResults: Stream<RawStepResults>) =
        stepsResults
        |> Stream.groupBy(fun x -> x.StepName)
        |> Stream.map(fun (stName, results) ->
            let resultsStream = results |> Stream.ofSeq
            let dataTransfer = resultsStream |> Stream.map(fun x -> x.DataTransfer) |> mergeTraffic
            { StepName = stName
              Responses = resultsStream |> Stream.collect(fun x -> x.Responses)
              DataTransfer = dataTransfer })

    let create (stepName: string, responses: Stream<StepResponse>) =
        { StepName = stepName
          Responses = responses
          DataTransfer = calcDataTransfer(responses) }

module RawStepStats =

    let create (executionTime: TimeSpan) (stepResults: RawStepResults) =
        let okLatencies = stepResults.Responses |> Stream.choose(fun x -> if x.Response.Exception.IsNone then Some x.LatencyMs else None)
        let histogram = buildHistogram okLatencies

        let requestCount = stepResults.Responses |> Stream.length
        let okCount = okLatencies |> Stream.length
        let failCount = requestCount - okCount

        { StepName = stepResults.StepName
          OkLatencies = okLatencies
          RequestCount = requestCount
          OkCount = okCount
          FailCount = failCount
          RPS = calcRPS executionTime okLatencies
          Min = calcMin okLatencies
          Mean = calcMean okLatencies
          Max = calcMax okLatencies
          Percent50 = calcPercentile 50.0 histogram
          Percent75 = calcPercentile 75.0 histogram
          Percent95 = calcPercentile 95.0 histogram
          Percent99 = calcPercentile 99.0 histogram
          StdDev = calcStdDev histogram
          DataTransfer = stepResults.DataTransfer
          ErrorStats = ErrorStats.create stepResults }

    let merge (stepsStats: Stream<RawStepStats>) (executionTime: TimeSpan) =
        stepsStats
        |> Stream.groupBy(fun x -> x.StepName)
        |> Stream.map(fun (name, stats) ->
            let statsStream = stats |> Stream.ofSeq
            let dataTransfer = statsStream |> Stream.map(fun x -> x.DataTransfer) |> StepResults.mergeTraffic
            let okLatencies = statsStream |> Stream.collect(fun x -> x.OkLatencies)
            let failCount = statsStream |> Stream.sumBy(fun x -> x.FailCount)
            let histogram = buildHistogram okLatencies

            let okLatenciesCount = okLatencies |> Stream.length

            { StepName = name
              OkLatencies = okLatencies
              RequestCount = okLatenciesCount + failCount
              OkCount = okLatenciesCount
              FailCount = failCount
              RPS = calcRPS executionTime okLatencies
              Min = calcMin okLatencies
              Mean = calcMean okLatencies
              Max = calcMax okLatencies
              Percent50 = calcPercentile 50.0 histogram
              Percent75 = calcPercentile 75.0 histogram
              Percent95 = calcPercentile 95.0 histogram
              Percent99 = calcPercentile 99.0 histogram
              StdDev = calcStdDev histogram
              DataTransfer = dataTransfer
              ErrorStats = ErrorStats.merge statsStream })

module RawScenarioStats =

    let calcLatencyCount (stepsStats: Stream<RawStepStats>) =
        let a = stepsStats |> Stream.collect(fun x -> x.OkLatencies |> Stream.filter(fun x -> x < 800))
        let b = stepsStats |> Stream.collect(fun x -> x.OkLatencies |> Stream.filter(fun x -> x > 800 && x < 1200))
        let c = stepsStats |> Stream.collect(fun x -> x.OkLatencies |> Stream.filter(fun x -> x > 1200))
        { Less800 = a |> Stream.length
          More800Less1200 = b |> Stream.length
          More1200 = c |> Stream.length }

    let createByStepStats (scnName: ScenarioName) (executionTime: TimeSpan)
                          (simulationStats: LoadSimulationStats)
                          (stepsStats: Stream<RawStepStats>) =

        let mergedStepsStats = RawStepStats.merge stepsStats executionTime
        { ScenarioName = scnName
          RequestCount = mergedStepsStats |> Stream.sumBy(fun x -> x.RequestCount)
          OkCount = mergedStepsStats |> Stream.sumBy(fun x -> x.OkCount)
          FailCount = mergedStepsStats |> Stream.sumBy(fun x -> x.FailCount)
          AllDataMB = mergedStepsStats |> Stream.sumBy(fun x -> % x.DataTransfer.AllMB) |> roundResult
          LatencyCount = mergedStepsStats |> calcLatencyCount
          RawStepsStats = RawStepStats.merge stepsStats executionTime
          LoadSimulationStats = simulationStats
          Duration = executionTime
          ErrorStats = ErrorStats.merge mergedStepsStats }

    let create (scenario: Scenario) (executionTime: TimeSpan)
               (simulationStats: LoadSimulationStats)
               (stepsResults: Stream<RawStepResults>) =

        stepsResults
        |> StepResults.merge
        |> Stream.map(RawStepStats.create executionTime)
        |> createByStepStats scenario.ScenarioName executionTime simulationStats

module NodeStats =

    let mapStepStats (stepStats: Stream<RawStepStats>) =
        stepStats
        |> Stream.map(fun x ->
            { StepName = x.StepName
              RequestCount = x.RequestCount
              OkCount = x.OkCount
              FailCount = x.FailCount
              Min = x.Min
              Mean = x.Mean
              Max = x.Max
              RPS = x.RPS
              Percent50 = x.Percent50
              Percent75 = x.Percent75
              Percent95 = x.Percent95
              Percent99 = x.Percent99
              StdDev = x.StdDev
              MinDataKb = % x.DataTransfer.MinKb
              MeanDataKb = % x.DataTransfer.MeanKb
              MaxDataKb = % x.DataTransfer.MaxKb
              AllDataMB = % x.DataTransfer.AllMB
              ErrorStats = x.ErrorStats |> Stream.toArray }
        )

    let mapScenarioStats (scnStats: Stream<RawScenarioStats>) =
        scnStats
        |> Stream.map(fun x ->
            { ScenarioName = x.ScenarioName
              RequestCount = x.RequestCount
              OkCount = x.OkCount
              FailCount = x.FailCount
              AllDataMB = x.AllDataMB
              StepStats = x.RawStepsStats |> mapStepStats |> Stream.toArray
              LatencyCount = x.LatencyCount
              LoadSimulationStats = x.LoadSimulationStats
              ErrorStats = x.ErrorStats |> Stream.toArray
              Duration = x.Duration }
        )

    let create (testInfo: TestInfo) (nodeInfo: NodeInfo) (scnStats: Stream<RawScenarioStats>) (pluginStats: Stream<DataSet>) = {
        RequestCount = scnStats |> Stream.sumBy(fun x -> x.RequestCount)
        OkCount = scnStats |> Stream.sumBy(fun x -> x.OkCount)
        FailCount = scnStats |> Stream.sumBy(fun x -> x.FailCount)
        AllDataMB = scnStats |> Stream.sumBy(fun x -> x.AllDataMB)
        ScenarioStats = scnStats |> mapScenarioStats |> Stream.toArray
        PluginStats = pluginStats |> Stream.toArray
        NodeInfo = nodeInfo
        TestInfo = testInfo
        ReportFiles = Array.empty
    }
