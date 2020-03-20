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
    let clientResponses = ClientResponses()
    let tsk1 = clientResponses.GetResponseAsync("id", CancellationToken.None)
    let tsk2 = clientResponses.GetResponseAsync("id", CancellationToken.None)
    let tsk3 = clientResponses.GetResponseAsync("new_id", CancellationToken.None)

    let set = Set.ofSeq [tsk1.Id; tsk2.Id; tsk3.Id]
    test <@ set.Count = 3 @>
