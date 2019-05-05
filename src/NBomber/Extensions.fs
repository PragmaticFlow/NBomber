namespace NBomber.Extensions

open System.Threading.Tasks
open FsToolkit.ErrorHandling

[<AutoOpen>]
module internal Extensions =

    type FastCancellationToken = { mutable ShouldCancel: bool }

    type Task<'T> with
        static member map f (m: Task<_>) =
            m.ContinueWith(fun (t: Task<_>) -> f t.Result)

    type Result<'T,'TError> with
        static member isOk  result =
            match result with
            | Ok _    -> true
            | Error _ -> false

        static member isError result = result |> Result.isOk |> not

        static member getOk result =
            match result with
            | Ok v    -> v
            | Error _ -> failwith "result is error"

        static member getError result =
            match result with
            | Ok _     -> failwith "result is not error"
            | Error er -> er

        static member sequence (results: Result<'a,'e>[]) =
            let folder state (acc: Result<'a [],'e []>) =
                match state, acc with
                | Ok v, Ok items     -> Ok(Array.append items [| v |])
                | Ok r, Error ers    -> Error ers
                | Error e, Ok items  -> Error [|e|]
                | Error e, Error ers -> Error(Array.append ers [| e |])

            Seq.foldBack folder results (Ok Array.empty)

    type MaybeBuilder() =

        member x.Bind(m, bind) =
            match m with
            | Some value -> bind value
            | None       -> None

        member x.Return value = Some value
        member x.ReturnFrom value = value

    let maybe = MaybeBuilder()

    module String =

        let replace (oldValue: string, newValue: string) (str: string) =
            str.Replace(oldValue, newValue)

        let concatWithCommaAndQuotes (strings: string seq) =
            strings
            |> Seq.map (sprintf "'%s'")
            |> String.concat ", "


namespace NBomber.Extensions.Operator

module internal Result =

    let (>=>) f1 f2 arg =
        match f1 arg with
        | Ok data -> f2 data
        | Error e -> Error e
