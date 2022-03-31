namespace NBomber.Plugins.Network.PsPing

open System
open System.Data
open System.Threading.Tasks
open System.Net.Sockets
open System.Diagnostics

open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration

open NBomber.Contracts
open NBomber.Extensions.Internal

[<CLIMutable>]
type PsPingPluginConfig = {
    Hosts: Uri[]
    /// The default is 1000 ms.
    Timeout: int
} with

    [<CompiledName("CreateDefault")>]
    static member createDefault([<ParamArray>]hosts: string[]) =
        { Hosts = hosts |> Array.map Uri
          Timeout = 1_000 }

    [<CompiledName("CreateDefault")>]
    static member createDefault(hosts: string seq) =
        hosts |> Seq.toArray |> PsPingPluginConfig.createDefault

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

        row["Host"] <- host
        row["Port"] <- port
        row["Status"] <- pingReply.Status.ToString()
        row["Address"] <- pingReply.Address.ToString()
        row["RoundTripTime"] <- $"%i{pingReply.RoundtripTime} ms"

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
            $"Physical latency to host: '%s{hostName}' on port: '%i{port}' is '%d{result.RoundtripTime}'. This is bigger than 2ms which is not appropriate for load testing. You should run your test in an environment with very small latency"

        pingResults
        |> Seq.filter(fun (_,_,result) -> result.RoundtripTime > 2L)
        |> Seq.map printHint
        |> Seq.toArray

type PsPingPlugin(pluginConfig: PsPingPluginConfig) =

    let _pluginName = "NBomber.Plugins.Network.PsPingPlugin"
    let mutable _logger = Serilog.Log.ForContext<PsPingPlugin>()
    let mutable _pingResults = Array.empty
    let mutable _pluginStats = new DataSet()

    let execPing (config: PsPingPluginConfig) = backgroundTask {
        try
            // from https://stackoverflow.com/questions/26067342/how-to-implement-psping-tcp-ping-in-c-sharp
            use sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            sock.Blocking <- true

            let! replies =
                config.Hosts
                |> Array.map(fun uri -> backgroundTask {
                    let stopwatch = Stopwatch()

                    // measure the Connect call only
                    stopwatch.Start()
                    let connectTask = sock.ConnectAsync(uri.Host, uri.Port)
                    let timeoutTask = Task.Delay(config.Timeout)
                    do! Task.WhenAny(connectTask, timeoutTask) |> Task.map ignore
                    stopwatch.Stop()

                    let psPingReply = {
                        Status = if sock.Connected then "Connected" else "NotConnected/TimedOut"
                        Address = uri
                        RoundtripTime = stopwatch.Elapsed.TotalMilliseconds |> int64
                    }

                    return uri.Host, uri.Port, psPingReply
                })
                |> Task.WhenAll

            sock.Close()

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

    new() = new PsPingPlugin(PsPingPluginConfig.createDefault())

    interface IWorkerPlugin with
        member _.PluginName = _pluginName

        member _.Init(context, infraConfig) =
            _logger <- context.Logger.ForContext<PsPingPlugin>()

            let config =
                infraConfig.GetSection("PsPingPlugin").Get<PsPingPluginConfig>()
                |> Option.ofRecord
                |> Option.defaultValue pluginConfig

            _logger.Verbose("PsPingPlugin config: @{PsPingPluginConfig}", config)

            config
            |> execPing
            |> Task.map(createStats config)
            |> Task.map(Result.map(fun (pingResults,stats) ->
                _pingResults <- pingResults
                _pluginStats <- stats
            ))
            |> Task.map(Result.mapError(fun ex -> _logger.Error(ex.ToString())))
            |> Task.map ignore
            :> Task

        member _.Start() = Task.CompletedTask
        member _.GetStats(currentOperation) = Task.singleton _pluginStats
        member _.GetHints() = PsPingPluginHintsAnalyzer.analyze _pingResults
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()
