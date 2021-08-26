namespace NBomber.Extensions.PushExtensions

open System
open System.Collections.Generic
open System.Threading.Tasks
open NBomber.Extensions

type PushResponse = {
    ClientId: string
    Payload: obj
    ReceivedTime: DateTime
}

type ClientId = string

type PushResponseBuffer() =

    let _lockObj = obj()
    let _awaiters = Dictionary<ClientId, TaskCompletionSource<PushResponse>>()
    let _awaitersBuffer = Dictionary<ClientId, Queue<PushResponse>>()
    let _currentTime = CurrentTime()

    let initBufferForClient (clientId) =
        _awaiters.[clientId] <- TaskCompletionSource<PushResponse>()
        _awaitersBuffer.[clientId] <- Queue<PushResponse>()

    let destroyBuffer () =
        _awaiters.Values |> Seq.iter(fun awaiterTcs -> awaiterTcs.TrySetCanceled() |> ignore)
        _awaiters.Clear()
        _awaitersBuffer.Clear()

    member _.InitBufferForClient(clientId: string) = initBufferForClient(clientId)

    member _.ReceiveResponse(clientId: string) =
        lock _lockObj (fun () ->
            let awaiterTsc = TaskCompletionSource<PushResponse>(TaskCreationOptions.RunContinuationsAsynchronously)
            let missedResponses = _awaitersBuffer.[clientId]
            if missedResponses.Count > 0 then
                let response = missedResponses.Dequeue()
                awaiterTsc.SetResult(response)
            else
                _awaiters.[clientId] <- awaiterTsc

            awaiterTsc.Task
        )

    member _.AddResponse(clientId: string, payload: obj) =
        lock _lockObj (fun () ->
            let pushResponse = {
                ClientId = clientId
                Payload = payload
                ReceivedTime = _currentTime.UtcNow
            }

            let missedResponses = _awaitersBuffer.[pushResponse.ClientId]
            missedResponses.Enqueue(pushResponse)

            if _awaiters.ContainsKey(pushResponse.ClientId) then
                let awaiterTsc = _awaiters.[pushResponse.ClientId]
                _awaiters.Remove(pushResponse.ClientId) |> ignore
                let latestResponse = missedResponses.Dequeue()
                awaiterTsc.SetResult(latestResponse)
        )

    member _.Destroy() = destroyBuffer()

    interface IDisposable with
        member _.Dispose() =
            destroyBuffer()
