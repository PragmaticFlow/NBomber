module internal NBomber.DomainServices.Reporting.Report

open System
open System.IO

open Serilog

open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes
open NBomber.Domain.Errors
open NBomber.Infra
open NBomber.Infra.Dependency

type ReportResult = {
    TxtReport: string
    HtmlReport: string    
}

let build (dep: Dependency, stats: GlobalStats,
           assertResults: Result<Assertion,DomainError>[]) =
    { TxtReport = TxtReport.print(stats)
      HtmlReport = HtmlReport.print(dep, stats) }

let save (dep: Dependency, outPutDir: string) (report: ReportResult) =
    try
        let reportsDir = Path.Combine(outPutDir, "reports")
        Directory.CreateDirectory(reportsDir) |> ignore
        ResourceManager.saveAssets(reportsDir)

        let filePath = reportsDir + "/report_" + dep.SessionId

        File.WriteAllText(filePath + ".txt", report.TxtReport)
        File.WriteAllText(filePath + ".html", report.HtmlReport)    
        Log.Information("reports saved in folder: '{0}', {1}", DirectoryInfo(reportsDir).FullName, Environment.NewLine)
        Log.Information(report.TxtReport)
    with
    | ex -> Log.Error(ex, "Report.save failed")