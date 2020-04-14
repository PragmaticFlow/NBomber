namespace NBomber.Plugins.Network.Ping

open System
open System.Data
open System.Net.NetworkInformation
open System.Text
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open Serilog

open NBomber.Contracts

type PingPluginConfig = {
    Hosts: string list
    BufferSizeBytes: int
    Ttl: int
    DontFragment: bool
    Timeout: TimeSpan
} with
    static member Create(hosts: string seq) =
        { Hosts = hosts |> Seq.toList
          BufferSizeBytes = 32
          Ttl = 128
          DontFragment = false
          Timeout = TimeSpan.FromMilliseconds(120.0) }

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
           "TimeToLive", "Time to Live", "System.String"
           "DontFragment", "Don't Fragment", "System.String"
           "BufferSize", "Buffer Size", "System.String" |]
        |> Array.map(fun x -> x |> createColumn)

    let private createRow (host: string, pingReply: PingReply) (table: DataTable) =
        let areOptionsAvailable = pingReply.Options |> isNull |> not
        let row = table.NewRow()

        row.["Host"] <- host
        row.["Status"] <- pingReply.Status.ToString()
        row.["Address"] <- pingReply.Address.ToString()
        row.["RoundTripTime"] <- sprintf "%s ms" <| pingReply.RoundtripTime.ToString()
        row.["TimeToLive"] <- if areOptionsAvailable then pingReply.Options.Ttl.ToString() else "n/a"
        row.["DontFragment"] <- if areOptionsAvailable then pingReply.Options.DontFragment.ToString() else "n/a"
        row.["BufferSize"] <- sprintf "%s bytes" <| pingReply.Buffer.Length.ToString()

        row

    let createTable (statsName) (pingReplies: (string * PingReply) list) =
        let table = new DataTable(statsName)

        createColumns() |> table.Columns.AddRange

        pingReplies
        |> List.map(fun (host, pingReply) -> table |> createRow(host, pingReply))
        |> List.iter(fun x -> x |> table.Rows.Add)

        table

type PingPlugin(config: PingPluginConfig) =

    let mutable _logger: ILogger = Unchecked.defaultof<ILogger>
    let mutable _pluginStats = new DataSet()

    let execPing (config: PingPluginConfig) =
        try
            let pingOptions = PingOptions()
            pingOptions.Ttl <- config.Ttl
            pingOptions.DontFragment <- config.DontFragment

            let ping = new Ping()
            let buffer = Array.create config.BufferSizeBytes '-'
                         |> Encoding.ASCII.GetBytes

            let replies =
                config.Hosts
                |> List.map(fun host -> host, ping.Send(host, config.Timeout.Milliseconds, buffer, pingOptions))

            Ok replies
        with
        | ex -> Error ex

    let createStats (statsName) (pingReplyResult: Result<(string * PingReply) list, exn>) = result {
        let! pingResult = pingReplyResult
        let stats = new DataSet()

        pingResult
        |> PingPluginStatistics.createTable statsName
        |> stats.Tables.Add

        return stats
    }

    interface IPlugin with
        member x.PluginName = "NBomber.Plugins.Network.PingPlugin"

        member x.Init(logger, infraConfig) =
            _logger <- logger.ForContext<PingPlugin>()

        member x.StartTest(testInfo: TestInfo) =
            config
            |> execPing
            |> createStats (x :> IPlugin).PluginName
            |> Result.map(fun x -> _pluginStats <- x)
            |> Result.mapError(fun ex -> _logger.Error(ex.ToString()))
            |> ignore

            Task.CompletedTask

        member x.GetStats() = _pluginStats
        member x.StopTest() = Task.CompletedTask
        member x.Dispose() = ()
