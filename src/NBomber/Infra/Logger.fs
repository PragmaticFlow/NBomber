module internal NBomber.Infra.Logger

open Serilog
open Microsoft.Extensions.Configuration

let createLogger (sessionId: string, testName: string, configPath: IConfiguration option) =
    match configPath with
    | Some path ->            
        LoggerConfiguration()
            .Enrich.WithProperty("SessionId", sessionId)
            .Enrich.WithProperty("TestName", testName)
            .ReadFrom.Configuration(path)            
            .CreateLogger()
    | None ->
        LoggerConfiguration()
            .WriteTo.Console()
            .Enrich.WithProperty("SessionId", sessionId)
            .Enrich.WithProperty("TestName", testName)
            .CreateLogger()