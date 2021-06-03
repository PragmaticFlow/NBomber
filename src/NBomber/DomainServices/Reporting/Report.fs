module internal NBomber.DomainServices.Reporting.Report

open System.Collections.Generic
open System.IO

open Serilog
open Spectre.Console.Rendering

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Extensions.InternalExtensions

type ReportsContent = {
    TxtReport: Lazy<string>
    HtmlReport: Lazy<string>
    CsvReport: Lazy<string>
    MdReport: Lazy<string>
    ConsoleReport: Lazy<IRenderable list>
    SessionFinishedWithErrors: bool
}

let build (logger: ILogger)
          (sessionResult: NodeSessionResult)
          (simulations: IDictionary<string, LoadSimulation list>) =

    logger.Verbose("Report.build")

    let errorsExist = sessionResult.NodeStats.ScenarioStats |> Array.exists(fun stats -> stats.FailCount > 0)

    { TxtReport = lazy (TxtReport.print logger sessionResult simulations)
      HtmlReport = lazy (HtmlReport.print logger sessionResult)
      CsvReport = lazy (CsvReport.print logger sessionResult)
      MdReport = lazy (MdReport.print logger sessionResult simulations)
      ConsoleReport = lazy (ConsoleReport.print logger sessionResult simulations)
      SessionFinishedWithErrors = errorsExist }

let save (folder: string, fileName: string, reportFormats: ReportFormat list,
          report: ReportsContent, logger: ILogger, testInfo: TestInfo) =
    try
        let reportsDir = Path.Combine(folder, testInfo.SessionId)
        Directory.CreateDirectory(reportsDir) |> ignore

        let buildReportFile (format: ReportFormat) =
            let fileExt =
                match format with
                | ReportFormat.Txt  -> ".txt"
                | ReportFormat.Html -> ".html"
                | ReportFormat.Csv  -> ".csv"
                | ReportFormat.Md   -> ".md"
                | _                 -> failwith "invalid report format."

            let filePath = Path.Combine(reportsDir, fileName) + fileExt
            { FilePath = filePath; ReportFormat = format }

        let reportFiles = reportFormats |> Seq.map(buildReportFile) |> Seq.toArray

        reportFiles
        |> Seq.map(fun x ->
            match x.ReportFormat with
            | ReportFormat.Txt  -> {| Content = report.TxtReport; FilePath = x.FilePath |}
            | ReportFormat.Html -> {| Content = report.HtmlReport; FilePath = x.FilePath |}
            | ReportFormat.Csv  -> {| Content = report.CsvReport; FilePath = x.FilePath |}
            | ReportFormat.Md   -> {| Content = report.MdReport; FilePath = x.FilePath |}
            | _                 -> failwith "invalid report format."
        )
        |> Seq.iter(fun x -> File.WriteAllText(x.FilePath, x.Content.Value))

        if report.SessionFinishedWithErrors then
            logger.Warning("Test finished with errors, please check the log file.")

        if reportFiles.Length > 0 then
            logger.Information("Reports saved in folder: '{0}'", DirectoryInfo(reportsDir).FullName)

        reportFiles
    with
    | ex -> logger.Error(ex, "Report.save failed")
            Array.empty
