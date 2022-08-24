namespace NBomber.Plugins.Network.Ping

open System
open System.Data
open System.Net.NetworkInformation
open System.Text
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration

open NBomber.Contracts
open NBomber.Extensions.Internal

[<CLIMutable>]
type PingPluginConfig = {
    Hosts: string[]
    /// A buffer of data to be transmitted. The default is 32.
    /// If you believe that a larger (or smaller) packet size will noticeably affect
    /// the response time from the target host, then you may wish to experiment with
    /// different values.  The range of sizes is from 1 to 65500.  Note that values
    /// (for Ethernet) require that the packet be fragmented for any value over 1386
    /// bytes in the data field.
    BufferSizeBytes: int
    /// Sets the number of routing nodes that can forward the Ping data before it is discarded.
    /// An Int32 value that specifies the number of times the Ping data packets can be forwarded. The default is 128.
    Ttl: int
    /// Sets a Boolean value that controls fragmentation of the data sent to the remote host.
    /// true if the data cannot be sent in multiple packets; otherwise false. The default is false
    /// This option is useful if you want to test the maximum transmission unit (MTU)
    /// of the routers and gateways used to transmit the packet.
    /// If this property is true and the data sent to the remote host is larger then the MTU of a gateway
    /// or router between the sender and the remote host, the ping operation fails with status PacketTooBig.
    DontFragment: bool
    /// The default is 1000 ms.
    Timeout: int
} with

    [<CompiledName("CreateDefault")>]
    static member createDefault([<ParamArray>]hosts: string[]) = {
        Hosts = hosts
        BufferSizeBytes = 32
        Ttl = 128
        DontFragment = false
        Timeout = 1_000
    }

    [<CompiledName("CreateDefault")>]
    static member createDefault(hosts: string seq) =
        hosts |> Seq.toArray |> PingPluginConfig.createDefault

module internal PingPluginStatistics =

    let private createColumn (name: string, caption: string, typeName: string) =
        let column = new DataColumn(name, Type.GetType(typeName))
        column.Caption <- caption
        column

    let private createColumns () =
        [| "Host", "Host", "System.String"
           "Status", "Status", "System.String"
           "Address", "Address", "System.String"
           "RoundTripTime", "Round Trip Time", "System.String"
           "Ttl", "Time to Live", "System.String"
           "DontFragment", "Don't Fragment", "System.String"
           "BufferSize", "Buffer Size", "System.String" |]
        |> Array.map(fun x -> x |> createColumn)

    let private createRow (table: DataTable, host: string, pingReply: PingReply, config: PingPluginConfig) =
        let row = table.NewRow()

        row["Host"] <- host
        row["Status"] <- pingReply.Status.ToString()
        row["Address"] <- pingReply.Address.ToString()
        row["RoundTripTime"] <- $"%i{pingReply.RoundtripTime} ms"
        row["Ttl"] <- config.Ttl.ToString()
        row["DontFragment"] <- config.DontFragment.ToString()
        row["BufferSize"] <- $"%i{config.BufferSizeBytes} bytes"

        row

    let createTable (statsName) (config: PingPluginConfig) (pingReplies: (string * PingReply)[]) =
        let table = new DataTable(statsName)

        createColumns() |> table.Columns.AddRange

        pingReplies
        |> Array.map(fun (host, pingReply) -> createRow(table, host, pingReply, config))
        |> Array.iter(fun x -> x |> table.Rows.Add)

        table

module internal PingPluginHintsAnalyzer =

    /// (hostName * result)[]
    let analyze (pingResults: (string * PingReply)[]) =

        let printHint (hostName, result: PingReply) =
            $"Physical latency to host: '%s{hostName}' is '%d{result.RoundtripTime}'. This is bigger than 2ms which is not appropriate for load testing. You should run your test in an environment with very small latency"

        pingResults
        |> Seq.filter(fun (_,result) -> result.RoundtripTime > 2L)
        |> Seq.map printHint
        |> Seq.toArray

type PingPlugin(pluginConfig: PingPluginConfig) =

    let _pluginName = "NBomber.Plugins.Network.PingPlugin"
    let mutable _logger = Serilog.Log.ForContext<PingPlugin>()
    let mutable _pingResults = Array.empty
    let mutable _pluginStats = new DataSet()

    let execPing (config: PingPluginConfig) =
        try
            let pingOptions = PingOptions()
            pingOptions.Ttl <- config.Ttl
            pingOptions.DontFragment <- config.DontFragment

            let ping = new Ping()

            let buffer =
                Array.create config.BufferSizeBytes '-'
                |> Encoding.ASCII.GetBytes

            let replies =
                config.Hosts
                |> Array.map(fun host -> host, ping.Send(host, int config.Timeout, buffer, pingOptions))

            Ok replies
        with
        | ex -> Error ex

    let createStats (config: PingPluginConfig) (pingReplyResult: Result<(string * PingReply)[], exn>) = result {
        let! pingResult = pingReplyResult
        let stats = new DataSet()

        pingResult
        |> PingPluginStatistics.createTable _pluginName config
        |> stats.Tables.Add

        return pingResult, stats
    }

    new() = new PingPlugin(PingPluginConfig.createDefault())

    interface IWorkerPlugin with
        member _.PluginName = _pluginName

        member _.Init(context, infraConfig) =
            _logger <- context.Logger.ForContext<PingPlugin>()

            let config =
                infraConfig.GetSection("PingPlugin").Get<PingPluginConfig>()
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
        member _.GetStats(stats) = Task.singleton _pluginStats
        member _.GetHints() = PingPluginHintsAnalyzer.analyze _pingResults
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()
