module Tests.PushExtensions

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Extensions.Push

[<Fact>]
let ``PushResponseQueue ReceiveResponse should return always a new task`` () =
    use responseQueue = new PushResponseQueue<_>()

    responseQueue.InitQueueForClient("id")
    responseQueue.InitQueueForClient("new_id")

    let tsk1 = responseQueue.ReceiveResponse("id")
    let tsk2 = responseQueue.ReceiveResponse("id")
    let tsk3 = responseQueue.ReceiveResponse("new_id")

    let set = Set.ofSeq [tsk1.Id; tsk2.Id; tsk3.Id]
    test <@ set.Count = 3 @>

[<Fact>]
let ``PushResponseQueue should handle waiting on response correctly`` () =
    let clientId = "clientId"
    let payload = "payload"

    use responseQueue = new PushResponseQueue<string>()
    responseQueue.InitQueueForClient(clientId)

    let task = responseQueue.ReceiveResponse(clientId)
    Task.Delay(seconds 1).Wait()

    if not task.IsCompleted then
        responseQueue.AddResponse(clientId, payload)

    let response = string task.Result.Payload
    test <@ response = payload @>

[<Fact>]
let ``PushResponseQueue should queue responses (in order) if no client is found`` () =
    let clientId = "clientId"

    use responseQueue = new PushResponseQueue<int>()
    responseQueue.InitQueueForClient(clientId)

    // register the time when requests were sent
    let addingTime = DateTime.UtcNow

    // adding responses that should be buffered
    [0..5] |> Seq.iter (fun payload -> responseQueue.AddResponse(clientId, payload))
    Task.Delay(seconds 1).Wait()

    [0..5]
    |> Seq.map (fun _ -> responseQueue.ReceiveResponse(clientId))
    |> Task.WhenAll
    |> fun allResponses ->

        [0..5]
        |> Seq.iter (fun i ->
            let payload = allResponses.Result[i].Payload
            let receivedTime = allResponses.Result[i].ReceivedTime
            let shiftedTime = addingTime.AddMilliseconds(100)
            test <@ payload = i @>
            test <@ receivedTime < shiftedTime @>
        )

[<Fact>]
let ``PushResponseQueue should handle concurrency without deadlocks`` () =

    use responseQueue = new PushResponseQueue<string>()

    // we create only 5 clients
    [0..4]
    |> Seq.iter (fun clientId -> responseQueue.InitQueueForClient(string clientId))

    // and create 300 threads that concurrently add/receive
    Parallel.For(1, 300, fun clientId ->
        let id = string (clientId % 4)
        responseQueue.AddResponse(id, id)
        responseQueue.AddResponse(id, id)

        let response1 = responseQueue.ReceiveResponse(id).Result
        let response2 = responseQueue.ReceiveResponse(id).Result

        test <@ id = string response1.Payload @>
        test <@ id = string response2.Payload @>
    )
    |> ignore

