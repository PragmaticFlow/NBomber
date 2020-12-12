namespace NBomber.Plugins.Network.Ping

open System
open System.Data
open System.Net.NetworkInformation
open System.Text
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration

open NBomber.Contracts
open NBomber.Extensions.InternalExtensions

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
    static member CreateDefault([<ParamArray>]hosts: string seq) = {
        Hosts = hosts |> Seq.toArray
        BufferSizeBytes = 32
        Ttl = 128
        DontFragment = false
        Timeout = 1_000
    }

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

        row.["Host"] <- host
        row.["Status"] <- pingReply.Status.ToString()
        row.["Address"] <- pingReply.Address.ToString()
        row.["RoundTripTime"] <- sprintf "%i ms" pingReply.RoundtripTime
        row.["Ttl"] <- config.Ttl.ToString()
        row.["DontFragment"] <- config.DontFragment.ToString()
        row.["BufferSize"] <- sprintf "%i bytes" config.BufferSizeBytes

        row

    let createTable (statsName) (config: PingPluginConfig) (pingReplies: (string * PingReply)[]) =
        let table = new DataTable(statsName)

        createColumns() |> table.Columns.AddRange

        pingReplies
        |> Array.map(fun (host, pingReply) -> createRow(table, host, pingReply, config))
        |> Array.iter(fun x -> x |> table.Rows.Add)

        table

type PingPlugin(pluginConfig: PingPluginConfig) =

    let mutable _logger = Serilog.Log.ForContext<PingPlugin>()
    let mutable _pluginStats = new DataSet()
    let mutable _config = pluginConfig
    let _pluginName = "NBomber.Plugins.Network.PingPlugin"

    let execPing () =
        try
            let pingOptions = PingOptions()
            pingOptions.Ttl <- _config.Ttl
            pingOptions.DontFragment <- _config.DontFragment

            let ping = new Ping()
            let buffer = Array.create _config.BufferSizeBytes '-'
                         |> Encoding.ASCII.GetBytes

            let replies =
                _config.Hosts
                |> Array.map(fun host -> host, ping.Send(host, int _config.Timeout, buffer, pingOptions))

            Ok replies
        with
        | ex -> Error ex

    let createStats (pingReplyResult: Result<(string * PingReply)[], exn>) = result {
        let! pingResult = pingReplyResult
        let stats = new DataSet()

        pingResult
        |> PingPluginStatistics.createTable _pluginName _config
        |> stats.Tables.Add

        return stats
    }

    new() = new PingPlugin(PingPluginConfig.CreateDefault Seq.empty)

    interface IWorkerPlugin with
        member _.PluginName = _pluginName

        member _.Init(logger, infraConfig) =
            _logger <- logger.ForContext<PingPlugin>()

            _config <-
                infraConfig
                |> Option.bind(fun x -> x.GetSection("PingPlugin").Get<PingPluginConfig>() |> Option.ofRecord)
                |> Option.defaultValue pluginConfig

        member _.Start(testInfo: TestInfo) =
            execPing()
            |> createStats
            |> Result.map(fun x -> _pluginStats <- x)
            |> Result.mapError(fun ex -> _logger.Error(ex.ToString()))
            |> ignore

            Task.CompletedTask

        member _.GetStats(currentOperation) = _pluginStats
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()
