module internal NBomber.Infra.Logger

open Serilog
open Microsoft.Extensions.Configuration
open NBomber.Contracts

let createLogger (testInfo: TestInfo, configPath: IConfiguration option) =
    let loggerConfig = LoggerConfiguration()
                        .Enrich.WithProperty("SessionId", testInfo.SessionId)
                        .Enrich.WithProperty("TestSuite", testInfo.TestSuite)
                        .Enrich.WithProperty("TestName", testInfo.TestName)                        
    match configPath with
    | Some path -> loggerConfig.ReadFrom.Configuration(path).CreateLogger()
    | None      -> loggerConfig.WriteTo.Console().CreateLogger()