module NBomber.Plugins

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
    static member Create (hosts) =
        { Hosts = hosts
          BufferSizeBytes = 32
          Ttl = 128
          DontFragment = false
          Timeout = TimeSpan.FromMilliseconds(120.0) }

type PingPluginError =
    | PingError of ex:Exception

  module internal PingPluginStatisticsHelper =

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

        yield table |> createRow("Status", pingReply.Status.ToString())
        yield table |> createRow("Address", pingReply.Address.ToString())
        yield table |> createRow("RoundTrip time", roundTripTime)
        yield table |> createRow("Time to live", timeToLive)
        yield table |> createRow("Don't fragment", dontFragment)
        yield table |> createRow("Buffer size", bufferSize)
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

type PingPlugin (config: PingPluginConfig) =
    let mutable _logger: ILogger = null
    let mutable _stats = new PluginStatistics()

    let ping (config: PingPluginConfig) =
        try
            let pingOptions = PingOptions()
            pingOptions.Ttl <- config.Ttl
            pingOptions.DontFragment <- config.DontFragment

            let ping = new Ping()
            let buffer = Array.create config.BufferSizeBytes '-' |> Encoding.ASCII.GetBytes

            let replies =
                config.Hosts
                |> List.map (fun x -> x, ping.Send(x, config.Timeout.Milliseconds, buffer, pingOptions))

            Ok replies
        with
        | ex -> Error <| PingError ex

    let createStats (pingReplyResult: Result<(string * PingReply) list, PingPluginError>) = result {
        let! pingResult = pingReplyResult
        let stats = new PluginStatistics()

        pingResult
        |> List.map (fun x -> x |> PingPluginStatisticsHelper.createTables)
        |> Array.ofList
        |> stats.Tables.AddRange

        return stats
    }

    interface IPlugin with
        member x.Init (logger, infraConfig) =
            _logger <- logger.ForContext<PingPlugin>()

        member x.StartTest (testInfo: TestInfo) =
            config
            |> ping
            |> createStats
            |> Result.mapError(fun x ->
                match x with
                | PingError ex -> _logger.Error(ex, ex.Message)
            )
            |> Result.map(fun x -> _stats <- x)
            |> ignore

            Task.CompletedTask

        member x.GetStats (testInfo: TestInfo) =
            Task.FromResult(_stats)

        member x.FinishTest (testInfo: TestInfo) =
            Task.CompletedTask
