module internal NBomber.DomainServices.Reporting.Report

open System
open System.IO

open Serilog

open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes
open NBomber.Domain.Errors
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.Contracts

type ReportResult = {
    TxtReport: string
    HtmlReport: string 
    CsvReport: string   
}

let build (dep: Dependency, stats: GlobalStats, failedAsserts: DomainError[]) =
    { TxtReport = TxtReport.print(stats)
      HtmlReport = HtmlReport.print(dep, stats, failedAsserts)
      CsvReport = CsvReport.print(stats) }

let save (dep: Dependency, outPutDir: string, reportFileName: string, reportFormats: ReportFormat[]) (report: ReportResult) =
    try
        let reportsDir = Path.Combine(outPutDir, "reports")
        Directory.CreateDirectory(reportsDir) |> ignore
        ResourceManager.saveAssets(reportsDir)
            
        let filePath = reportsDir + "/" + reportFileName
        
        let isPrintingTxt  = reportFormats |> Array.exists(fun x -> x = ReportFormat.Txt)
        let isPrintingHtml = reportFormats |> Array.exists(fun x -> x = ReportFormat.Html)
        let isPrintingCsv  = reportFormats |> Array.exists(fun x -> x = ReportFormat.Csv)

        if isPrintingTxt  then File.WriteAllText(filePath + ".txt", report.TxtReport)
        if isPrintingHtml then File.WriteAllText(filePath + ".html", report.HtmlReport)
        if isPrintingCsv  then File.WriteAllText(filePath + ".csv", report.CsvReport)
 
        Log.Information("reports saved in folder: '{0}', {1}", DirectoryInfo(reportsDir).FullName, Environment.NewLine)
        Log.Information(report.TxtReport)
    with
    | ex -> Log.Error(ex, "Report.save failed")