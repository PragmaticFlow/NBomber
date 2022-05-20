namespace NBomber.Contracts.Thresholds

open Microsoft.FSharp.Reflection

type Threshold =
    | RequestAllCount of (int -> bool)
    | RequestOkCount of (int -> bool)
    | RequestFailedCount of (int -> bool)
    | RequestFailedRate of (float -> bool)
    | RPS of (float -> bool)
    | LatencyMin of (float -> bool)
    | LatencyMean of (float -> bool)
    | LatencyMax of (float -> bool)
    | LatencyStdDev of (float -> bool)
    | LatencyPercent50 of (float -> bool)
    | LatencyPercent75 of (float -> bool)
    | LatencyPercent95 of (float -> bool)
    | LatencyPercent99 of (float -> bool)
    | DataTransferMinBytes of (int -> bool)
    | DataTransferMeanBytes of (int -> bool)
    | DataTransferMaxBytes of (int -> bool)
    | DataTransferPercent50 of (int -> bool)
    | DataTransferPercent75 of (int -> bool)
    | DataTransferPercent95 of (int -> bool)
    | DataTransferPercent99 of (int -> bool)
    | DataTransferStdDev of (float -> bool)
    | DataTransferAllBytes of (int64 -> bool)
with
    member this.Name =
        match FSharpValue.GetUnionFields(this, typeof<Threshold>) with
        | case, _ -> case.Name

    override this.ToString () = this.Name
