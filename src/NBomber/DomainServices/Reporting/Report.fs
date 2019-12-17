module internal NBomber.DomainServices.Reporting.Report
open FsToolkit.ErrorHandling

#nowarn "0104"

open System
open System.IO

open NBomber.Contracts
open NBomber.Domain
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.Configuration

let emptyReport = { TxtReportPath = ""; HtmlReportPath = ""; CsvReportPath = ""; MdReportPath = "" }

let build (dep: Dependency, nodeStats: RawNodeStats[], failedAsserts: DomainError[]) =
    match dep.NodeType with
    | NodeType.SingleNode when nodeStats.Length > 0 ->
        { TxtReportPath = TxtReport.print(nodeStats.[0], failedAsserts)
          HtmlReportPath = HtmlReport.print(dep, nodeStats.[0], failedAsserts)
          CsvReportPath = CsvReport.print(nodeStats.[0])
          MdReportPath = MdReport.print(nodeStats.[0], failedAsserts) }
    
    | NodeType.Coordinator when nodeStats.Length > 0 ->
          nodeStats
          |> Array.tryFind(fun x -> x.NodeStatsInfo.Sender = NodeType.Cluster)
          |> Option.map(fun clusterStats ->
              { TxtReportPath = TxtReport.print(clusterStats, failedAsserts)
                HtmlReportPath = HtmlReport.print(dep, clusterStats, failedAsserts)
                CsvReportPath = CsvReport.print(clusterStats)
                MdReportPath = MdReport.print(clusterStats, failedAsserts) }
          )
          |> Option.defaultValue(emptyReport)
    
    | _ -> emptyReport

let save (outPutDir: string, reportFileName: string, 
          reportFormats: ReportFormat list, report: ReportResult,
          logger: Serilog.ILogger) =
    try
        let reportsDir = Path.Combine(outPutDir, "reports")
        Directory.CreateDirectory(reportsDir) |> ignore
        ResourceManager.saveAssets(reportsDir)

        reportFormats 
        |> List.map(function            
            | ReportFormat.Txt -> report.TxtReportPath, ".txt"
            | ReportFormat.Html -> report.HtmlReportPath, ".html"
            | ReportFormat.Csv -> report.CsvReportPath, ".csv"
            | ReportFormat.Md -> report.MdReportPath, ".md")

        |> List.iter(fun (content, fileExt) -> 
            let filePath = reportsDir + "/" + reportFileName + fileExt
            File.WriteAllText(filePath, content))

        logger.Information("reports saved in folder: '{0}', {1}", DirectoryInfo(reportsDir).FullName, Environment.NewLine)
        logger.Information(report.TxtReportPath)
    with
    | ex -> logger.Error(ex, "Report.save failed")