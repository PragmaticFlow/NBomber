module internal NBomber.DomainServices.Reports.Report

open System
open System.IO
open Serilog
open Spectre.Console.Rendering

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Extensions.Internal
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.Statistics
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices

type ReportsContent = {
    TxtReport: Lazy<string>
    HtmlReport: Lazy<string>
    CsvReport: Lazy<string>
    MdReport: Lazy<string>
    ConsoleReport: Lazy<IRenderable list>
}

let getLoadSimulations (scenarios: Scenario list) =
    scenarios
    |> Seq.map(fun scn -> scn.ScenarioName, scn.LoadTimeLine |> List.map(fun x -> x.LoadSimulation))
    |> dict

let build (logger: ILogger)
          (sessionResult: NodeSessionResult)
          (targetScenarios: Scenario list) =

    logger.Verbose "Report.build"

    let simulations = targetScenarios |> getLoadSimulations

    let scnStats =
        sessionResult.FinalStats.ScenarioStats
        |> Array.map(fun scn ->
            let globalInfoStep = StepStats.extractGlobalInfoStep scn
            let stepStats = scn.StepStats |> Array.append [| globalInfoStep |]
            { scn with StepStats = stepStats }
        )

    let finalStats = { sessionResult.FinalStats with ScenarioStats = scnStats }
    let newSessionResult = { sessionResult with FinalStats = finalStats }

    { TxtReport = lazy (TxtReport.print logger newSessionResult simulations)
      HtmlReport = lazy (HtmlReport.print logger newSessionResult)
      CsvReport = lazy (CsvReport.print logger newSessionResult)
      MdReport = lazy (MdReport.print logger newSessionResult simulations)
      ConsoleReport = lazy (ConsoleReport.print logger newSessionResult simulations) }

let saveToFolder (logger: ILogger, folder: string, fileName: string,
                  reportFormats: ReportFormat list, report: ReportsContent) =
    try
        Directory.CreateDirectory(folder) |> ignore

        let buildReportFile (format: ReportFormat) =
            let fileExt =
                match format with
                | ReportFormat.Txt  -> ".txt"
                | ReportFormat.Html -> ".html"
                | ReportFormat.Csv  -> ".csv"
                | ReportFormat.Md   -> ".md"
                | _                 -> failwith "invalid report format"

            let fileContent =
                match format with
                | ReportFormat.Txt  -> report.TxtReport
                | ReportFormat.Html -> report.HtmlReport
                | ReportFormat.Csv  -> report.CsvReport
                | ReportFormat.Md   -> report.MdReport
                | _                 -> failwith "invalid report format"

            let filePath = Path.Combine(folder, fileName) + fileExt
            { FilePath = filePath; ReportFormat = format; ReportContent = fileContent.Value }

        let reportFiles = reportFormats |> Seq.map(buildReportFile) |> Seq.toArray

        reportFiles
        |> Seq.iter(fun x ->
            try
                File.WriteAllText(x.FilePath, x.ReportContent)
            with
            | ex -> logger.Error(ex, "Could not save the report file {0}", x.FilePath)
        )

        if reportFiles.Length > 0 then
            logger.Information("Reports saved in folder: {0}", DirectoryInfo(folder).FullName)

        reportFiles
    with
    | ex -> logger.Error(ex, "Report.save failed")
            Array.empty

let save (dep: IGlobalDependency) (context: NBomberContext) (stats: NodeStats) (report: ReportsContent) =

    let fileName = context |> NBomberContext.getReportFileNameOrDefault DateTime.UtcNow
    let folder   = context |> NBomberContext.getReportFolderOrDefault stats.TestInfo.SessionId
    let formats  = context |> NBomberContext.getReportFormats

    report.ConsoleReport.Value |> List.iter Console.render

    if formats.Length > 0 then
        let reportFiles = saveToFolder(dep.Logger, folder, fileName, formats, report)
        { stats with ReportFiles = reportFiles }
    else
        stats
