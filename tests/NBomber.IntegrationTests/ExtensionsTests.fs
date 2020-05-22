module Tests.Extensions

open System.Threading

open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit

open NBomber.Extensions

[<Fact>]
let ``String concatWithCommaAndQuotes should concat strings with quotes`` () =
    Assert.Equal("'foo', 'bar', 'baz'", ["foo"; "bar"; "baz"] |> String.concatWithCommaAndQuotes)
    Assert.Equal("'foo'", ["foo"] |> String.concatWithCommaAndQuotes)
    Assert.Equal("", [] |> String.concatWithCommaAndQuotes)
