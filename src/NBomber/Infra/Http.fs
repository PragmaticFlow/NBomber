﻿module internal NBomber.Infra.Http

open System
open System.Net
open System.Net.Http
open System.IO
open System.Runtime.Serialization.Formatters.Binary

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

type ReqMsg = byte[]
type ResMsg = byte[]

let private httpClient = new HttpClient()

let createUrl (host: string, port: int, clusterId: string) =
    sprintf "http://%s:%i/nbomber/v1/%s/" host port clusterId

let serializeBinary (data: obj) =
    use ms = new MemoryStream()
    let formatter = BinaryFormatter()
    formatter.Serialize(ms, data)
    ms.ToArray()
    
let deserializeBinary<'T>(data: byte[]) = 
    use ms = new MemoryStream(data)
    let formatter = BinaryFormatter()
    formatter.Deserialize(ms) :?> 'T 

let createHandler (handler: 'TCommand -> 'TResponse) (request: byte[]) = 
    let command = deserializeBinary<'TCommand>(request)
    let response = handler(command)
    serializeBinary(response)   

let sendMsg<'TRequest,'TResponse>(url: Uri, msg: 'TRequest) = task {    
    let httpMsg = new HttpRequestMessage(HttpMethod.Post, url)
    let binaryReq = serializeBinary(msg)
    httpMsg.Content <- new ByteArrayContent(binaryReq)
    let! httpResponse = httpClient.SendAsync(httpMsg)
    let! binaryRes = httpResponse.Content.ReadAsByteArrayAsync()
    return deserializeBinary<'TResponse>(binaryRes)
}

type HttpServer(listenUrl: string, handler: ReqMsg -> ResMsg) =    
    
    let mutable stop = false
    let listener = new HttpListener()     
    do listener.Prefixes.Add(listenUrl)

    let start () = task {    
        listener.Start()        

        while not stop do            
            try 
                let! context = listener.GetContextAsync()            
                let request  = context.Request
                let response = context.Response 
                
                use ms = new MemoryStream()
                do! request.InputStream.CopyToAsync(ms)
                let reqMsg = ms.ToArray()
                let resMsg = handler(reqMsg) 
                response.ContentLength64 <- resMsg.LongLength                               
                
                use responseStream = response.OutputStream
                do! responseStream.WriteAsync(resMsg, 0, resMsg.Length)                
            with
            | ex -> Log.Error(ex, "HttpServer error")
        
        listener.Stop()        
    }

    member x.Start() = stop <- false
                       start()

    member x.Stop()  = stop <- true
                       listener.Close()