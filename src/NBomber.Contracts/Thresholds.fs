namespace NBomber.Contracts.Thresholds

module private ThresholdDefaultDescriptions =

    let [<Literal>] RequestAllCountDefaultDescription = "request count - all"
    let [<Literal>] RequestOkCountDefaultDescription = "request count - ok"
    let [<Literal>] RequestFailedCountDefaultDescription = "request count - failed"
    let [<Literal>] RequestFailedRateDefaultDescription = "request rate - failed"
    let [<Literal>] RPSDefaultDescription = "request count - RPS"
    let [<Literal>] LatencyMinDefaultDescription = "latency - min"
    let [<Literal>] LatencyMeanDefaultDescription = "latency - mean"
    let [<Literal>] LatencyMaxDefaultDescription = "latency - max"
    let [<Literal>] LatencyStdDevDefaultDescription = "latency - StdDev"
    let [<Literal>] LatencyPercent50DefaultDescription = "latency percentile - 50%"
    let [<Literal>] LatencyPercent75DefaultDescription = "latency percentile - 75%"
    let [<Literal>] LatencyPercent95DefaultDescription = "latency percentile - 95%"
    let [<Literal>] LatencyPercent99DefaultDescription = "latency percentile - 99%"
    let [<Literal>] DataTransferMinBytesDefaultDescription = "data transfer bytes - min"
    let [<Literal>] DataTransferMeanBytesDefaultDescription = "data transfer bytes - mean"
    let [<Literal>] DataTransferMaxBytesDefaultDescription = "data transfer bytes - max"
    let [<Literal>] DataTransferAllBytesDefaultDescription = "data transfer bytes - all"
    let [<Literal>] DataTransferStdDevDefaultDescription = "data transfer - StdDev"
    let [<Literal>] DataTransferPercent50DefaultDescription = "data transfer percentile - 50%"
    let [<Literal>] DataTransferPercent75DefaultDescription = "data transfer percentile - 75%"
    let [<Literal>] DataTransferPercent95DefaultDescription = "data transfer percentile - 95%"
    let [<Literal>] DataTransferPercent99DefaultDescription = "data transfer percentile - 99%"

open ThresholdDefaultDescriptions

type ThresholdBody<'a> = ('a -> bool) * string option

type Threshold =
    | RequestAllCount of ThresholdBody<int>
    | RequestOkCount of ThresholdBody<int>
    | RequestFailedCount of ThresholdBody<int>
    | RequestFailedRate of ThresholdBody<float>
    | RPS of ThresholdBody<float>
    | LatencyMin of ThresholdBody<float>
    | LatencyMean of ThresholdBody<float>
    | LatencyMax of ThresholdBody<float>
    | LatencyStdDev of ThresholdBody<float>
    | LatencyPercent50 of ThresholdBody<float>
    | LatencyPercent75 of ThresholdBody<float>
    | LatencyPercent95 of ThresholdBody<float>
    | LatencyPercent99 of ThresholdBody<float>
    | DataTransferMinBytes of ThresholdBody<int>
    | DataTransferMeanBytes of ThresholdBody<int>
    | DataTransferMaxBytes of ThresholdBody<int>
    | DataTransferPercent50 of ThresholdBody<int>
    | DataTransferPercent75 of ThresholdBody<int>
    | DataTransferPercent95 of ThresholdBody<int>
    | DataTransferPercent99 of ThresholdBody<int>
    | DataTransferStdDev of ThresholdBody<float>
    | DataTransferAllBytes of ThresholdBody<int64>
with
    member this.Description =
        match this with
        | RequestAllCount (_, desc) -> desc, RequestAllCountDefaultDescription
        | RequestOkCount (_, desc) -> desc, RequestOkCountDefaultDescription
        | RequestFailedCount (_, desc) -> desc, RequestFailedCountDefaultDescription
        | RequestFailedRate (_, desc) -> desc, RequestFailedRateDefaultDescription
        | RPS (_, desc) -> desc, RPSDefaultDescription
        | LatencyMin (_, desc) -> desc, LatencyMinDefaultDescription
        | LatencyMean (_, desc) -> desc, LatencyMeanDefaultDescription
        | LatencyMax (_, desc) -> desc, LatencyMaxDefaultDescription
        | LatencyStdDev (_, desc) -> desc, LatencyStdDevDefaultDescription
        | LatencyPercent50 (_, desc) -> desc, LatencyPercent50DefaultDescription
        | LatencyPercent75 (_, desc) -> desc, LatencyPercent75DefaultDescription
        | LatencyPercent95 (_, desc) -> desc, LatencyPercent95DefaultDescription
        | LatencyPercent99 (_, desc) -> desc, LatencyPercent99DefaultDescription
        | DataTransferMinBytes (_, desc) -> desc, DataTransferMinBytesDefaultDescription
        | DataTransferMeanBytes (_, desc) -> desc, DataTransferMeanBytesDefaultDescription
        | DataTransferMaxBytes (_, desc) -> desc, DataTransferMaxBytesDefaultDescription
        | DataTransferPercent50 (_, desc) -> desc, DataTransferPercent50DefaultDescription
        | DataTransferPercent75 (_, desc) -> desc, DataTransferPercent75DefaultDescription
        | DataTransferPercent95 (_, desc) -> desc, DataTransferPercent95DefaultDescription
        | DataTransferPercent99 (_, desc) -> desc, DataTransferPercent99DefaultDescription
        | DataTransferStdDev (_, desc) -> desc, DataTransferStdDevDefaultDescription
        | DataTransferAllBytes (_, desc) -> desc, DataTransferAllBytesDefaultDescription
        |> fun x ->
            match x with
            | desc, defaultDesc -> desc |> Option.defaultValue defaultDesc

    override this.ToString () = this.Description
