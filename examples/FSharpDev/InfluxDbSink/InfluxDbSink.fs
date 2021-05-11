namespace FSharpDev.InfluxDbSink

open System
open System.Runtime.InteropServices
open System.Threading.Tasks

open Serilog
open App.Metrics
open App.Metrics.Gauge
open App.Metrics.Reporting.InfluxDB
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats

[<CLIMutable>]
type InfluxDbSinkConfig = {
    Url: string
    Database: string
    UserName: string
    Password: string
} with
    [<CompiledName("Create")>]
    static member create(url: string,
                         database: string,
                         [<Optional;DefaultParameterValue("")>] userName: string,
                         [<Optional;DefaultParameterValue("")>] password: string) =

        { Url = url; Database = database; UserName = userName; Password = password }

type InfluxDBSink(metricsRoot: IMetricsRoot) =

    let mutable _logger = Unchecked.defaultof<ILogger>
    let mutable _metricsRoot = metricsRoot
    let mutable _context = Unchecked.defaultof<IBaseContext>

    let getOperationName (operation: OperationType) =
        match operation with
        | OperationType.Bombing  -> "bombing"
        | OperationType.Complete -> "complete"
        | _                      -> "bombing"

    let saveScenarioStats (stats: ScenarioStats) =
        let operation = getOperationName(stats.CurrentOperation)
        let nodeType = _context.NodeInfo.NodeType.ToString()
        let testInfo = _context.TestInfo
        let simulation = stats.LoadSimulationStats

        stats.StepStats
        |> Array.iter(fun s ->
            try
                let okR = s.Ok.Request
                let okL = s.Ok.Latency
                let okD = s.Ok.DataTransfer

                let fR = s.Fail.Request
                let fL = s.Fail.Latency
                let fD = s.Fail.DataTransfer

                [("Ok.Request.Count", float okR.Count); ("Ok.Request.RPS", float okR.RPS)
                 ("Ok.Latency.Min", float okL.MinMs); ("Ok.Latency.Mean", float okL.MeanMs)
                 ("Ok.Latency.Max", float okL.MaxMs); ("Ok.Latency.StdDev", float okL.StdDev)
                 ("Ok.Latency.Percent50", float okL.Percent50); ("Ok.Latency.Percent75", float okL.Percent75)
                 ("Ok.Latency.Percent95", float okL.Percent95); ("Ok.Latency.Percent99", float okL.Percent99)
                 ("Ok.DataTransfer.Min", float okD.MinBytes); ("Ok.DataTransfer.Mean", float okD.MeanBytes)
                 ("Ok.DataTransfer.Max", float okD.MaxBytes); ("Ok.DataTransfer.AllMb", okD.AllBytes |> mb |> float)

                 ("Fail.Request.Count", float fR.Count); ("Fail.Request.RPS", float fR.RPS)
                 ("Fail.Latency.Min", float fL.MinMs); ("Fail.Latency.Mean", float fL.MeanMs)
                 ("Fail.Latency.Max", float fL.MaxMs); ("Fail.Latency.StdDev", float fL.StdDev)
                 ("Fail.Latency.Percent50", float fL.Percent50); ("Fail.Latency.Percent75", float fL.Percent75)
                 ("Fail.Latency.Percent95", float fL.Percent95); ("Fail.Latency.Percent99", float fL.Percent99)
                 ("Fail.DataTransfer.Min", float fD.MinBytes); ("Fail.DataTransfer.Mean", float fD.MeanBytes)
                 ("Fail.DataTransfer.Max", float fD.MaxBytes); ("Fail.DataTransfer.AllMb", fD.AllBytes |> mb |> float)

                 ("simulation.value", float simulation.Value)]

                |> List.iter(fun (name, value) ->
                    let metric =
                        GaugeOptions(
                            Name = name,
                            Context = "NBomber",
                            Tags = MetricTags([|"node_type"; "test_suite"; "test_name"
                                                "scenario"; "step"; "operation"; "simulation.name"|],
                                              [|nodeType; testInfo.TestSuite; testInfo.TestName
                                                stats.ScenarioName; s.StepName; operation; simulation.SimulationName|]))

                    _metricsRoot.Measure.Gauge.SetValue(metric, value)
                )
            with
            | ex -> _logger.Error(ex.ToString())
        )

    static let createMetricsRoot (config: InfluxDbSinkConfig) =
        let options = MetricsReportingInfluxDbOptions()
        options.InfluxDb.BaseUri <- Uri(config.Url)
        options.InfluxDb.Database <- config.Database
        options.InfluxDb.UserName <- config.UserName
        options.InfluxDb.Password <- config.Password
        MetricsBuilder().Report.ToInfluxDb(options).Build()

    new (config: InfluxDbSinkConfig) =
        new InfluxDBSink(createMetricsRoot config)

    new() = new InfluxDBSink(null)

    interface IReportingSink with
        member x.SinkName = "NBomber.Sinks.InfluxDB"

        member x.Init(context: IBaseContext, infraConfig: IConfiguration) =
            _logger <- context.Logger.ForContext<InfluxDBSink>()
            _context <- context

            infraConfig
            |> Option.ofObj
            |> Option.map(fun x -> x.GetSection("InfluxDBSink").Get<InfluxDbSinkConfig>())
            |> Option.bind(fun x ->
                if not(x |> box |> isNull) then Some x
                else None
            )
            |> Option.iter(fun config ->
                _metricsRoot <- createMetricsRoot config
            )

            Task.CompletedTask

        member x.Start() = Task.CompletedTask

        member x.SaveRealtimeStats(stats: ScenarioStats[]) =
            stats |> Array.iter(saveScenarioStats)
            Task.WhenAll(_metricsRoot.ReportRunner.RunAllAsync())

        member x.SaveFinalStats(stats: NodeStats[]) = Task.CompletedTask
        member x.Stop() = Task.CompletedTask
        member x.Dispose() = ()
