module internal NBomber.DomainServices.Reporting.HtmlReport

open NBomber.Contracts
open NBomber.DomainServices.ResourceManager

let print (stats: NodeStats) =
    let template = HtmlReportResourceManager.readIndexHtml()
    template
