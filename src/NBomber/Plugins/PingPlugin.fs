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
        [| "Key", "Property", "System.String"
           "Value", "Value", "System.String" |]
        |> Array.map(fun x -> x |> createColumn)

    let private createRow (key: string, value: string) (table: DataTable) =
        let row = table.NewRow()
        row.["Key"] <- key
        row.["Value"] <- value
        row

    let private createRows (pingReply: PingReply) (table: DataTable) = [|
        let areOptionsAvailable = pingReply.Options |> isNull |> not

        let roundTripTime = sprintf "%s ms" <| pingReply.RoundtripTime.ToString()
        let timeToLive = if areOptionsAvailable then pingReply.Options.Ttl.ToString() else "n/a"
        let dontFragment = if areOptionsAvailable then pingReply.Options.DontFragment.ToString() else "n/a"
        let bufferSize = sprintf "%s bytes" <| pingReply.Buffer.Length.ToString()

        table |> createRow("Status", pingReply.Status.ToString())
        table |> createRow("Address", pingReply.Address.ToString())
        table |> createRow("RoundTrip time", roundTripTime)
        table |> createRow("Time to live", timeToLive)
        table |> createRow("Don't fragment", dontFragment)
        table |> createRow("Buffer size", bufferSize)
    |]

    let private createTable (name) (pingReply: PingReply) =
        let table = new DataTable(name)

        createColumns() |> table.Columns.AddRange

        table
        |> createRows pingReply
        |> Array.iter(fun x -> x |> table.Rows.Add)

        table

    let createTables (host: string, pingReply: PingReply) =
        let tableName = sprintf "Ping %s" host
        pingReply |> createTable tableName

type PingPlugin(config: PingPluginConfig) =

    let mutable _logger: ILogger = Unchecked.defaultof<ILogger>
    let mutable _stats = new PluginStats()

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

    let createStats (pingReplyResult: Result<(string * PingReply) list, exn>) = result {
        let! pingResult = pingReplyResult
        let stats = new PluginStats()

        pingResult
        |> List.map(fun x -> x |> PingPluginStatistics.createTables)
        |> Array.ofList
        |> stats.Tables.AddRange

        return stats
    }

    interface IPlugin with
        member x.PluginName = "NBomber.Plugins.Network.PingPlugin"

        member x.Init(logger, infraConfig) =
            _logger <- logger.ForContext<PingPlugin>()

        member x.StartTest(testInfo: TestInfo) =
            config
            |> execPing
            |> createStats
            |> Result.map(fun x -> _stats <- x)
            |> Result.mapError(fun ex -> _logger.Error(ex.ToString()))
            |> ignore

            Task.CompletedTask

        member x.GetStats() = _stats
        member x.StopTest() = Task.CompletedTask
        member x.Dispose() = ()
