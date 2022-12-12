module internal NBomber.Domain.Stats.RawMeasurementStats

open System
open System.Collections.Generic
open HdrHistogram
open NBomber
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Extensions.Internal

type RawStatusCodeStats = {
    StatusCode: string
    IsError: bool
    Message: string
    mutable Count: int
}

type RawItemStats = {
    mutable MinMicroSec: int
    mutable MaxMicroSec: int
    mutable MinBytes: int
    mutable MaxBytes: int
    mutable RequestCount: int
    mutable LessOrEq800: int
    mutable More800Less1200: int
    mutable MoreOrEq1200: int
    mutable AllBytes: int64
    LatencyHistogram: LongHistogram
    DataTransferHistogram: LongHistogram
    StatusCodes: Dictionary<string,RawStatusCodeStats>
}

type RawMeasurementStats = {
    Name: string
    OkStats: RawItemStats
    FailStats: RawItemStats
}

let empty (stepName) =

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
        StatusCodes = Dictionary<string,RawStatusCodeStats>()
    }

    { Name = stepName
      OkStats = createStats()
      FailStats = createStats() }

let addMeasurement (rawStats: RawMeasurementStats) (measurement: Measurement) =

    let updateStatusCodeStats (statuses: Dictionary<string,RawStatusCodeStats>, res: IResponse) =
        match statuses.TryGetValue res.StatusCode with
        | true, codeStats ->
            codeStats.Count <- codeStats.Count + 1

        | false, _ ->
            statuses[res.StatusCode] <- { StatusCode = res.StatusCode; IsError = res.IsError; Message = res.Message; Count = 1 }

    let clientRes = measurement.ClientResponse

    // calc latency
    let latencyMs =
        if clientRes.LatencyMs > 0.0 then clientRes.LatencyMs
        else measurement.LatencyMs

    let stats =
        match clientRes.IsError with
        | true when not (String.IsNullOrEmpty clientRes.StatusCode) ->
            updateStatusCodeStats(rawStats.FailStats.StatusCodes, clientRes)
            rawStats.FailStats

        | true -> rawStats.FailStats

        | false when not (String.IsNullOrEmpty clientRes.StatusCode) ->
            updateStatusCodeStats(rawStats.OkStats.StatusCodes, clientRes)
            rawStats.OkStats

        | false -> rawStats.OkStats

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
