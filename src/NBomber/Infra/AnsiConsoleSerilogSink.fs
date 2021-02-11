module internal NBomber.Infra.ConsoleSerilogSink

open System
open System.IO
open System.Runtime.CompilerServices

open Serilog.Configuration
open Serilog.Core
open Serilog.Events
open Serilog.Formatting
open Serilog.Formatting.Display
open Serilog.Parsing

module LogEvent =

    let toString (textFormatter: ITextFormatter) (logEvent: LogEvent) =
        use writer = new StringWriter()
        textFormatter.Format(logEvent, writer)
        writer.Flush()
        writer.ToString()

module LogEventPropertyValue =

    let toString (format: string) (value: LogEventPropertyValue) =
        use writer = new StringWriter()
        value.Render(writer, format)
        writer.Flush()
        writer.ToString()

module StructureValue =

    let toString (format: string) (value: StructureValue) =
        use writer = new StringWriter()
        value.Render(writer, format)
        writer.Flush()
        writer.ToString()

module MessageTemplate =

    let containsPropName (propName: string) (template: MessageTemplate) =
        template.Tokens
        |> Seq.exists(fun token ->
            (token :? PropertyToken) && (token :?> PropertyToken).PropertyName = propName
        )

type AnsiConsoleSink (textFormatter: ITextFormatter, lockObj: obj) =

    do
        if isNull textFormatter then
            nameof textFormatter |> ArgumentNullException |> raise   

    interface ILogEventSink with
        member _.Emit(logEvent: LogEvent) =
            if isNull logEvent then
                nameof logEvent |> ArgumentNullException |> raise

            lock lockObj (fun _ ->
                logEvent
                |> LogEvent.toString textFormatter
                |> AnsiConsole.write AnsiConsole.DefaultConsole
            )

type AnsiConsoleRenderer = (LogEvent * TextWriter) -> unit

