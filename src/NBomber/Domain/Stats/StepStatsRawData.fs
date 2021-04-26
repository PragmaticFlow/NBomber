module internal NBomber.Domain.Stats.StepStatsRawData

open System
open System.Collections.Generic

open HdrHistogram

open NBomber
open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.Statistics

let createEmpty () =

    let createStats () = {
        MinMicroSec = Int32.MaxValue
        MaxMicroSec = 0
        MinBytes = Int32.MaxValue
        MaxBytes = 0
        RequestCount = 0
        LessOrEq800 = 0
        More800Less1200 = 0
        MoreOrEq1200 = 0
        AllBytes = 0L
        LatencyHistogram = LongHistogram(highestTrackableValue = Constants.MaxTrackableStepLatency, numberOfSignificantValueDigits = 3)
        DataTransferHistogram = LongHistogram(highestTrackableValue = Constants.MaxTrackableStepResponseSize, numberOfSignificantValueDigits = 3)
        StatusCodes = Dictionary<int,StatusCodeStats>()
    }

    { OkStats = createStats()
      FailStats = createStats() }

let addResponse (stData: StepStatsRawData) (response: StepResponse) =

    let updateStatusCodeStats (statuses: Dictionary<int,StatusCodeStats>, res: Response) =
        let statusCode = res.StatusCode.Value
        match statuses.TryGetValue statusCode with
        | true, codeStats -> codeStats.Count <- codeStats.Count + 1
        | false, _ ->
            statuses.[statusCode] <- { StatusCode = statusCode
                                       IsError = res.IsError
                                       Message = res.ErrorMessage
                                       Count = 1 }

    let clientRes = response.ClientResponse

    // calc latency
    let latencyMs =
        if clientRes.LatencyMs > 0.0 then clientRes.LatencyMs
        else response.LatencyMs

    let stats =
        match clientRes.IsError with
        | true when clientRes.StatusCode.HasValue ->
            updateStatusCodeStats(stData.FailStats.StatusCodes, clientRes)
            stData.FailStats

        | true -> stData.FailStats

        | false when clientRes.StatusCode.HasValue ->
            updateStatusCodeStats(stData.OkStats.StatusCodes, clientRes)
            stData.OkStats

        | false -> stData.OkStats

    stats.RequestCount <- stats.RequestCount + 1

    // checks that the response is real (it was created after the request was sent)
    if latencyMs > 0.0 then

        // add latency
        let latencyMicroSec = Converter.fromMsToMicroSec(latencyMs)
        stats.LatencyHistogram.RecordValue(int64 latencyMicroSec)

        if latencyMicroSec < stats.MinMicroSec then
            stats.MinMicroSec <- latencyMicroSec

        if latencyMicroSec > stats.MaxMicroSec then
            stats.MaxMicroSec <- latencyMicroSec

        if latencyMs <= 800.0 then stats.LessOrEq800 <- stats.LessOrEq800 + 1
        elif latencyMs > 800.0 && latencyMs < 1200.0 then stats.More800Less1200 <- stats.More800Less1200 + 1
        elif latencyMs >= 1200.0 then stats.MoreOrEq1200 <- stats.MoreOrEq1200 + 1

        // add data transfer
        if clientRes.SizeBytes > 0 then
            stats.AllBytes <- stats.AllBytes + int64 clientRes.SizeBytes
            stats.DataTransferHistogram.RecordValue(int64 clientRes.SizeBytes)

            if clientRes.SizeBytes < stats.MinBytes then
                stats.MinBytes <- clientRes.SizeBytes

            if clientRes.SizeBytes > stats.MaxBytes then
                stats.MaxBytes <- clientRes.SizeBytes

    stData
