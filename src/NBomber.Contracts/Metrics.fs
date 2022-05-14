namespace NBomber.Contracts.Metrics

type RequestCountThreshold =
    | AllCount of (float -> bool)
    | OkCount of (float -> bool)
    | FailedCount of (float -> bool)
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

type Metric =
    | RequestCount of RequestCountThreshold list
    | Latency of LatencyThreshold list
    | LatencyPercentile of LatencyPercentileThreshold list
