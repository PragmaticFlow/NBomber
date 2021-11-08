namespace NBomber.Plugins.Network.PsPing

open System
open System.Data
open System.Threading.Tasks
open System.Net.Sockets
open System.Diagnostics

open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration

open NBomber.Contracts
open NBomber.Extensions.InternalExtensions

[<CLIMutable>]
type PsPingPluginConfig = {
    Hosts: Uri[]
} with
    static member CreateDefault([<ParamArray>]hosts: string[]) =
        {
            Hosts = hosts |> Array.map Uri
        }

    static member CreateDefault(hosts: string seq) =
        hosts |> Seq.toArray |> PsPingPluginConfig.CreateDefault

type PsPingReply = {
    Status: string
    Address: Uri
    RoundtripTime: int64
}

module internal PsPingPluginStatistics =

    let private createColumn (name: string, caption: string, typeName: string) =
        let column = new DataColumn(name, Type.GetType(typeName))
        column.Caption <- caption
        column

    let private createColumns () =
        [| "Host", "Host", "System.String"
           "Port", "Port", "System.Int32"
           "Status", "Status", "System.String"
           "Address", "Address", "System.String"
           "RoundTripTime", "Round Trip Time", "System.String" |]
        |> Array.map(fun x -> x |> createColumn)

    let private createRow (table: DataTable, host: string, port: int, pingReply: PsPingReply, config: PsPingPluginConfig) =
        let row = table.NewRow()

        row.["Host"] <- host
        row.["Port"] <- port
        row.["Status"] <- pingReply.Status.ToString()
        row.["Address"] <- pingReply.Address.ToString()
        row.["RoundTripTime"] <- sprintf "%i ms" pingReply.RoundtripTime

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
            $"Physical latency to host: '%s{hostName}' on port: '%i{port}' is '%d{result.RoundtripTime}'.  This is bigger than 2ms which is not appropriate for load testing. You should run your test in an environment with very small latency."

        pingResults
        |> Seq.filter(fun (_,_,result) -> result.RoundtripTime > 2L)
        |> Seq.map printHint
        |> Seq.toArray

type PsPingPlugin(pluginConfig: PsPingPluginConfig) =

    let _pluginName = "NBomber.Plugins.Network.PsPingPlugin"
    let mutable _logger = Serilog.Log.ForContext<PsPingPlugin>()
    let mutable _pingResults = Array.empty
    let mutable _pluginStats = new DataSet()

    let execPing (config: PsPingPluginConfig) =
        try
            // from https://stackoverflow.com/questions/26067342/how-to-implement-psping-tcp-ping-in-c-sharp
            use sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            sock.Blocking <- true

            let replies =
                config.Hosts
                |> Array.map(fun uri ->
                    let stopwatch = Stopwatch()

                    // Measure the Connect call only
                    stopwatch.Start()
                    sock.Connect(uri.Host, uri.Port)
                    stopwatch.Stop()

                    let psPingReply = {
                        Status = if sock.Connected then "Connected" else "Unknown"
                        Address = uri
                        RoundtripTime = stopwatch.Elapsed.TotalMilliseconds |> int64
                    }

                    uri.Host, uri.Port, psPingReply)

            sock.Close()

            Ok replies
        with
        | ex -> Error ex

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
            |> createStats config
            |> Result.map(fun (pingResults,stats) ->
                _pingResults <- pingResults
                _pluginStats <- stats
            )
            |> Result.mapError(fun ex -> _logger.Error(ex.ToString()))
            |> ignore

            Task.CompletedTask

        member _.Start() = Task.CompletedTask
        member _.GetStats(currentOperation) = Task.singleton _pluginStats
        member _.GetHints() = PsPingPluginHintsAnalyzer.analyze _pingResults
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()
