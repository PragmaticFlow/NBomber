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

let build (dep: Dependency, stats: GlobalStats,
           assertResults: Result<Assertion,DomainError>[]) =
    { TxtReport = TxtReport.print(stats)
      HtmlReport = HtmlReport.print(dep, stats)
      CsvReport = CsvReport.print(stats) }

let save (dep: Dependency, outPutDir: string, outputFilename: string option, outputFiletypes: FileType[]) (report: ReportResult) =
    try
        let reportsDir = Path.Combine(outPutDir, "reports")
        Directory.CreateDirectory(reportsDir) |> ignore
        ResourceManager.saveAssets(reportsDir)
        
        let fileName = outputFilename |> Option.defaultValue ("report_" + dep.SessionId)
        let filePath = reportsDir + "/" + fileName
        
        let isPrintingTxt = outputFiletypes |> Array.exists(fun x -> x = FileType.Txt)
        let isPrintingHtml = outputFiletypes |> Array.exists(fun x -> x = FileType.Html)
        let isPrintingCsv = outputFiletypes |> Array.exists(fun x -> x = FileType.Csv)

        if(isPrintingTxt) then File.WriteAllText(filePath + ".txt", report.TxtReport)
        if(isPrintingHtml) then File.WriteAllText(filePath + ".Html", report.HtmlReport)
        if(isPrintingCsv) then File.WriteAllText(filePath + ".csv", report.CsvReport)
 
        Log.Information("reports saved in folder: '{0}', {1}", DirectoryInfo(reportsDir).FullName, Environment.NewLine)
        Log.Information(report.TxtReport)
    with
    | ex -> Log.Error(ex, "Report.save failed")