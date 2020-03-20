namespace NBomber.Extensions

open System.Collections.Concurrent
open System.Runtime.CompilerServices
open System.Threading.Tasks

open System.Threading
open System.Threading.Tasks
open Newtonsoft.Json

[<Extension>]
type StringExtensions() =

    [<Extension>]
    static member DeserializeJson<'T>(json: string) =
        JsonConvert.DeserializeObject<'T>(json)

type ClientResponses() =

    let responses = ConcurrentDictionary<string, TaskCompletionSource<byte[]>>()

    let initResponseTask(clientId: string) =
        responses.[clientId] <- TaskCompletionSource<byte[]>()

    member x.SetResponse(clientId: string, payload: byte[]) =
        responses.[clientId].TrySetResult(payload) |> ignore

    member x.GetResponseAsync(clientId: string, cancellationToken: CancellationToken) =

        let autoReInitTask (tsk: Task<byte[]>) =
            tsk.ContinueWith((fun (t: Task<byte[]>) -> initResponseTask(clientId)
                                                       t.Result)
                             , cancellationToken)

        match responses.ContainsKey(clientId) with
        | true  ->
            responses.[clientId].Task |> autoReInitTask

        | false ->
            initResponseTask(clientId)
            responses.[clientId].Task |> autoReInitTask
