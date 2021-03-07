namespace Serilog.Sinks.SpectreConsole

open System
open System.IO

open Serilog.Events
open Serilog.Formatting
open Serilog.Formatting.Display
open Serilog.Parsing

module internal LogEventPropertyValue =

    let toString (value: LogEventPropertyValue) =
        use writer = new StringWriter()
        if value :? ScalarValue && (value :?> ScalarValue).Value :? string then
            writer.Write((value :?> ScalarValue).Value)
        else
            value.Render(writer)
        writer.Flush()
        writer.ToString()

module internal StructureValue =

    let toString (value: StructureValue) =
        LogEventPropertyValue.toString(value)

module internal MessageTemplate =

    let containsPropName (propName: string) (template: MessageTemplate) =
        template.Tokens
        |> Seq.exists(fun token ->
            (token :? PropertyToken) && (token :?> PropertyToken).PropertyName = propName
        )
            
module internal SpectreConsoleTextFormatter =

    let private textRenderer (token: TextToken) (logEvent: LogEvent, output: TextWriter) =
        output.Write(token.Text)

    let private propRenderer (token: PropertyToken) (logEvent: LogEvent, output: TextWriter) =
        if logEvent.Properties.ContainsKey(token.PropertyName) then
            logEvent.Properties.[token.PropertyName]
            |> LogEventPropertyValue.toString
            |> SpectreConsole.escapeMarkup
            |> SpectreConsole.highlightProp
            |> SpectreConsole.render output

    let private messageRenderer (token: PropertyToken) (logEvent: LogEvent, output: TextWriter) =
        let format = token.Format

        logEvent.MessageTemplate.Tokens
        |> Seq.iter(fun token ->
            if token :? TextToken then
                textRenderer (token :?> TextToken) (logEvent, output)
            else
                propRenderer (token :?> PropertyToken) (logEvent, output)
        )
    
    let private timestampRenderer (token: PropertyToken) (logEvent: LogEvent, output: TextWriter) =
        let sv = ScalarValue(logEvent.Timestamp)
        sv.Render(output, token.Format)

    let private levelRenderer (token: PropertyToken) (logEvent: LogEvent, output: TextWriter) =
        let levelMoniker = LevelOutputFormat.getLevelMoniker token.Format logEvent.Level

        match logEvent.Level with
        | LogEventLevel.Verbose     -> levelMoniker |> SpectreConsole.highlightVerbose
        | LogEventLevel.Debug       -> levelMoniker |> SpectreConsole.highlightDebug
        | LogEventLevel.Information -> levelMoniker |> SpectreConsole.highlightInfo
        | LogEventLevel.Warning     -> levelMoniker |> SpectreConsole.highlightWarning
        | LogEventLevel.Error       -> levelMoniker |> SpectreConsole.highlightError
        | LogEventLevel.Fatal       -> levelMoniker |> SpectreConsole.highlightFatal
        | _                         -> levelMoniker
        |> SpectreConsole.render output

    let private newLineRenderer (logEvent: LogEvent, output: TextWriter) =
        output.WriteLine()
     
    let private exceptionRenderer (logEvent: LogEvent, output: TextWriter) =
        if not (isNull logEvent.Exception) then
            logEvent.Exception |> SpectreConsole.writeException output

    let private propertiesRenderer (token: PropertyToken) (outputTemplate: MessageTemplate) (logEvent: LogEvent, output: TextWriter) =
        let shouldBeRendered (propName: string, logEvent: LogEvent, outputTemplate: MessageTemplate) =
            let containsPropName = MessageTemplate.containsPropName propName
            let result = not(containsPropName logEvent.MessageTemplate) && not(containsPropName outputTemplate)
            result

        logEvent.Properties
        |> Seq.filter(fun x -> shouldBeRendered(x.Key, logEvent, outputTemplate))
        |> Seq.map(fun x -> LogEventProperty(x.Key, x.Value))
        |> StructureValue
        |> StructureValue.toString
        |> SpectreConsole.escapeMarkup
        |> SpectreConsole.highlightMuted
        |> SpectreConsole.render output

    let private eventPropRenderer (token: PropertyToken) (logEvent: LogEvent, output: TextWriter) =
        propRenderer (token) (logEvent, output)

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

type SpectreConsoleRenderer = (LogEvent * TextWriter) -> unit
            
type SpectreConsoleTextFormatter (outputTemplate: string) = 

    let mutable _renderers: SpectreConsoleRenderer[] = Array.empty<SpectreConsoleRenderer>

    do
        if isNull outputTemplate then
            nameof outputTemplate |> ArgumentNullException |> raise   

        let template = MessageTemplateParser().Parse(outputTemplate)
        _renderers <- template.Tokens |> Seq.map(SpectreConsoleTextFormatter.createRenderer template) |> Array.ofSeq

    interface ITextFormatter with
        member _.Format(logEvent: LogEvent, output: TextWriter) =
            if isNull logEvent then
                nameof logEvent |> ArgumentNullException |> raise   
            if isNull output then
                nameof output |> ArgumentNullException |> raise

            _renderers |> Seq.iter(fun renderer -> renderer(logEvent, output))
