module internal NBomber.DomainServices.Reporting.Report
open FsToolkit.ErrorHandling

#nowarn "0104"

open System
open System.IO

open Serilog

open NBomber.Contracts
open NBomber.Domain
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.Configuration

type ReportResult = {
    TxtReport: string
    HtmlReport: string
    CsvReport: string
    MdReport: string
} with
  static member empty =
      { TxtReport = ""
        HtmlReport = ""
        CsvReport = ""
        MdReport = "" }

let build (dep: Dependency, nodeStats: RawNodeStats[], failedAsserts: DomainError[]) =
    match dep.NodeType with
    | NodeType.SingleNode when nodeStats.Length > 0 ->
        { TxtReport = TxtReport.print(nodeStats.[0], failedAsserts)
          HtmlReport = HtmlReport.print(dep, nodeStats.[0], failedAsserts)
          CsvReport = CsvReport.print(nodeStats.[0])
          MdReport = MdReport.print(nodeStats.[0], failedAsserts) }
    
    | NodeType.Coordinator when nodeStats.Length > 0 ->
          nodeStats
          |> Array.tryFind(fun x -> x.Meta.Sender = NodeType.Cluster)
          |> Option.map(fun clusterStats ->
              { TxtReport = TxtReport.print(clusterStats, failedAsserts)
                HtmlReport = HtmlReport.print(dep, clusterStats, failedAsserts)
                CsvReport = CsvReport.print(clusterStats)
                MdReport = MdReport.print(clusterStats, failedAsserts) }
          )
          |> Option.defaultValue(ReportResult.empty)
    
    | _ -> ReportResult.empty        

let save (outPutDir: string, reportFileName: string, 
          reportFormats: ReportFormat list, report: ReportResult) =
    try
        let reportsDir = Path.Combine(outPutDir, "reports")
        Directory.CreateDirectory(reportsDir) |> ignore
        ResourceManager.saveAssets(reportsDir)

        reportFormats 
        |> List.map(function            
            | ReportFormat.Txt -> report.TxtReport, ".txt"
            | ReportFormat.Html -> report.HtmlReport, ".html"
            | ReportFormat.Csv -> report.CsvReport, ".csv"
            | ReportFormat.Md -> report.MdReport, ".md")

        |> List.iter(fun (content, fileExt) -> 
            let filePath = reportsDir + "/" + reportFileName + fileExt
            File.WriteAllText(filePath, content))

        Log.Information("reports saved in folder: '{0}', {1}", DirectoryInfo(reportsDir).FullName, Environment.NewLine)
        Log.Information(report.TxtReport)
    with
    | ex -> Log.Error(ex, "Report.save failed")