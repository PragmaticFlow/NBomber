module internal NBomber.Domain.Statistics

open System

open HdrHistogram

open NBomber.Contracts
open NBomber.Domain

let create (nodeStats: NodeStats) =
    
    let mapStep (scnName: string, step: StepStats) =
        { ScenarioName = scnName
          StepName = step.StepName
          OkCount = step.OkCount
          FailCount = step.FailCount
          Min = step.Min
          Mean = step.Mean
          Max = step.Max
          RPS = step.RPS
          Percent50 = step.Percent50
          Percent75 = step.Percent75
          Percent95 = step.Percent95
          StdDev = step.StdDev
          DataMinKb = step.DataTransfer.MinKb
          DataMeanKb = step.DataTransfer.MeanKb
          DataMaxKb = step.DataTransfer.MaxKb
          AllDataMB = step.DataTransfer.AllMB
          Meta = nodeStats.Meta }        

    nodeStats.AllScenariosStats
    |> Array.collect(fun scn -> 
        scn.StepsStats |> Array.map(fun step -> mapStep(scn.ScenarioName, step)))    

let buildHistogram (latencies) =    
    let histogram = LongHistogram(TimeStamp.Hours(24), 3)
    latencies |> Array.filter(fun x -> x > 0)
              |> Array.iter(fun x -> x |> int64 |> histogram.RecordValue)
    histogram    

let calcRPS (latencies: Latency[], scnDuration: TimeSpan) =
    let totalSec = if scnDuration.TotalSeconds < 1.0 then 1.0
                   else scnDuration.TotalSeconds
    latencies.Length / int(totalSec)

let calcMin (latencies: Latency[]) =
    if Array.isEmpty latencies then 0 else Array.min(latencies)

let calcMean (latencies: Latency[]) =        
    if Array.isEmpty latencies 
    then 0
    else latencies |> Array.map(float) |> Array.average |> int

let calcMax (latencies: Latency[]) =
    if Array.isEmpty latencies then 0 else Array.max(latencies)

let calcPercentile (histogram: LongHistogram, percentile: float) =
    if histogram.TotalCount > 0L then int(histogram.GetValueAtPercentile(percentile)) else 0

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
    |> Array.fold(fun sizeMb sizeKb -> sizeMb + toMB(sizeKb)) 0.0

module StepResults =

    let calcDataTransfer (responses: (Response*Latency)[]) = 
        let allSizesBytes = responses
                            |> Array.map(fst)
                            |> Array.filter(fun x -> x.SizeBytes > 0)
                            |> Array.map(fun x -> x.SizeBytes)

        { MinKb  = allSizesBytes |> calcMin |> fromBytesToKb
          MeanKb = allSizesBytes |> calcMean |> fromBytesToKb
          MaxKb  = allSizesBytes |> calcMax |> fromBytesToKb
          AllMB  = calcAllMB(allSizesBytes) }

    let mergeTraffic (counts: DataTransferCount[]) =
        
        let roundResult (value: float) =
            let result = Math.Round(value, 2)
            if result > 0.01 then result            
            else Math.Round(value, 4)

        { MinKb  = counts |> Array.map(fun x -> x.MinKb) |> Array.min |> roundResult
          MeanKb = counts |> Array.map(fun x -> x.MeanKb) |> Array.average |> roundResult
          MaxKb  = counts |> Array.map(fun x -> x.MaxKb) |> Array.max |> roundResult
          AllMB  = counts |> Array.sumBy(fun x -> x.AllMB) |> roundResult }  
          
    let merge (stepsResults: StepResults[]) =
        stepsResults
        |> Array.groupBy(fun x -> x.StepName)
        |> Array.map(fun (stName, results) ->            
            let dataTransfer = results |> Array.map(fun x -> x.DataTransfer) |> mergeTraffic
            { StepName = stName
              Results = results |> Array.collect(fun x -> x.Results)
              DataTransfer = dataTransfer })

    let create (stepName: string) (results: (Response*Latency)[]) =
        { StepName = stepName 
          Results = results
          DataTransfer = calcDataTransfer(results) }

