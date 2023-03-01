module Tests.Extensions

open System
open System.Diagnostics
open System.Threading

open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Extensions.Internal

[<Fact>]
let ``String concatWithComma should concat strings with comma`` () =
    Assert.Equal("foo, bar, baz", ["foo"; "bar"; "baz"] |> String.concatWithComma)
    Assert.Equal("foo", ["foo"] |> String.concatWithComma)
    Assert.Equal("", [] |> String.concatWithComma)