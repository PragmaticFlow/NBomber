namespace NBomber.Extensions

open System
open System.Threading.Tasks
open FsToolkit.ErrorHandling

[<AutoOpen>]
module internal Extensions =

    let inline isNotNull (value) =
        not(isNull value)

    module Result =

        let getOk (result) =
            match result with
            | Ok v    -> v
            | Error _ -> failwith "result is error"

        let getError (result) =
            match result with
            | Ok _     -> failwith "result is not error"
            | Error er -> er

        let sequence (results: Result<'T,'E> seq) =
            let folder state (acc: Result<'T list,'E list>) =
                match state, acc with
                | Ok v, Ok items     -> Ok(v :: items)
                | Ok r, Error ers    -> Error ers
                | Error e, Ok items  -> Error [e]
                | Error e, Error ers -> Error(e :: ers)

            Seq.foldBack folder results (Ok List.empty)

        let toEmptyIO (results: Result<'T,'E> seq) =
            results |> sequence |> Result.map(ignore) |> Result.mapError(List.head) |> Task.singleton

    type MaybeBuilder() =

        member x.Bind(m, bind) =
            match m with
            | Some value -> bind value
            | None       -> None

        member x.Return(value) = Some value
        member x.ReturnFrom(value) = value

    let maybe = MaybeBuilder()

    module Option =

        let ofRecord (value: 'T) =
            let boxed = box(value)
            if isNotNull(boxed) then Some value
            else None

    module String =

        let replace (oldValue: string, newValue: string) (str: string) =
            str.Replace(oldValue, newValue)

        let concatWithCommaAndQuotes (strings: string seq) =
            strings |> Seq.map(sprintf "'%s'") |> String.concat(", ")

        let filterDuplicates (data: string list) =
            data
            |> List.groupBy(id)
            |> List.choose(fun (key, set) -> if set.Length > 1 then Some key else None)

        let toOption (str: string) =
            if String.IsNullOrWhiteSpace str then None
            else Some str

    module Array =

        /// Safe variant of `Array.min`
        let minOrDefault defaultValue array =
            if Array.isEmpty array then defaultValue
            else Array.min array

        /// Safe variant of `Array.max`
        let maxOrDefault defaultValue array =
            if Array.isEmpty array then defaultValue
            else Array.max array

        /// Safe variant of `Array.average`
        let averageOrDefault (defaultValue: float) array =
            if Array.isEmpty array then defaultValue
            else array |> Array.average

        /// Safe variant of `Array.average`
        let averageByOrDefault (defaultValue: float) f array =
            if Array.isEmpty array then defaultValue
            else array |> Array.averageBy f

        /// shuffle an array (in-place)
        let shuffleInPlace a =
            let swap (a: _[]) x y =
                let tmp = a.[x]
                a.[x] <- a.[y]
                a.[y] <- tmp

            let rand = Random()
            Array.iteri (fun i _ -> swap a i (rand.Next(i, Array.length a))) a

        /// copy and shuffle
        let shuffle a =
            let a' = a |> Array.copy
            a' |> shuffleInPlace
            a'

    module Map =

        let inline ofDictionary (dictionary) =
            dictionary
            |> Seq.map (|KeyValue|)
            |> Map.ofSeq

    type Dict<'k, 'v> = System.Collections.Generic.IDictionary<'k,'v>

    module Dict =

        let inline empty<'K,'V when 'K: equality> =
            System.Collections.Generic.Dictionary<'K,'V>()
            :> Dict<'K,'V>

namespace NBomber.Extensions.Operator

module internal Result =

    let (>=>) f1 f2 arg =
        match f1 arg with
        | Ok data -> f2 data
        | Error e -> Error e
