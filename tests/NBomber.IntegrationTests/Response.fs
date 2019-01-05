module Tests.Response

open NBomber.Contracts

open Xunit
open FsCheck.Xunit

[<Property>]
let ``Response.Ok(payload: byte[]) should calculate SizeBytes automatically`` (payload: byte[]) =
    let actual = Response.Ok(payload)
    if isNull(payload) then Assert.Equal(0, actual.SizeBytes)
    else Assert.Equal(Array.length(payload), actual.SizeBytes)