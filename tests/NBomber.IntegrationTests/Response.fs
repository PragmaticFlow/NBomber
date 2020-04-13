module Tests.Response

open Swensen.Unquote
open FsCheck.Xunit

open NBomber.Contracts

[<Property>]
let ``Ok(payload: byte[]) should calculate SizeBytes automatically`` (payload: byte[]) =
    let response = Response.Ok(payload)

    let actual = {| Size = response.SizeBytes |}

    if isNull payload then
        test <@ 0 = actual.Size @>
    else
        test <@ payload.Length = actual.Size @>
