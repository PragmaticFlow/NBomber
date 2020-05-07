module internal NBomber.Domain.Statistics

open System
open System.Data

open HdrHistogram

open NBomber.Extensions
open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes

let buildHistogram (latencies) =
    let histogram = LongHistogram(TimeStamp.Hours(24), 3)
    latencies |> Array.filter(fun x -> x > 0)
              |> Array.iter(fun x -> x |> int64 |> histogram.RecordValue)
    histogram

let calcRPS (latencies: Latency[], executionTime: TimeSpan) =

    let allLatenciesIn1SecCount = latencies |> Seq.filter(fun x -> x <= 1_000) |> Seq.length

    let totalSec = if executionTime.TotalSeconds < 1.0 then 1.0
                   else executionTime.TotalSeconds

    allLatenciesIn1SecCount / int(totalSec)

let calcMin (latencies: Latency[]) =
    latencies |> Array.minOrDefault 0

let calcMean (latencies: Latency[]) =
    latencies
    |> Array.map float
    |> Array.averageOrDefault 0.0 |> int

let calcMax (latencies: Latency[]) =
    Array.maxOrDefault 0 latencies

let calcPercentile (histogram: LongHistogram, percentile: float) =
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

let fromKbToMb (sizeKb: float) =
    if sizeKb > 0.0 then sizeKb / 1024.0
    else 0.0

let calcAllMB (sizesBytes: int[]) =
    let toMB (sizeBytes) = sizeBytes |> fromBytesToKb |> fromKbToMb
    sizesBytes
    |> Array.fold(fun sizeMb sizeKb -> sizeMb + toMB sizeKb) 0.0

module StepResults =

    let calcDataTransfer (responses: StepResponse[]) =
        let allSizesBytes =
            responses
            |> Array.choose(fun x ->
                match x.Response.SizeBytes > 0 with
                | true  -> Some x.Response.SizeBytes
                | false -> None)

        { MinKb  = allSizesBytes |> calcMin |> fromBytesToKb
          MeanKb = allSizesBytes |> calcMean |> fromBytesToKb
          MaxKb  = allSizesBytes |> calcMax |> fromBytesToKb
          AllMB  = allSizesBytes |> calcAllMB }

    let mergeTraffic (counts: DataTransferCount[]) =

        let roundResult (value: float) =
            let result = Math.Round(value, 2)
            if result > 0.01 then result
            else Math.Round(value, 4)

        { MinKb  = counts |> Array.map(fun x -> x.MinKb) |> Array.minOrDefault 0.0 |> roundResult
          MeanKb = counts |> Array.map(fun x -> x.MeanKb) |> Array.averageOrDefault 0.0 |> roundResult
          MaxKb  = counts |> Array.map(fun x -> x.MaxKb) |> Array.maxOrDefault 0.0 |> roundResult
          AllMB  = counts |> Array.sumBy(fun x -> x.AllMB) |> roundResult }

    let merge (stepsResults: RawStepResults[]) =
        stepsResults
        |> Array.groupBy(fun x -> x.StepName)
        |> Array.map(fun (stName, results) ->
            let dataTransfer = results |> Array.map(fun x -> x.DataTransfer) |> mergeTraffic
            { StepName = stName
              Responses = results |> Array.collect(fun x -> x.Responses)
              DataTransfer = dataTransfer })

    let create (stepName: string, responses: StepResponse[]) =
        { StepName = stepName
          Responses = responses
          DataTransfer = calcDataTransfer(responses) }

