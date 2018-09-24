module internal NBomber.DomainServices.Reporting.Report

open System.IO

open NBomber.Domain.StatisticsTypes
open NBomber.Infra
open NBomber.Infra.Dependency

type ReportResult = {
    TxtReport: string
    HtmlReport: string    
}

let build (dep: Dependency, scResult: ScenarioStats) =
    { TxtReport = TxtReport.print(dep, scResult)
      HtmlReport = HtmlReport.print(dep, scResult) }

let save (dep: Dependency, report: ReportResult, outPutDir: string) =
    let reportsDir = Path.Combine(outPutDir, "reports")
    Directory.CreateDirectory(reportsDir) |> ignore
    ResourceManager.saveAssets(reportsDir)

    let filePath = reportsDir + "/report-" + dep.SessionId

    File.WriteAllText(filePath + ".txt", report.TxtReport)
    File.WriteAllText(filePath + ".html", report.HtmlReport)