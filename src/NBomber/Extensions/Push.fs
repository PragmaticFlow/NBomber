namespace NBomber.Extensions.Push

open System
open System.Collections.Generic
open System.Diagnostics
open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

open NBomber.Extensions.Internal

type PushResponse<'T> = {
    ClientId: string
    Payload: 'T
    ReceivedTime: DateTime
}

type ClientId = string

type internal CurrentTime() =

    let _timer = Stopwatch()
    let _initTime = DateTime.UtcNow

    do _timer.Start()

    member _.UtcNow = _initTime + _timer.Elapsed

type internal ActorMessage<'T> =
    | InitQueueForClient of awaiterTsc:TaskCompletionSource<unit> * ClientId
    | ReceivedPushResponse of PushResponse<'T>
    | SubscribeOnResponse of awaiterTsc:TaskCompletionSource<PushResponse<'T>> * ClientId
    | ClearQueue of awaiterTsc:TaskCompletionSource<unit>

type internal ActorState<'T> =
    { Clients: Dictionary<ClientId, TaskCompletionSource<PushResponse<'T>> option>
      ClientResponses: Dictionary<ClientId, Queue<PushResponse<'T>>> }

    static member init () =
        { Clients = Dictionary<ClientId, TaskCompletionSource<PushResponse<'T>> option>()
          ClientResponses = Dictionary<ClientId, Queue<PushResponse<'T>>>() }

    static member receive (state: ActorState<'T>) (msg: ActorMessage<'T>) =
        match msg with
        | InitQueueForClient (awaiterTsc, clientId) ->
            state.Clients[clientId] <- None
            state.ClientResponses[clientId] <- Queue<PushResponse<'T>>()
            awaiterTsc.TrySetResult() |> ignore

        | ReceivedPushResponse pushResponse ->
            let clientResponses = state.ClientResponses[pushResponse.ClientId]
            clientResponses.Enqueue(pushResponse)

            match state.Clients.TryGetValue(pushResponse.ClientId) with
            | true, Some awaiterTsc ->
                let latestResponse = clientResponses.Dequeue()
                awaiterTsc.TrySetResult(latestResponse) |> ignore
                state.Clients[pushResponse.ClientId] <- None

            | _ -> ()

        | SubscribeOnResponse (awaiterTsc, clientId) ->
            let clientResponses = state.ClientResponses[clientId]
            if clientResponses.Count > 0 then
                let response = clientResponses.Dequeue()
                awaiterTsc.TrySetResult(response) |> ignore
            else
                state.Clients[clientId] <- Some awaiterTsc

        | ClearQueue awaiterTsc ->
            state.Clients.Values
            |> Seq.iter (fun awaiterTcs -> awaiterTcs |> Option.iter (fun x -> x.TrySetCanceled() |> ignore))

            state.Clients.Clear()
            state.ClientResponses.Clear()
            awaiterTsc.TrySetResult() |> ignore

        state

type PushResponseQueue<'T>() =

    let mutable _state = ActorState.init()
    let _actor = ActionBlock(fun msg -> _state <- ActorState.receive _state msg)
    let _currentTime = CurrentTime()

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.InitQueueForClient(clientId: string) =
        let awaiterTsc = TaskCompletionSource<unit>()
        _actor.Post(InitQueueForClient(awaiterTsc, clientId)) |> ignore
        awaiterTsc.Task.Wait()

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.ReceiveResponse(clientId: string) =
        let awaiterTsc = TaskCompletionSource<PushResponse<'T>>()
        _actor.Post(SubscribeOnResponse(awaiterTsc, clientId)) |> ignore
        awaiterTsc.Task

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.AddResponse(clientId: string, payload: 'T) =
        let pushResponse = { ClientId = clientId; Payload = payload; ReceivedTime = _currentTime.UtcNow }
        _actor.Post(ReceivedPushResponse pushResponse) |> ignore

    interface IDisposable with
        member _.Dispose() =
            let awaiterTsc = TaskCompletionSource<unit>()
            _actor.Post(ClearQueue awaiterTsc) |> ignore
            awaiterTsc.Task.Wait()
