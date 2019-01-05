module Tests.Result

open NBomber.Contracts

open Xunit
open FsCheck.Xunit

[<Property>]
let ``SizeBytes in Response object should equal size of incoming payload`` (payload: byte[]) =
    let actual = Response.Ok(payload)
    if payload = null then Assert.Equal(0, actual.SizeBytes)
    else Assert.Equal(Array.length payload, actual.SizeBytes)