module RawStepStats =

    let create (executionTime: TimeSpan) (stepResults: RawStepResults) =
        let okLatencies = stepResults.Responses |> Array.choose(fun x -> if x.Response.Exception.IsNone then Some x.LatencyMs else None)
        let histogram = buildHistogram(okLatencies)

        { StepName = stepResults.StepName
          OkLatencies = okLatencies
          RequestCount = stepResults.Responses.Length
          OkCount = okLatencies.Length
          FailCount = stepResults.Responses.Length - okLatencies.Length
          RPS = calcRPS(okLatencies, executionTime)
          Min = calcMin(okLatencies)
          Mean = calcMean(okLatencies)
          Max = calcMax(okLatencies)
          Percent50 = calcPercentile(histogram, 50.0)
          Percent75 = calcPercentile(histogram, 75.0)
          Percent95 = calcPercentile(histogram, 95.0)
          StdDev = calcStdDev(histogram)
          DataTransfer = stepResults.DataTransfer }

    let merge (stepsStats: RawStepStats[]) (executionTime: TimeSpan) =
        stepsStats
        |> Array.groupBy(fun x -> x.StepName)
        |> Array.map(fun (name, stats) ->
            let dataTransfer = stats |> Array.map(fun x -> x.DataTransfer) |> StepResults.mergeTraffic
            let okLatencies = stats |> Array.collect(fun x -> x.OkLatencies)
            let histogram = buildHistogram(okLatencies)
            let failCount = stats |> Array.sumBy(fun x -> x.FailCount)

            { StepName = name
              OkLatencies = okLatencies
              RequestCount = okLatencies.Length + failCount
              OkCount = okLatencies.Length
              FailCount = failCount
              RPS = calcRPS(okLatencies, executionTime)
              Min = calcMin(okLatencies)
              Mean = calcMean(okLatencies)
              Max = calcMax(okLatencies)
              Percent50 = calcPercentile(histogram, 50.0)
              Percent75 = calcPercentile(histogram, 75.0)
              Percent95 = calcPercentile(histogram, 95.0)
              StdDev = calcStdDev(histogram)
              DataTransfer = dataTransfer })

module RawScenarioStats =

    let calcLatencyCount (stepsStats: RawStepStats[]) =
        let a = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x < 800))
        let b = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x > 800 && x < 1200))
        let c = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x > 1200))
        { Less800 = a.Length
          More800Less1200 = b.Length
          More1200 = c.Length }

    let createByStepStats (scnName: ScenarioName) (executionTime: TimeSpan) (stepsStats: RawStepStats[]) =
        let mergedStepsStats = RawStepStats.merge stepsStats executionTime
        { ScenarioName = scnName
          RequestCount = mergedStepsStats |> Array.sumBy(fun x -> x.RequestCount)
          OkCount = mergedStepsStats |> Array.sumBy(fun x -> x.OkCount)
          FailCount = mergedStepsStats |> Array.sumBy(fun x -> x.FailCount)
          AllDataMB = mergedStepsStats |> Array.sumBy(fun x -> x.DataTransfer.AllMB)
          LatencyCount = mergedStepsStats |> calcLatencyCount
          RawStepsStats = RawStepStats.merge stepsStats executionTime
          Duration = executionTime }

    let create (scenario: Scenario) (executionTime: TimeSpan) (stepsResults: RawStepResults[]) =
        stepsResults
        |> StepResults.merge
        |> Array.map(RawStepStats.create executionTime)
        |> createByStepStats scenario.ScenarioName executionTime

module NodeStats =

    let mapStepStats (stepStats: RawStepStats[]) =
        stepStats
        |> Array.map(fun x ->
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
              StdDev = x.StdDev
              MinDataKb = x.DataTransfer.MinKb
              MeanDataKb = x.DataTransfer.MeanKb
              MaxDataKb = x.DataTransfer.MaxKb
              AllDataMB = x.DataTransfer.AllMB }
        )

    let mapScenarioStats (scnStats: RawScenarioStats[]) =
        scnStats
        |> Array.map(fun x ->
            { ScenarioName = x.ScenarioName
              RequestCount = x.RequestCount
              OkCount = x.OkCount
              FailCount = x.FailCount
              AllDataMB = x.AllDataMB
              StepStats = x.RawStepsStats |> mapStepStats
              LatencyCount = x.LatencyCount
              Duration = x.Duration }
        )

    let create (nodeInfo: NodeInfo) (scnStats: RawScenarioStats[]) (pluginStats: DataSet[]) =
        { RequestCount = scnStats |> Array.sumBy(fun x -> x.RequestCount)
          OkCount = scnStats |> Array.sumBy(fun x -> x.OkCount)
          FailCount = scnStats |> Array.sumBy(fun x -> x.FailCount)
          AllDataMB = scnStats |> Array.sumBy(fun x -> x.AllDataMB)
          ScenarioStats = scnStats |> mapScenarioStats
          PluginStats = pluginStats
          NodeInfo = nodeInfo }
