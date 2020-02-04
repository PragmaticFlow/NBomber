namespace NBomber.Extensions

open System
open System.Threading.Tasks

[<AutoOpen>]
module internal Extensions =

    type FastCancellationToken = { mutable ShouldCancel: bool }

    type Task<'T> with
        static member map f (m: Task<_>) =
            m.ContinueWith(fun (t: Task<_>) -> f t.Result)

    type Result<'T,'TError> with
        static member getOk (result) =
            match result with
            | Ok v    -> v
            | Error _ -> failwith "result is error"

        static member getError (result) =
            match result with
            | Ok _     -> failwith "result is not error"
            | Error er -> er

        static member sequence (results: Result<'T,'e>[]) =
            let folder state (acc: Result<'T [],'e []>) =
                match state, acc with
                | Ok v, Ok items     -> Ok(Array.append items [| v |])
                | Ok r, Error ers    -> Error ers
                | Error e, Ok items  -> Error [|e|]
                | Error e, Error ers -> Error(Array.append ers [| e |])

            Array.foldBack folder results (Ok Array.empty)

    type MaybeBuilder() =

        member x.Bind(m, bind) =
            match m with
            | Some value -> bind value
            | None       -> None

        member x.Return(value) = Some value
        member x.ReturnFrom(value) = value

    let maybe = MaybeBuilder()

    module String =

        let replace (oldValue: string, newValue: string) (str: string) =
            str.Replace(oldValue, newValue)

        let concatWithCommaAndQuotes (strings: string seq) =
            strings |> Seq.map(sprintf "'%s'") |> String.concat(", ")

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
        let inline fromDictionary (dictionary) =
            dictionary
            |> Seq.map (|KeyValue|)
            |> Map.ofSeq

    type Dict<'k, 'v> = System.Collections.Generic.IDictionary<'k,'v>

    module Dict =
        let isEmpty (dictionary: Dict<'K,'V>) =
            dictionary.Count = 0

        let mapValues (f: 'V1 -> 'V2) (dictionary: Dict<'K,'V1>) =
            dictionary
            |> Seq.map (fun (KeyValue(k,v)) -> k, f v)
            |> dict

        let fillFrom (other: Dict<'K,'T>) (dictionary: Dict<'K,'T>) =
            for KeyValue(k,v) in other do
                dictionary.[k] <- v

namespace NBomber.Extensions.Operator

module internal Result =

    let (>=>) f1 f2 arg =
        match f1 arg with
        | Ok data -> f2 data
        | Error e -> Error e
