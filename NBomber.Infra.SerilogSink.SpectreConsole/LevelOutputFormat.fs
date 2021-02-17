module internal NBomber.Infra.SerilogSink.SpectreConsole.LevelOutputFormat

open Serilog.Events
open System

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
    if levelMonikers.ContainsKey(level) then
        let caseFormat = getCaseFormat(format, titleCaseFormat)
        let monikerWidth = getMonikerWidth(format, 3)
        let monikers = levelMonikers.[level].[caseFormat]
        let index = Math.Min(monikerWidth, monikers.Length) - 1
        let moniker = monikers.[index]
        moniker
    else
        level.ToString()
