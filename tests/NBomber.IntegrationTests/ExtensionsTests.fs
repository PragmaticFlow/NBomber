module Tests.Extensions

open System.Threading

open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit

open NBomber.Extensions.InternalExtensions

[<Fact>]
let ``String concatWithComma should concat strings with comma`` () =
    Assert.Equal("foo, bar, baz", ["foo"; "bar"; "baz"] |> String.concatWithComma)
    Assert.Equal("foo", ["foo"] |> String.concatWithComma)
    Assert.Equal("", [] |> String.concatWithComma)
