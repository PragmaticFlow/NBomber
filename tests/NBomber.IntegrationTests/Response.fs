module Tests.Response

open Swensen.Unquote
open FsCheck.Xunit

open NBomber.Contracts

[<Property>]
let ``Response.Ok(payload: byte[]) should calculate SizeBytes automatically`` (payload: byte[]) =
    let actual = Response.Ok(payload)
    if isNull payload then test <@ 0 = actual.SizeBytes @>
    else test <@ payload.Length = actual.SizeBytes @>
