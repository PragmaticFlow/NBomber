﻿module internal NBomber.Domain.Errors

open System
open NBomber.Contracts
open NBomber.Domain.DomainTypes

type StepError = {
    ScenarioName: string
    StepName: string
    Error: string
}

type DomainError =
    | InitScenarioError  of ex:exn
    | CleanScenarioError of ex:exn
    | AssertNotFound of assertNumber:int * assertion:Assertion
    | AssertionError of assertNumber:int * assertion:Assertion * stats:Statistics

    // Validation errors
    | DurationLessThanOneSecond of scenarioNames:string[]
    | ConcurrentCopiesLessThanOne of scenarioNames:string[]
    | EmptyReportFileName
    | UnsupportedReportFormat of reportFormats:string[]
    | DuplicateScenarios
    | DuplicateSteps of scenarioNames:string[]
    | EmptyScenarioName
    | EmptyStepName of scenarioNames:string[]
    | ScenariosNotFound of notFoundScenarios:string[] * availableScenarios:string[]

    // Cluster Coordinator errors
    | StartNewSessionError of agentErrors:DomainError[]
    | StartWarmUpError     of agentErrors:DomainError[]
    | StartBombingError    of agentErrors:DomainError[]
    | GetStatisticsError   of agentErrors:DomainError[]

    // Cluster Agent errors
    | AgentErrors        of errors:DomainError[]
    | AgentIsWorking
    | CommunicationError of ex:exn

let rec toString error =
    match error with
    | InitScenarioError ex -> String.Format("init scenario error:'{0}'", ex.ToString())
    | CleanScenarioError ex -> String.Format("clean scenario error:'{0}'", ex.ToString())

    | AssertNotFound (assertNum,assertion) ->
        match assertion with
        | Step s -> sprintf "Assertion #'%i' is not found for step: '%s' in scenario: '%s'" assertNum s.StepName s.ScenarioName

    | AssertionError (assertNum,assertion,stats) ->
        match assertion with
        | Step s -> [ sprintf "Assertion #'%i' FAILED for:" assertNum
                      sprintf "SCENARIO: '%s'" s.ScenarioName
                      sprintf "STEP: '%s'" s.StepName
                      s.Label |> Option.map (fun x -> sprintf "LABEL: '%s'" x)
                              |> Option.defaultValue ""
                      "STATS:"
                      sprintf "%A" stats
                    ] |> List.filter ((<>) "")
                      |> String.concat Environment.NewLine

    | ScenariosNotFound (notFoundScenarios,availableScenarios) ->
        notFoundScenarios
        |> String.concatWithCommaAndQuotes
        |> sprintf "Target scenarios %s is not found. Available scenarios are %s" <| String.concatWithCommaAndQuotes(availableScenarios)

    | DurationLessThanOneSecond scenarioNames ->
        scenarioNames
        |> String.concatWithCommaAndQuotes
        |> sprintf "Duration for scenarios %s can not be less than 1 sec."

    | ConcurrentCopiesLessThanOne scenarioNames ->
        scenarioNames
        |> String.concatWithCommaAndQuotes
        |> sprintf "Concurrent copies for scenarios %s can not be less than 1."

    | EmptyScenarioName -> "Scenario name can not be empty."
    | EmptyReportFileName -> "Report File Name can not be empty string."
    | UnsupportedReportFormat reportFormats ->
        reportFormats
        |> String.concatWithCommaAndQuotes
        |> sprintf "Unknown Report Formats %s. Allowed formats: Txt, Html or Csv."

    | DuplicateScenarios -> "Scenario names should be unique."
    | DuplicateSteps scenarioNames ->
        scenarioNames
        |> String.concatWithCommaAndQuotes
        |> sprintf "Step names are not unique in scenarios: %s. Step names should be unique within scenario."

    | EmptyStepName scenarioNames->
        scenarioNames
        |> String.concatWithCommaAndQuotes
        |> sprintf "Step names are empty in scenarios: %s. Step names should not be empty within scenario."

    | _ -> "undefined error"

let getErrorsString (results: Result<_,DomainError>[]) =
    results
    |> Array.filter Result.isError
    |> Array.map(Result.getError >> toString)
    |> String.concat(Environment.NewLine)
