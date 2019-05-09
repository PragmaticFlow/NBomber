﻿namespace NBomber.Configuration

open System
open System.IO

type ReportFormat =
    | Txt = 0
    | Html = 1
    | Csv = 2
    | Md = 3

type ScenarioSetting = {
    ScenarioName: string
    ConcurrentCopies: int
    WarmUpDuration: DateTime
    Duration: DateTime
}

type GlobalSettings = {
    ScenariosSettings: ScenarioSetting list
    TargetScenarios: string list
    ReportFileName: string option
    ReportFormats: ReportFormat list option
}

type AgentSettings = {
    ClusterId: string
    Port: int
}

type AgentInfoSettings = {
    Host: string
    Port: int
    TargetScenarios: string list
}

type CoordinatorSettings = {
    ClusterId: string
    TargetScenarios: string list
    Agents: AgentInfoSettings list
}

type ClusterSettings =
    | Coordinator of CoordinatorSettings
    | Agent       of AgentSettings

type NBomberConfig = {
    GlobalSettings: GlobalSettings option
    ClusterSettings: ClusterSettings option
}

module internal NBomberConfig =
    open FSharp.Json

    let parse  json =
        Json.deserialize<NBomberConfig> json
    let load path =
        path
        |> File.ReadAllText
        |> parse
        |> Some
