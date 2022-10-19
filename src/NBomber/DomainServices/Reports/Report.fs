module internal NBomber.DomainServices.Reports.Report

open System
open System.IO
open System.Threading.Tasks

open NBomber.Contracts
open NBomber.Infra.Dependency
open Serilog
open Spectre.Console.Rendering

open NBomber.Contracts.Stats
open NBomber.Extensions.Internal
open NBomber.Domain.DomainTypes
open NBomber.Infra
open NBomber.DomainServices

type ReportsContent = {
    TxtReport: Lazy<string>
    HtmlReport: Lazy<string>
    CsvReport: Lazy<string>
    MdReport: Lazy<string>
    ConsoleReport: Lazy<IRenderable list>
    SessionFinishedWithErrors: bool
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
    let errorsExist = sessionResult.FinalStats.ScenarioStats |> Array.exists(fun stats -> stats.Fail.Request.Count > 0)

    { TxtReport = lazy (TxtReport.print logger sessionResult simulations)
      HtmlReport = lazy (HtmlReport.print logger sessionResult)
      CsvReport = lazy (CsvReport.print logger sessionResult)
      MdReport = lazy (MdReport.print logger sessionResult simulations)
      ConsoleReport = lazy (ConsoleReport.print logger sessionResult simulations)
      SessionFinishedWithErrors = errorsExist }

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

        if report.SessionFinishedWithErrors then
            logger.Warning("Test finished with errors, please check the log file")

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