module AnsiConsoleTextFormatter =

    let private textRenderer (token: TextToken) (logEvent: LogEvent, output: TextWriter) =
        output.Write(token.Text)

    let private propRenderer (token: PropertyToken) (format: string) (logEvent: LogEvent, output: TextWriter) =
        if logEvent.Properties.ContainsKey(token.PropertyName) then
            let console = AnsiConsole.create(output)

            logEvent.Properties.[token.PropertyName]
            |> LogEventPropertyValue.toString format
            |> AnsiConsole.highlight
            |> AnsiConsole.markup
            |> AnsiConsole.render console

    let private messageRenderer (token: PropertyToken) (logEvent: LogEvent, output: TextWriter) =
        let format = token.Format

        logEvent.MessageTemplate.Tokens
        |> Seq.iter(fun token ->
            if token :? TextToken then
                textRenderer (token :?> TextToken) (logEvent, output)
            else
                propRenderer (token :?> PropertyToken) format (logEvent, output)
        )
    
    let private timestampRenderer (token: PropertyToken) (logEvent: LogEvent, output: TextWriter) =
        let sv = ScalarValue(logEvent.Timestamp)
        sv.Render(output, token.Format)

    let private levelRenderer (token: PropertyToken) (logEvent: LogEvent, output: TextWriter) =
        //todo: implement format handling
        let convertLevelToString (format: string) (level: LogEventLevel) =
            match level with
            | LogEventLevel.Verbose     -> "VERB"
            | LogEventLevel.Debug       -> "DBUG"
            | LogEventLevel.Information -> "INFO"
            | LogEventLevel.Warning     -> "WARN"
            | LogEventLevel.Error       -> "EROR"
            | LogEventLevel.Fatal       -> "FATL"

        let console = AnsiConsole.create(output)
        let level = convertLevelToString token.Format logEvent.Level

        match logEvent.Level with
        | LogEventLevel.Verbose     -> level |> AnsiConsole.highlightVerbose
        | LogEventLevel.Debug       -> level |> AnsiConsole.highlightDebug
        | LogEventLevel.Information -> level |> AnsiConsole.highlightInfo
        | LogEventLevel.Warning     -> level |> AnsiConsole.highlightWarning
        | LogEventLevel.Error       -> level |> AnsiConsole.highlightError
        | LogEventLevel.Fatal       -> level |> AnsiConsole.highlightFatal
        |> AnsiConsole.markup
        |> AnsiConsole.render console

    let private newLineRenderer (logEvent: LogEvent, output: TextWriter) =
        output.WriteLine()
     
    let private exceptionRenderer (logEvent: LogEvent, output: TextWriter) =
        if not (isNull logEvent.Exception) then
            let console = AnsiConsole.create(output)
            AnsiConsole.writeException console logEvent.Exception

    let private propertiesRenderer (token: PropertyToken) (outputTemplate: MessageTemplate) (logEvent: LogEvent, output: TextWriter) =
        let shouldBeRendered (propName: string, logEvent: LogEvent, outputTemplate: MessageTemplate) =
            let containsPropName = MessageTemplate.containsPropName propName
            let result = not(containsPropName logEvent.MessageTemplate) && not(containsPropName outputTemplate)
            result

        let console = AnsiConsole.create(output)

        logEvent.Properties
        |> Seq.filter(fun x -> shouldBeRendered(x.Key, logEvent, outputTemplate))
        |> Seq.map(fun x -> LogEventProperty(x.Key, x.Value))
        |> StructureValue
        |> StructureValue.toString token.Format
        |> AnsiConsole.highlightMuted
        |> AnsiConsole.markup
        |> AnsiConsole.render console


    let private eventPropRenderer (token: PropertyToken) (logEvent: LogEvent, output: TextWriter) =
        propRenderer (token) token.Format (logEvent, output)

    let private createPropTokenRenderer (outputTemplate: MessageTemplate) (token: PropertyToken) =
        match token.PropertyName with
        | OutputProperties.MessagePropertyName       -> messageRenderer token
        | OutputProperties.TimestampPropertyName     -> timestampRenderer token
        | OutputProperties.LevelPropertyName         -> levelRenderer token
        | OutputProperties.NewLinePropertyName       -> newLineRenderer
        | OutputProperties.ExceptionPropertyName     -> exceptionRenderer
        | OutputProperties.PropertiesPropertyName    -> propertiesRenderer token outputTemplate
        | _                                          -> eventPropRenderer token

    let createRenderer (outputTemplate: MessageTemplate) (token: MessageTemplateToken) =
        if token :? TextToken then
            textRenderer(token :?> TextToken)
        else
            createPropTokenRenderer outputTemplate (token :?> PropertyToken)


type AnsiConsoleTextFormatter (outputTemplate: string) = 

    let mutable _renderers: AnsiConsoleRenderer[] = Array.empty<AnsiConsoleRenderer>

    do
        if isNull outputTemplate then
            nameof outputTemplate |> ArgumentNullException |> raise   

        let template = MessageTemplateParser().Parse(outputTemplate)
        _renderers <- template.Tokens |> Seq.map(AnsiConsoleTextFormatter.createRenderer template) |> Array.ofSeq

    interface ITextFormatter with
        member _.Format(logEvent: LogEvent, output: TextWriter) =
            if isNull logEvent then
                nameof logEvent |> ArgumentNullException |> raise   
            if isNull output then
                nameof output |> ArgumentNullException |> raise

            _renderers |> Seq.iter(fun renderer -> renderer(logEvent, output))

[<Extension>]
type AnsiConsoleLoggerConfigurationExtensions() =

    [<CompiledName("AnsiConsole")>]
    [<Extension>]
    static member ansiConsole (sinkConfiguration: LoggerSinkConfiguration, outputTemplate: string, minLevel: LogEventLevel) =
        let formatter = AnsiConsoleTextFormatter(outputTemplate)
        let lockObj = obj()
        let sink = AnsiConsoleSink(formatter, lockObj)
        sinkConfiguration.Sink(sink, minLevel)
