namespace NBomber.Extensions

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open FSharp.Control.Tasks.V2.ContextInsensitive
open Newtonsoft.Json

open NBomber.Contracts

[<Extension>]
type StringExtensions() =
    
    [<Extension>]
    static member DeserializeObject<'T>(json: string) =
        JsonConvert.DeserializeObject<'T>(json)

[<Extension>]
type TaskExtensions() =

    [<Extension>]
    static member TimeoutAfter(request: Task<'T>, 
                               duration: TimeSpan, 
                               cancellationToken: CancellationToken,
                               response: 'T -> Response) = task {
        let! completedTask = Task.WhenAny(request, Task.Delay(duration, cancellationToken))
        match completedTask.Equals(request) with
        | true  -> return response(request.Result)
        | false -> return Response.Fail()
    }

    [<Extension>]
    static member TimeoutAfter(request: Task, 
                               duration: TimeSpan, 
                               cancellationToken: CancellationToken,
                               response: unit -> Response) = task {
        let! completedTask = Task.WhenAny(request, Task.Delay(duration, cancellationToken))
        match completedTask.Equals(request) with
        | true  -> return response()
        | false -> return Response.Fail()
    }
    
type ClientResponses() =

    let responses = ConcurrentDictionary<string, TaskCompletionSource<byte[]>>()

    member x.InitClientId(clientId: string) =
        responses.TryAdd(clientId, TaskCompletionSource<byte[]>())
        |> ignore

    member x.SetResponse(clientId: string, payload: byte[]) =
        responses.[clientId].TrySetResult(payload) |> ignore
        x.InitClientId(clientId)
        
    member x.GetResponse(clientId: string) =
        match responses.ContainsKey(clientId) with
        | true  -> responses.[clientId].Task
        
        | false -> x.InitClientId(clientId)
                   responses.[clientId].Task