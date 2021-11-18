namespace NBomber.Plugins.Network.PsPing

open System
open System.Data
open System.Threading.Tasks
open System.Net.Sockets
open System.Diagnostics

open FSharp.Control.Tasks.NonAffine
open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Contracts
open NBomber.Domain.Stats.Statistics
open NBomber.Extensions.InternalExtensions

[<CLIMutable>]
type PsPingPluginConfig = {
    Hosts: Uri[]
    /// The default is 1000 ms.
    Timeout: int
    /// Number of ping executions. The default is 4.
    Executions: int
} with
    static member CreateDefault([<ParamArray>]hosts: string[]) =
        {
            Hosts = hosts |> Array.map Uri
            Timeout = 1_000
            Executions = 4
        }

    static member CreateDefault(hosts: string seq) =
        hosts |> Seq.toArray |> PsPingPluginConfig.CreateDefault

type PsPingReply = {
    Address: Uri
    RequestCountSucceeded: int
    RequestCountFailed: int
    RoundtripTimeMin: int64
    RoundtripTimeMax: int64
    RoundtripTimeAvg: float
    RoundtripTimeStdDev: float
}

module internal PsPingPluginStatistics =

    let private createColumn (name: string, caption: string, typeName: string) =
        let column = new DataColumn(name, Type.GetType(typeName))
        column.Caption <- caption
        column

    let private createColumns () =
        [| "Host", "Host", "System.String"
           "Port", "Port", "System.Int32"
           "Address", "Address", "System.String"
           "Requests", "Requests", "System.String"
           "RoundTripTime", "Round Trip Time", "System.String" |]
        |> Array.map(fun x -> x |> createColumn)

    let private createRow (table: DataTable, host: string, port: int, pingReply: PsPingReply, config: PsPingPluginConfig) =
        let row = table.NewRow()

        row.["Host"] <- host
        row.["Port"] <- port
        row.["Address"] <- pingReply.Address.ToString()
        row.["Requests"] <- $"all = {pingReply.RequestCountSucceeded + pingReply.RequestCountFailed}, ok = {pingReply.RequestCountSucceeded}, fail = {pingReply.RequestCountFailed}"
        row.["RoundTripTime"] <-
            $"min = {pingReply.RoundtripTimeMin}" +
            $", mean = {pingReply.RoundtripTimeAvg |> Converter.round(Constants.StatsRounding)}" +
            $", max = {pingReply.RoundtripTimeMax}" +
            $", StdDev = {pingReply.RoundtripTimeStdDev |> Converter.round(Constants.StatsRounding)}"

        row

    let createTable (statsName) (config: PsPingPluginConfig) (pingReplies: (string * int * PsPingReply)[]) =
        let table = new DataTable(statsName)

        createColumns() |> table.Columns.AddRange

        pingReplies
        |> Array.map(fun (host, port, pingReply) -> createRow(table, host, port, pingReply, config))
        |> Array.iter(fun x -> x |> table.Rows.Add)

        table

module internal PsPingPluginHintsAnalyzer =

    /// (hostName * result)[]
    let analyze (pingResults: (string * int * PsPingReply)[]) =

        let printHint (hostName, port, result: PsPingReply) =
            $"Physical latency to host: '%s{hostName}' on port: '%i{port}' is '%f{result.RoundtripTimeAvg}'.  This is bigger than 2ms which is not appropriate for load testing. You should run your test in an environment with very small latency."

        pingResults
        |> Seq.filter(fun (_,_,result) -> result.RoundtripTimeAvg > 2.0)
        |> Seq.map printHint
        |> Seq.toArray

type PsPingPlugin(pluginConfig: PsPingPluginConfig) =

    let _pluginName = "NBomber.Plugins.Network.PsPingPlugin"
    let mutable _logger = Serilog.Log.ForContext<PsPingPlugin>()
    let mutable _pingResults = Array.empty
    let mutable _pluginStats = new DataSet()

    let execPing (config: PsPingPluginConfig) = task {
        try
            let! replies =
                config.Hosts
                |> Array.map(fun uri -> task {
                    let! results =
                        [1..config.Executions]
                        |> Seq.map(fun _ -> task {
                            // from https://stackoverflow.com/questions/26067342/how-to-implement-psping-tcp-ping-in-c-sharp
                            use sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                            sock.Blocking <- true

                            let stopwatch = Stopwatch()

                            // Measure the Connect call only
                            stopwatch.Start()
                            let connectTask = sock.ConnectAsync(uri.Host, uri.Port)
                            let timeoutTask = Task.Delay(config.Timeout)
                            do! Task.WhenAny(connectTask, timeoutTask) |> Task.map ignore
                            stopwatch.Stop()

                            let result =
                                sock.Connected,
                                stopwatch.Elapsed.TotalMilliseconds |> int64

                            sock.Close()

                            return result
                        })
                        |> Task.WhenAll

                    let totalMsResults = results |> Seq.map (fun (_, totalMs) -> totalMs |> float)
                    let avg = totalMsResults |> Seq.average
                    let psPingReply = {
                        Address = uri
                        RequestCountSucceeded = results |> Seq.filter (fun (connected, _) -> connected) |> Seq.length
                        RequestCountFailed = results |> Seq.filter (fun (connected, _) -> not connected) |> Seq.length
                        RoundtripTimeMin = totalMsResults |> Seq.min |> int64
                        RoundtripTimeMax = totalMsResults |> Seq.max |> int64
                        RoundtripTimeAvg = avg
                        RoundtripTimeStdDev =
                            let sumOfSquaresOfDifferences = totalMsResults |> Seq.map (fun totalMs -> (totalMs - avg) * (totalMs - avg)) |> Seq.sum
                            Math.Sqrt(sumOfSquaresOfDifferences / (totalMsResults |> Seq.length |> float))
                    }

                    return uri.Host, uri.Port, psPingReply
                })
                |> Task.WhenAll



            return Ok replies
        with
        | ex -> return Error ex
    }

    let createStats (config: PsPingPluginConfig) (pingReplyResult: Result<(string * int * PsPingReply)[], exn>) = result {
        let! pingResult = pingReplyResult
        let stats = new DataSet()

        pingResult
        |> PsPingPluginStatistics.createTable _pluginName config
        |> stats.Tables.Add

        return pingResult, stats
    }

    new() = new PsPingPlugin(PsPingPluginConfig.CreateDefault())

    interface IWorkerPlugin with
        member _.PluginName = _pluginName

        member _.Init(context, infraConfig) =
            _logger <- context.Logger.ForContext<PsPingPlugin>()

            let config =
                infraConfig.GetSection("PingPlugin").Get<PsPingPluginConfig>()
                |> Option.ofRecord
                |> Option.defaultValue pluginConfig

            _logger.Verbose("PingPlugin config: @{PingPluginConfig}", config)

            config
            |> execPing
            |> Task.map (createStats config)
            |> Task.map (Result.map(fun (pingResults,stats) ->
                _pingResults <- pingResults
                _pluginStats <- stats
            ))
            |> Task.map (Result.mapError(fun ex -> _logger.Error(ex.ToString())))
            |> Task.map ignore
            :> Task

        member _.Start() = Task.CompletedTask
        member _.GetStats(currentOperation) = Task.singleton _pluginStats
        member _.GetHints() = PsPingPluginHintsAnalyzer.analyze _pingResults
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()
