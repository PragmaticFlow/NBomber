namespace NBomber.Contracts.Thresholds

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
    | LatencyP50 of (float -> bool)
    | LatencyP75 of (float -> bool)
    | LatencyP95 of (float -> bool)
    | LatencyP99 of (float -> bool)
