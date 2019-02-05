module internal NBomber.DomainServices.Reporting.Report

open System
open System.IO

open Serilog

open NBomber.Domain.StatisticsTypes
open NBomber.Domain.Errors
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.Contracts

type ReportResult = {
      TxtReport: string
      HtmlReport: string
      CsvReport: string
      MdReport: string
    }

let build (dep: Dependency, stats: GlobalStats, failedAsserts: DomainError[]) =
    { TxtReport = TxtReport.print(stats, failedAsserts)
      HtmlReport = HtmlReport.print(dep, stats, failedAsserts)
      CsvReport = CsvReport.print(stats)
      MdReport = MdReport.print (stats, failedAsserts)
    }


let save (dep: Dependency, outPutDir: string, reportFileName: string, reportFormats: ReportFormat[]) (report: ReportResult) =
    try
        let reportsDir = Path.Combine(outPutDir, "reports")
        Directory.CreateDirectory(reportsDir) |> ignore
        ResourceManager.saveAssets(reportsDir)

        [ ReportFormat.Txt, report.TxtReport, ".txt"
          ReportFormat.Html, report.HtmlReport, ".html"
          ReportFormat.Csv, report.CsvReport, ".csv"
          ReportFormat.Md, report.MdReport, ".md" ]
        |> List.iter
               (fun (format, report, ext) ->
               let filePath = reportsDir + "/" + reportFileName + ext
               if reportFormats |> Array.contains format then
                   File.WriteAllText(filePath, report))

        Log.Information("reports saved in folder: '{0}', {1}", DirectoryInfo(reportsDir).FullName, Environment.NewLine)
        Log.Information(report.TxtReport)
    with
    | ex -> Log.Error(ex, "Report.save failed")
