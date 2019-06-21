module Test.Extensions.String

open System
open System.Threading
open System.Threading.Tasks

open Xunit

open NBomber.Contracts
open NBomber.Extensions

[<Fact>]
let ``Extensions.String.concatWithCommaAndQuotes should concat strings with quotes`` () =
    Assert.Equal("'foo', 'bar', 'baz'", ["foo"; "bar"; "baz"] |> String.concatWithCommaAndQuotes)
    Assert.Equal("'foo'", ["foo"] |> String.concatWithCommaAndQuotes)
    Assert.Equal("", [] |> String.concatWithCommaAndQuotes)

[<Fact>]
let ``TimeoutAfter should work correctly`` () =
    let longRequest = Task.Delay(TimeSpan.FromSeconds(2.0))    
    let timeoutResponse = 
        longRequest.TimeoutAfter(TimeSpan.FromSeconds(1.0),
                                 CancellationToken.None, 
                                 fun () -> Response.Ok())

    let fastRequest = Task.Delay(TimeSpan.FromSeconds(0.5))
    let okResponse = 
        fastRequest.TimeoutAfter(TimeSpan.FromSeconds(5.0),
                                 CancellationToken.None, 
                                 fun () -> Response.Ok())

    Assert.False(timeoutResponse.Result.IsOk)
    Assert.True(okResponse.Result.IsOk)