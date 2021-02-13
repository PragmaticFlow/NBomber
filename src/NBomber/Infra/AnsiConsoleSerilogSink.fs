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

module LogEventPropertyValue =

    let toString (format: string) (value: LogEventPropertyValue) =
        use writer = new StringWriter()
        value.Render(writer, format)
        writer.Flush()
        writer.ToString()

module StructureValue =

    let toString (format: string) (value: StructureValue) =
        LogEventPropertyValue.toString format value

module MessageTemplate =

    let containsPropName (propName: string) (template: MessageTemplate) =
        template.Tokens
        |> Seq.exists(fun token ->
            (token :? PropertyToken) && (token :?> PropertyToken).PropertyName = propName
        )

module LevelOutputFormat =

    let private lowerCaseFormat = "w"
    let private titleCaseFormat = "t"
    let private upperCaseFormat = "u"

    let private verboseLevelMonikers =
        [ lowerCaseFormat, ["v"; "vb"; "vrb"; "verb"; "verbo"; "verbos"; "verbose"]
          titleCaseFormat, ["V"; "Vb"; "Vrb"; "Verb"; "Verbo"; "Verbos"; "Verbose"]
          upperCaseFormat, ["V"; "VB"; "VRB"; "VERB"; "VERBO"; "VERBOS"; "VERBOSE"] ]
        |> Map.ofSeq

    let private debugLevelMonikers =
        [ lowerCaseFormat, ["d"; "de"; "dbg"; "dbug"; "debug"]
          titleCaseFormat, ["D"; "De"; "Dbg"; "Dbug"; "Debug"]
          upperCaseFormat, ["D"; "DE"; "DBG"; "DBUG"; "DEBUG"] ]
        |> Map.ofSeq

    let private informationLevelMonikers =
        [ lowerCaseFormat, ["i"; "in"; "inf"; "info"; "infor"; "inform"; "informa"; "informat"; "informati"; "informatio"; "information"]
          titleCaseFormat, ["I"; "In"; "Inf"; "Info"; "Infor"; "Inform"; "Informa"; "Informat"; "Informati"; "Informatio"; "Information"]
          upperCaseFormat, ["I"; "IN"; "INF"; "INFO"; "INFOR"; "INFORM"; "INFORMA"; "INFORMAT"; "INFORMATI"; "INFORMATIO"; "INFORMATION"] ]
        |> Map.ofSeq

    let private warningLevelMonikers =
        [ lowerCaseFormat, ["w"; "wn"; "wrn"; "warn"; "warni"; "warnin"; "warning"]
          titleCaseFormat, ["W"; "Wn"; "Wrn"; "Warn"; "Warni"; "Warnin"; "Warning"]
          upperCaseFormat, ["W"; "WN"; "WRN"; "WARN"; "WARNI"; "WARNIN"; "WARNING"] ]
        |> Map.ofSeq

    let private errorLevelMonikers =
        [ lowerCaseFormat, ["e"; "er"; "err"; "eror"; "error"]
          titleCaseFormat, ["E"; "Er"; "Err"; "Eror"; "Error"]
          upperCaseFormat, ["E"; "ER"; "ERR"; "EROR"; "ERROR"] ]
        |> Map.ofSeq

    let private fatalLevelMonikers =
        [ lowerCaseFormat, ["f"; "fa"; "ftl"; "fatl"; "fatal"]
          titleCaseFormat, ["F"; "Fa"; "Ftl"; "Fatl"; "Fatal"]
          upperCaseFormat, ["F"; "FA"; "FTL"; "FATL"; "FATAL"] ]
        |> Map.ofSeq

    let private levelMonikers =
        [ LogEventLevel.Verbose, verboseLevelMonikers
          LogEventLevel.Debug, debugLevelMonikers
          LogEventLevel.Information, informationLevelMonikers
          LogEventLevel.Warning, warningLevelMonikers
          LogEventLevel.Error, errorLevelMonikers
          LogEventLevel.Fatal, fatalLevelMonikers ]
        |> Map.ofSeq

    let private getCaseFormat (format: string, defaultValue: string) =
        if (isNull format) || (format.Length <> 2 && format.Length <> 3) then
            defaultValue
        else
            let caseFormat = format.[0].ToString()
            if caseFormat = lowerCaseFormat || caseFormat = titleCaseFormat || caseFormat = upperCaseFormat then
                caseFormat
            else
                defaultValue

    let private getMonikerWidth (format: string, defaultValue: int) =
        if isNull format then
            defaultValue
        else if format.Length = 2 || format.Length = 3 then
            let (parsed, width) = Int32.TryParse(format.Substring(1))
            if parsed && width > 0 then
                width
            else
                defaultValue
        else
            defaultValue
            
    let getLevelMoniker (format: string) (level: LogEventLevel) =
        let caseFormat = getCaseFormat(format, titleCaseFormat)
        let monikerWidth = getMonikerWidth(format, 3)
        let monikers = levelMonikers.[level].[caseFormat]
        let index = Math.Min(monikerWidth, monikers.Length) - 1
        let moniker = monikers.[index]
        moniker
            
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
        let console = AnsiConsole.create(output)
        let levelMoniker = LevelOutputFormat.getLevelMoniker token.Format logEvent.Level

        match logEvent.Level with
        | LogEventLevel.Verbose     -> levelMoniker |> AnsiConsole.highlightVerbose
        | LogEventLevel.Debug       -> levelMoniker |> AnsiConsole.highlightDebug
        | LogEventLevel.Information -> levelMoniker |> AnsiConsole.highlightInfo
        | LogEventLevel.Warning     -> levelMoniker |> AnsiConsole.highlightWarning
        | LogEventLevel.Error       -> levelMoniker |> AnsiConsole.highlightError
        | LogEventLevel.Fatal       -> levelMoniker |> AnsiConsole.highlightFatal
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

type AnsiConsoleRenderer = (LogEvent * TextWriter) -> unit
            
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

type AnsiConsoleSink (textFormatter: ITextFormatter, lockObj: obj) =

    do
        if isNull textFormatter then
            nameof textFormatter |> ArgumentNullException |> raise   

    interface ILogEventSink with
        member _.Emit(logEvent: LogEvent) =
            if isNull logEvent then
                nameof logEvent |> ArgumentNullException |> raise

            lock lockObj (fun _ ->
                let output = Console.Out
                textFormatter.Format(logEvent, output)
                output.Flush()
            )

[<Extension>]
type AnsiConsoleLoggerConfigurationExtensions() =

    [<CompiledName("AnsiConsole")>]
    [<Extension>]
    static member ansiConsole (sinkConfiguration: LoggerSinkConfiguration, outputTemplate: string, minLevel: LogEventLevel) =
        let formatter = AnsiConsoleTextFormatter(outputTemplate)
        let lockObj = obj()
        let sink = AnsiConsoleSink(formatter, lockObj)
        sinkConfiguration.Sink(sink, minLevel)
