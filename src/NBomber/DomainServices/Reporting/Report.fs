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

type ReportsContent = {
    TxtReport: string
    HtmlReport: string
    CsvReport: string
    MdReport: string
}
 with
 static member empty = { TxtReport = ""; HtmlReport = ""; CsvReport = ""; MdReport = "" }

let build (dep: Dependency, nodeStats: RawNodeStats[], failedAsserts: DomainError[]) =
    match dep.NodeType with
    | NodeType.SingleNode when nodeStats.Length > 0 ->
        { TxtReport = TxtReport.print(nodeStats.[0], failedAsserts)
          HtmlReport = HtmlReport.print(dep, nodeStats.[0], failedAsserts)
          CsvReport = CsvReport.print(nodeStats.[0])
          MdReport = MdReport.print(nodeStats.[0], failedAsserts) }
    
    | NodeType.Coordinator when nodeStats.Length > 0 ->
          nodeStats
          |> Array.tryFind(fun x -> x.NodeStatsInfo.Sender = NodeType.Cluster)
          |> Option.map(fun clusterStats ->
              { TxtReport = TxtReport.print(clusterStats, failedAsserts)
                HtmlReport = HtmlReport.print(dep, clusterStats, failedAsserts)
                CsvReport = CsvReport.print(clusterStats)
                MdReport = MdReport.print(clusterStats, failedAsserts) }
          )
          |> Option.defaultValue(ReportsContent.empty)
    
    | _ -> ReportsContent.empty

let save (outPutDir: string, reportFileName: string, 
          reportFormats: ReportFormat list, report: ReportsContent,
          logger: Serilog.ILogger) =
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
    
        reportFiles
        |> Array.map(fun x ->
            match x.ReportFormat with
            | ReportFormat.Txt  -> {| Content = report.TxtReport; FilePath = x.FilePath |}
            | ReportFormat.Html -> {| Content = report.HtmlReport; FilePath = x.FilePath |}
            | ReportFormat.Csv  -> {| Content = report.CsvReport; FilePath = x.FilePath |}
            | ReportFormat.Md   -> {| Content = report.MdReport; FilePath = x.FilePath |}
        )
        |> Array.iter(fun x -> File.WriteAllText(x.FilePath, x.Content))

        logger.Information("reports saved in folder: '{0}', {1}", DirectoryInfo(reportsDir).FullName, Environment.NewLine)
        logger.Information(report.TxtReport)
        reportFiles
    with
    | ex -> logger.Error(ex, "Report.save failed")
            Array.empty