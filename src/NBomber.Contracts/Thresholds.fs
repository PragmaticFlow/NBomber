namespace NBomber.Contracts.Thresholds

type RequestCountThreshold =
    | AllCount of (int -> bool)
    | OkCount of (int -> bool)
    | FailedCount of (int -> bool)
    | FailedRate of (float -> bool)
    | RPS of (float -> bool)

type LatencyThreshold =
    | Min of (float -> bool)
    | Mean of (float -> bool)
    | Max of (float -> bool)
    | StdDev of (float -> bool)

type LatencyPercentileThreshold =
    | P50 of (float -> bool)
    | P75 of (float -> bool)
    | P95 of (float -> bool)
    | P99 of (float -> bool)

type Threshold =
    | RequestCount of RequestCountThreshold list
    | Latency of LatencyThreshold list
    | LatencyPercentile of LatencyPercentileThreshold list
