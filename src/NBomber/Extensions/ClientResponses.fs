namespace NBomber.Extensions

open System.Collections.Concurrent
open System.Threading.Tasks

type ClientResponses() =

    let responses = ConcurrentDictionary<string, TaskCompletionSource<byte[]>>()

    member x.InitClientId(clientId: string) =
        responses.[clientId] = TaskCompletionSource<byte[]>()

    member x.SetResponse(clientId: string, payload: byte[]) =
        responses.[clientId].SetResult(payload)
        x.InitClientId(clientId)
        
    member x.GetResponse(clientId: string) =
        responses.[clientId].Task