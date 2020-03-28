module Tests.Extensions

open System.Threading

open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit

open NBomber.Extensions

[<Fact>]
let ``Extensions.String.concatWithCommaAndQuotes should concat strings with quotes`` () =
    Assert.Equal("'foo', 'bar', 'baz'", ["foo"; "bar"; "baz"] |> String.concatWithCommaAndQuotes)
    Assert.Equal("'foo'", ["foo"] |> String.concatWithCommaAndQuotes)
    Assert.Equal("", [] |> String.concatWithCommaAndQuotes)

[<Fact>]
let ``ClientResponses.GetResponseAsync should return always a new task`` () =
    let buffer = PushResponseBuffer()
    let tsk1 = buffer.WaitOnPushResponse("id")
    let tsk2 = buffer.WaitOnPushResponse("id")
    let tsk3 = buffer.WaitOnPushResponse("new_id")

    let set = Set.ofSeq [tsk1.Id; tsk2.Id; tsk3.Id]
    test <@ set.Count = 3 @>
