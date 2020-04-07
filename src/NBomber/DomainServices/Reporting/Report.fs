module internal NBomber.DomainServices.Reporting.Report

#nowarn "0104"

open System
open System.IO

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain.StatisticsTypes
open NBomber.Extensions
open NBomber.Infra
open NBomber.Infra.Dependency

type ReportsContent = {
    TxtReport: string list
    HtmlReport: string
    CsvReport: string
    MdReport: string
}
 with
 static member empty = { TxtReport = List.empty; HtmlReport = ""; CsvReport = ""; MdReport = "" }

let private buildTxtReport (nodeStats: RawNodeStats[], extensionStats: ExtensionStatistics[]) =
    let extensionStatsData =
        extensionStats
        |> Array.collect(fun x -> x.GetTables())
        |> Array.map(fun x -> x |> TxtReportCustom.print)
        |> List.ofSeq

    TxtReport.print(nodeStats.[0]) :: extensionStatsData

let build (dep: GlobalDependency, nodeStats: RawNodeStats[], extensionStats: ExtensionStatistics[]) =
    match dep.NodeType with
    | NodeType.SingleNode when nodeStats.Length > 0 ->
        { TxtReport = buildTxtReport(nodeStats, extensionStats)
          HtmlReport = HtmlReport.print(dep, nodeStats.[0])
          CsvReport = CsvReport.print(nodeStats.[0])
          MdReport = MdReport.print(nodeStats.[0]) }

    | _ -> ReportsContent.empty

let save (outPutDir: string, reportFileName: string, reportFormats: ReportFormat list,
          report: ReportsContent, logger: Serilog.ILogger) =
    try
        let reportsDir = Path.Combine(outPutDir, "reports")
        Directory.CreateDirectory(reportsDir) |> ignore
        ResourceManager.saveAssets(reportsDir)

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
        |> Array.map(fun x ->
            match x.ReportFormat with
            | ReportFormat.Txt  -> {| Content = txtRportContent; FilePath = x.FilePath |}
            | ReportFormat.Html -> {| Content = report.HtmlReport; FilePath = x.FilePath |}
            | ReportFormat.Csv  -> {| Content = report.CsvReport; FilePath = x.FilePath |}
            | ReportFormat.Md   -> {| Content = report.MdReport; FilePath = x.FilePath |}
        )
        |> Array.iter(fun x -> File.WriteAllText(x.FilePath, x.Content))

        logger.Information("reports saved in folder: '{0}', {1}", DirectoryInfo(reportsDir).FullName, Environment.NewLine)
        logger.Information(txtRportContent)
        reportFiles
    with
    | ex -> logger.Error(ex, "Report.save failed")
            Array.empty
