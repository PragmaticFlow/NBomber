module PingExtensionScenario

open System
open System.Data
open System.Net.Http
open System.Net.NetworkInformation
open System.Text
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling
open Serilog

open NBomber.Contracts
open NBomber.FSharp

// it's a very basic PingExtension example to give you a playground for writing your own custom extension

type PingExtensionConfig =
    { Host: string
      BufferSizeBytes: int
      Ttl: int
      DontFragment: bool
      Timeout: TimeSpan }

    static member Create (host) =
        { Host = host
          BufferSizeBytes = 32
          Ttl = 128
          DontFragment = false
          Timeout = TimeSpan.FromMilliseconds(120.0) }

type PingExtensionError =
    | PingError of ex:Exception

  module internal PingExtensionStatisticsHelper =

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

    let createStatistics (pingReply: PingReply) =
        let table = pingReply |> createTable "Ping"

        let stats = new CustomStatistics()
        stats.Tables.Add(table)
        stats

type PingExtension (config: PingExtensionConfig) =
    let mutable _logger: ILogger = null
    let mutable _stats = new CustomStatistics()

    let ping (config: PingExtensionConfig) =
        try
            let pingOptions = PingOptions()
            pingOptions.Ttl <- config.Ttl
            pingOptions.DontFragment <- config.DontFragment

            let ping = new Ping()
            let buffer = Array.create config.BufferSizeBytes '-' |> Encoding.ASCII.GetBytes
            let reply = ping.Send(config.Host, config.Timeout.Milliseconds, buffer, pingOptions)

            Ok reply
        with
        | ex -> Error <| PingError ex

    let createStats (pingReplyResult: Result<PingReply, PingExtensionError>) = result {
        let! pingResult = pingReplyResult
        let stats = pingResult |> PingExtensionStatisticsHelper.createStatistics

        return stats
    }

    interface IExtension with
        member x.Init (logger, infraConfig) =
            _logger <- logger.ForContext<PingExtension>()
            ()

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

        member x.FinishTest (testInfo: TestInfo) =
            Task.FromResult(_stats)

let run () =
    let httpClient = new HttpClient()

    let step = Step.create("pull html", fun context -> task {
        let! response = httpClient.GetAsync("https://nbomber.com", context.CancellationToken)

        match response.IsSuccessStatusCode with
        | true  -> let bodySize = int response.Content.Headers.ContentLength.Value
                   let headersSize = response.Headers.ToString().Length
                   return Response.Ok(sizeBytes = headersSize + bodySize)

        | false -> return Response.Fail()
    })

    let scenario = Scenario.create "test_nbomber" [step]
                   |> Scenario.withLoadSimulations [
                       InjectScenariosPerSec(copiesCount = 150, during = TimeSpan.FromMinutes 1.0)
                   ]

    let pingExtensionConfig = PingExtensionConfig.Create("nbomber.com")
    let pingExtension = PingExtension(pingExtensionConfig)

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.loadInfraConfigYaml "infra_config.yaml"
    |> NBomberRunner.withExtensions([pingExtension])
    |> NBomberRunner.runInConsole
