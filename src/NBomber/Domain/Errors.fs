module internal NBomber.Errors

open System
open System.IO
open NBomber.Contracts
open NBomber.Extensions.Internal

exception RestartScenarioIteration

type ScenarioError =
    | EmptyRegisterScenarios
    | EmptyScenarioName
    | DuplicateScenarioName of scenarioNames:string list
    | DuplicateScenarioNamesInConfig of scenarioNames:string list
    | EmptyScenarioWithEmptyInitAndClean of scenarioName:string
    | TargetScenariosNotFound of notFoundScenarios:string list * regScenarios:string list
    | InitScenarioError  of ex:exn
    | CleanScenarioError of ex:exn
    | WarmUpDurationIsBiggerScnDuration of scnName:string * warmUpDuration:TimeSpan * scnDuration:TimeSpan

type ReportError =
    | EmptyReportName
    | InvalidReportName
    | EmptyReportFolderPath
    | InvalidReportFolderPath
    | ReportingIntervalSmallerThanMin

type LoadSimulationError =
    | EmptySimulationsList         of scnName:string
    | DurationIsSmallerThanMin     of scnName:string * simulation:LoadSimulation
    | IntervalIsBiggerThanDuration of scnName:string * simulation:LoadSimulation
    | CopiesCountIsNegative        of scnName:string * simulation:LoadSimulation
    | RateIsNegative               of scnName:string * simulation:LoadSimulation
    | MinRateIsBiggerThanMax       of scnName:string * simulation:LoadSimulation

type AppError =
    | Scenario of ScenarioError
    | Report   of ReportError
    | LoadSimulation of LoadSimulationError

    static member create (e: ScenarioError) = Scenario e
    static member create (e: ReportError) = Report e
    static member create (e: LoadSimulationError) = LoadSimulation e

    static member createResult (e: ScenarioError) = Error(Scenario e)
    static member createResult (e: ReportError) = Error(Report e)
    static member createResult (e: LoadSimulationError) = Error(LoadSimulation e)

    static member createResult (e: AppError) =
        match e with
        | Scenario e -> AppError.createResult e
        | Report e -> AppError.createResult e
        | LoadSimulation e -> AppError.createResult e

    static member toString (error: ScenarioError) =
        match error with
        | EmptyRegisterScenarios ->
            "No scenarios were registered. Please use NBomberRunner.RegisterScenarios(scenarios) to register scenarios"

        | EmptyScenarioName -> "Scenario name cannot be empty"

        | DuplicateScenarioName scenarioNames ->
            $"Scenario names are not unique: '{String.concatWithComma scenarioNames}'"

        | DuplicateScenarioNamesInConfig scenarioNames ->
            $"Scenario names are not unique in JSON config: '{String.concatWithComma scenarioNames}'"

        | EmptyScenarioWithEmptyInitAndClean name ->
            $"Empty scenario: '{name}' has no Init and Clean functions defined. The empty scenario should have at least Init or Clean functions defined."

        | TargetScenariosNotFound (notFoundScenarios, regScenarios) ->
            $"Target scenarios: '{String.concatWithComma regScenarios}' are not found. Available scenarios are: '{String.concatWithComma notFoundScenarios}'"

        | InitScenarioError ex  -> $"Init scenario error: '{ex.ToString()}'"
        | CleanScenarioError ex -> $"Clean scenario error: '{ex.ToString()}'"

        | WarmUpDurationIsBiggerScnDuration (scnName, warmUpDuration, scnDuration) ->
            $"Scenario '{scnName}' has a warm-up duration '{warmUpDuration}' that is bigger than the actual scenario's duration '{scnDuration}'. It should be equal or smaller but not bigger."

    static member toString (error: ReportError) =
        match error with
        | EmptyReportName -> "Report file name cannot be empty string"

        | InvalidReportName -> $"Report file name contains invalid chars: '%A{Path.GetInvalidFileNameChars()}'"

        | EmptyReportFolderPath -> "Report folder path cannot be empty string"
        | InvalidReportFolderPath -> $"Report folder path contains invalid chars: '%A{Path.GetInvalidFileNameChars()}'"

        | ReportingIntervalSmallerThanMin ->
            $"ReportingInterval should be bigger than min value: '{int Constants.MinReportingInterval.TotalSeconds}'"

    static member toString (error: LoadSimulationError) =
        match error with
        | EmptySimulationsList scnName ->
            $"Scenario: '{scnName}', LoadSimulations list is empty"

        | DurationIsSmallerThanMin (scnName, simulation) ->
            let min = Constants.MinSimulationDuration.ToString("hh\:mm\:ss")
            $"Scenario: '{scnName}', LoadSimulation duration: '%A{simulation}' is smaller than min duration value: '{min}'"

        | IntervalIsBiggerThanDuration (scnName, simulation) ->
            $"Scenario: '{scnName}', LoadSimulation interval: '%A{simulation}' should be smaller than simulation duration"

        | CopiesCountIsNegative (scnName, simulation) ->
            $"Scenario: '{scnName}', LoadSimulation: '%A{simulation}' has invalid copiesCount value. The value should be bigger or equal 0"

        | RateIsNegative (scnName, simulation) ->
            $"Scenario: '{scnName}', LoadSimulation: '%A{simulation}' has invalid rate value. The value should be bigger or equal 0"

        | MinRateIsBiggerThanMax (scnName, simulation) ->
            $"Scenario: '{scnName}', LoadSimulation: '%A{simulation}' has invalid value of minRate. The value should be bigger than value of maxRate"

    static member toString (error: AppError) =
        match error with
        | Scenario e -> AppError.toString e
        | Report e   -> AppError.toString e
        | LoadSimulation e -> AppError.toString e