module StepStats = 

    let create (scnDuration: TimeSpan) (stepResults: StepResults) =        
        let okLatencies = stepResults.Results |> Array.filter(fun (res,_) -> res.IsOk) |> Array.map(snd)       
        let histogram = buildHistogram(okLatencies)
        
        { StepName = stepResults.StepName
          OkLatencies = okLatencies
          ReqeustCount = stepResults.Results.Length          
          OkCount = okLatencies.Length
          FailCount = stepResults.Results.Length - okLatencies.Length
          RPS = calcRPS(okLatencies, scnDuration)
          Min = calcMin(okLatencies)
          Mean = calcMean(okLatencies)
          Max = calcMax(okLatencies)
          Percent50 = calcPercentile(histogram, 50.0)
          Percent75 = calcPercentile(histogram, 75.0)
          Percent95 = calcPercentile(histogram, 95.0)
          StdDev = calcStdDev(histogram)
          DataTransfer = stepResults.DataTransfer } 
          
    let merge (stepsStats: StepStats[]) (scnDuration: TimeSpan) = 
        stepsStats
        |> Array.groupBy(fun x -> x.StepName)
        |> Array.map(fun (name, stats) -> 
            let dataTransfer = stats |> Array.map(fun x -> x.DataTransfer) |> StepResults.mergeTraffic
            let okLatencies = stats |> Array.collect(fun x -> x.OkLatencies)
            let histogram = buildHistogram(okLatencies)
            let failCount = stats |> Array.sumBy(fun x -> x.FailCount)

            { StepName = name
              OkLatencies = okLatencies
              ReqeustCount = okLatencies.Length + failCount
              OkCount = okLatencies.Length
              FailCount = failCount
              RPS = calcRPS(okLatencies, scnDuration)
              Min = calcMin(okLatencies)
              Mean = calcMean(okLatencies)
              Max = calcMax(okLatencies)
              Percent50 = calcPercentile(histogram, 50.0)
              Percent75 = calcPercentile(histogram, 75.0)
              Percent95 = calcPercentile(histogram, 95.0)
              StdDev = calcStdDev(histogram)
              DataTransfer = dataTransfer }) 

module ScenarioStats =

    let calcLatencyCount (stepsStats: StepStats[]) = 
        let a = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x < 800))
        let b = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x > 800 && x < 1200))
        let c = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x > 1200))        
        { Less800 = a.Length
          More800Less1200 = b.Length
          More1200 = c.Length } 

    let createByStepStats (scenario: Scenario) (stepsStats: StepStats[]) =        
        let mergedStepsStats = StepStats.merge stepsStats scenario.Duration

        let latencyCount = calcLatencyCount mergedStepsStats

        let allOkCount = mergedStepsStats |> Array.sumBy(fun x -> x.OkCount)
        let allFailCount = mergedStepsStats |> Array.sumBy(fun x -> x.FailCount)
        let allRPS = mergedStepsStats |> Array.map(fun x -> x.RPS) |> Array.min

        { ScenarioName = scenario.ScenarioName
          StepsStats = mergedStepsStats
          RPS = allRPS
          ConcurrentCopies = scenario.ConcurrentCopies
          OkCount = allOkCount
          FailCount = allFailCount
          LatencyCount = latencyCount          
          Duration = scenario.Duration }         

    let create (scenario: Scenario) (stepsResults: StepResults[]) =
        stepsResults
        |> StepResults.merge
        |> Array.map(StepStats.create scenario.Duration)
        |> createByStepStats scenario        

module NodeStats =

    let create (meta: StatisticsMeta) (allScnStats: ScenarioStats[]) =
        
        let allOkCount = allScnStats
                         |> Array.collect(fun x -> x.StepsStats)
                         |> Array.sumBy(fun x -> x.OkCount)

        let allFailCount = allScnStats
                           |> Array.collect(fun x -> x.StepsStats)
                           |> Array.sumBy(fun x -> x.FailCount)

        let latencyCount = allScnStats
                           |> Array.collect(fun x -> x.StepsStats)
                           |> ScenarioStats.calcLatencyCount

        { AllScenariosStats = allScnStats
          OkCount = allOkCount
          FailCount = allFailCount
          LatencyCount = latencyCount
          Meta = meta }

    let merge (meta: StatisticsMeta, allNodesStats: NodeStats[]) (allScenarios: Scenario[]) =
        allNodesStats 
        |> Array.collect(fun x -> x.AllScenariosStats)
        |> Array.groupBy(fun x -> x.ScenarioName)
        |> Array.map(fun (scnName, allStats) -> 
            let scn = allScenarios |> Array.find(fun x -> x.ScenarioName = scnName)
            allStats |> Array.collect(fun x -> x.StepsStats) |> ScenarioStats.createByStepStats scn)             
        |> create meta