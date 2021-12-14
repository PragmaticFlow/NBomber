namespace NBomber.Extensions.PushExtensions

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading.Tasks
open NBomber.Extensions.InternalExtensions

type PushResponse = {
    ClientId: string
    Payload: obj
    ReceivedTime: DateTime
}

type ClientId = string

type internal CurrentTime() =

    let _timer = Stopwatch()
    let _initTime = DateTime.UtcNow

    do _timer.Start()

    member _.UtcNow = _initTime + _timer.Elapsed

type internal ActorMessage =
    | InitQueueForClient of awaiterTsc:TaskCompletionSource<unit> * ClientId
    | ReceivedPushResponse of PushResponse
    | SubscribeOnResponse of awaiterTsc:TaskCompletionSource<PushResponse> * ClientId
    | ClearQueue of awaiterTsc:TaskCompletionSource<unit>

type internal ActorState =
    { Clients: Dictionary<ClientId, TaskCompletionSource<PushResponse> option>
      ClientResponses: Dictionary<ClientId, Queue<PushResponse>> }

    static member init () =
        { Clients = Dictionary<ClientId, TaskCompletionSource<PushResponse> option>()
          ClientResponses = Dictionary<ClientId, Queue<PushResponse>>() }

    static member handler (state: ActorState) (msg: ActorMessage) =
        match msg with
        | InitQueueForClient (awaiterTsc, clientId) ->
            state.Clients[clientId] <- None
            state.ClientResponses[clientId] <- Queue<PushResponse>()
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
            let clientResponses = state.ClientResponses.[clientId]
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

type PushResponseQueue() =

    let _actor = Actor.FastActor(initialState = ActorState.init(), handler = ActorState.handler)
    let _currentTime = CurrentTime()

    member _.InitQueueForClient(clientId: string) =
        let awaiterTsc = TaskCompletionSource<unit>()
        _actor.Publish(InitQueueForClient(awaiterTsc, clientId))
        awaiterTsc.Task.Wait()

    member _.ReceiveResponse(clientId: string) =
        let awaiterTsc = TaskCompletionSource<PushResponse>()
        _actor.Publish(SubscribeOnResponse(awaiterTsc, clientId))
        awaiterTsc.Task

    member _.AddResponse(clientId: string, payload: obj) =
        let pushResponse = { ClientId = clientId; Payload = payload; ReceivedTime = _currentTime.UtcNow }
        _actor.Publish(ReceivedPushResponse pushResponse)

    interface IDisposable with
        member _.Dispose() =
            let awaiterTsc = TaskCompletionSource<unit>()
            _actor.Publish(ClearQueue awaiterTsc)
            awaiterTsc.Task.Wait()
