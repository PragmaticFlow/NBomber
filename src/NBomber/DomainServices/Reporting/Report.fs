module internal NBomber.DomainServices.Reporting.Report

#nowarn "0104"

open System
open System.IO

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Extensions

type ReportsContent = {
    TxtReport: string list
    HtmlReport: string
    CsvReport: string
    MdReport: string
} with
    static member empty = { TxtReport = List.empty; HtmlReport = ""; CsvReport = ""; MdReport = "" }

let private buildTxtReport (nodeStats: NodeStats) =
    let pluginsStats =
        nodeStats.PluginStats
        |> Seq.collect(fun x -> x.GetTables())
        |> Seq.map(fun x -> x |> TxtReportPluginStats.print)
        |> Seq.toList

    TxtReport.print(nodeStats) :: pluginsStats

let build (nodeStats: NodeStats) =
    { TxtReport = buildTxtReport(nodeStats)
      //HtmlReport = HtmlReport.print(dep, nodeStats)
      HtmlReport = ""
      CsvReport = CsvReport.print(nodeStats)
      MdReport = MdReport.print(nodeStats) }

let save (outPutDir: string, reportFileName: string, reportFormats: ReportFormat list,
          report: ReportsContent, logger: Serilog.ILogger) =
    try
        let reportsDir = Path.Combine(outPutDir, "reports")
        Directory.CreateDirectory(reportsDir) |> ignore

        let buildReportFile (format: ReportFormat) =
            let fileExt =
                match format with
                | ReportFormat.Txt  -> ".txt"
                | ReportFormat.Html -> ".html"
                | ReportFormat.Csv  -> ".csv"
                | ReportFormat.Md   -> ".md"

            let filePath = Path.Combine(reportsDir, reportFileName) + fileExt
            { FilePath = filePath; ReportFormat = format }

        let reportFiles = reportFormats |> Seq.map(buildReportFile) |> Seq.toArray
        let txtRportContent = String.Join(Environment.NewLine, report.TxtReport)

        reportFiles
        |> Seq.map(fun x ->
            match x.ReportFormat with
            | ReportFormat.Txt  -> {| Content = txtRportContent; FilePath = x.FilePath |}
            | ReportFormat.Html -> {| Content = report.HtmlReport; FilePath = x.FilePath |}
            | ReportFormat.Csv  -> {| Content = report.CsvReport; FilePath = x.FilePath |}
            | ReportFormat.Md   -> {| Content = report.MdReport; FilePath = x.FilePath |}
        )
        |> Seq.iter(fun x -> File.WriteAllText(x.FilePath, x.Content))

        logger.Information("reports saved in folder: '{0}', {1}", DirectoryInfo(reportsDir).FullName, Environment.NewLine)
        logger.Information(txtRportContent)
        reportFiles
    with
    | ex -> logger.Error(ex, "Report.save failed")
            Array.empty
