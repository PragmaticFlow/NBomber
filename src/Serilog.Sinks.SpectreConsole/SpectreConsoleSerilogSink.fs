namespace Serilog.Sinks.SpectreConsole

open System
open System.IO
open System.Runtime.CompilerServices

open Serilog.Configuration
open Serilog.Core
open Serilog.Events
open Serilog.Formatting
open Spectre.Console

module internal LogEvent =

    let toString (textFormatter: ITextFormatter) (logEvent: LogEvent) =
        let writer = new StringWriter()
        textFormatter.Format(logEvent, writer)
        writer.Flush()
        writer.ToString()

type SpectreConsoleSink (textFormatter: ITextFormatter) =

    do
        if isNull textFormatter then
            nameof textFormatter |> ArgumentNullException |> raise   

    interface ILogEventSink with
        member _.Emit(logEvent: LogEvent) =
            if isNull logEvent then
                nameof logEvent |> ArgumentNullException |> raise

            logEvent
            |> LogEvent.toString textFormatter
            |> AnsiConsole.Write

[<Extension>]
type SpectreConsoleSinkLoggerConfigurationExtensions() =

    [<CompiledName("SpectreConsole")>]
    [<Extension>]
    static member spectreConsole (sinkConfiguration: LoggerSinkConfiguration, outputTemplate: string, minLevel: LogEventLevel) =
        let formatter = SpectreConsoleTextFormatter(outputTemplate)
        let sink = SpectreConsoleSink(formatter)
        sinkConfiguration.Sink(sink, minLevel)
