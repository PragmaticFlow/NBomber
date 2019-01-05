module Tests.Result

open NBomber.Contracts

open Xunit
open FsCheck.Xunit

[<Property>]
let ``Response.Ok() should calculate SizeBytes when passing array of bytes in`` (payload: byte[]) =
    let actual = Response.Ok(payload)
    if isNull(payload) then Assert.Equal(0, actual.SizeBytes)
    else Assert.Equal(Array.length payload, actual.SizeBytes)