module internal NBomber.DomainServices.Reporting.Report

#nowarn "0104"

open System
open System.IO

open Serilog

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Extensions.InternalExtensions

type ReportsContent = {
    TxtReport: string
    HtmlReport: string
    CsvReport: string
    MdReport: string
    SessionWithErrors: bool
}

let build (testInfo: TestInfo) (nodeStats: NodeStats)
          (timeLineStats: (TimeSpan * NodeStats) list) =

    let errorsExist =
        timeLineStats
        |> Seq.map(snd)
        |> Seq.tryFind(fun x -> x.FailCount > 0)
        |> Option.isSome

    { TxtReport = TxtReport.print(testInfo, nodeStats)
      HtmlReport = HtmlReport.print(testInfo, nodeStats, timeLineStats)
      CsvReport = CsvReport.print(testInfo, nodeStats)
      MdReport = MdReport.print(testInfo, nodeStats)
      SessionWithErrors = errorsExist }

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
        )
        |> Seq.iter(fun x -> File.WriteAllText(x.FilePath, x.Content))

        if report.SessionWithErrors then
            logger.Warning("test finished with errors, please check logs in './logs' folder.")

        if reportFiles.Length > 0 then
            logger.Information("reports saved in folder: '{0}', {1}",
                DirectoryInfo(reportsDir).FullName, Environment.NewLine)

        logger.Information(Environment.NewLine + report.TxtReport)
        reportFiles
    with
    | ex -> logger.Error(ex, "Report.save failed")
            Array.empty
