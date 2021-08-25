module Tests.PushExtensions

open Xunit
open Swensen.Unquote
open NBomber.Extensions.PushExtensions

[<Fact>]
let ``PushResponseBuffer WaitResponse should return always a new task`` () =
    use buffer = new PushResponseBuffer()

    buffer.InitBufferForClient("id")
    buffer.InitBufferForClient("new_id")

    let tsk1 = buffer.ReceiveResponse("id")
    let tsk2 = buffer.ReceiveResponse("id")
    let tsk3 = buffer.ReceiveResponse("new_id")

    let set = Set.ofSeq [tsk1.Id; tsk2.Id; tsk3.Id]
    test <@ set.Count = 3 @>
