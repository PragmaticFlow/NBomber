module internal NBomber.DomainServices.Reporting.Report

open System
open System.IO

open Serilog

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
}

//todo: fix stats.[0]
let build (dep: Dependency, stats: NodeStats[], failedAsserts: DomainError[]) =
    { TxtReport = TxtReport.print(stats.[0], failedAsserts)
      HtmlReport = HtmlReport.print(dep, stats.[0], failedAsserts)
      CsvReport = CsvReport.print(stats.[0])
      MdReport = MdReport.print(stats.[0], failedAsserts) }

let save (dep: Dependency, outPutDir: string,
          reportFileName: string, reportFormats: ReportFormat list, report: ReportResult) =
    try
        let reportsDir = Path.Combine(outPutDir, "reports")
        reportsDir |> Directory.CreateDirectory |> ignore
        reportsDir |> ResourceManager.saveAssets

        [ ReportFormat.Txt,  report.TxtReport,  ".txt"
          ReportFormat.Html, report.HtmlReport, ".html"
          ReportFormat.Csv,  report.CsvReport,  ".csv"
          ReportFormat.Md,   report.MdReport,   ".md" ]
        |> List.iter
               (fun (format, report, ext) ->
               let filePath = reportsDir + "/" + reportFileName + ext
               if reportFormats |> List.contains format then
                   File.WriteAllText(filePath, report))

        Log.Information("reports saved in folder: '{0}', {1}", DirectoryInfo(reportsDir).FullName, Environment.NewLine)
        Log.Information report.TxtReport
    with
    | ex -> Log.Error(ex, "Report.save failed")
