module FileLogger

open System.IO
open Xunit

open NBomber.Configuration
open NBomber.Infra
open NBomber.Infra.Dependency
open Serilog.Events
open Serilog

[<Fact>]
let ``With file name in LogSetting.LogToFile Logger should save to file`` () =

    let tempFilePath = Path.GetTempFileName()

    let logSetting = {
        MinimumLevel = Some LogEventLevel.Information
        LogToFile = tempFilePath
    }

    let information = "Information message for test"
    let verbose = "Verbose message for test"

    Dependency.Logger.initLogger(ApplicationType.Console, Some logSetting)

    Log.Logger.Information(information)
    Log.Logger.Verbose(verbose)
    Log.CloseAndFlush()

    let content = File.ReadAllText(tempFilePath)
    Assert.Contains(information, content)
    Assert.DoesNotContain(verbose, content)