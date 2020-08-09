namespace NBomber.Extensions

open System
open FsToolkit.ErrorHandling
open Nessos.Streams

module internal InternalExtensions =

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
        member x.Zero () = None

    let maybe = MaybeBuilder()

    module Option =

        let ofRecord (value: 'T) =
            let boxed = box(value)
            if isNotNull(boxed) then Some value
            else None

    module String =

        let replace (oldValue: string, newValue: string) (str: string) =
            str.Replace(oldValue, newValue)

        let splitLines (str: string) =
            str.Split([| Environment.NewLine |], StringSplitOptions.None)

        let concatLines (strings: string seq) =
            String.Join(Environment.NewLine, strings)

        let concatWithCommaAndQuotes (strings: string seq) =
            strings |> Seq.map(sprintf "'%s'") |> String.concat(", ")

        let filterDuplicates (data: string seq) =
            data
            |> Seq.groupBy(id)
            |> Seq.choose(fun (key, set) -> if Seq.length(set) > 1 then Some key else None)

        let toOption (str: string) =
            if String.IsNullOrWhiteSpace str then None
            else Some str

        let contains (value: string) (strings: string seq) =
            strings |> Seq.exists(fun x -> x = value)

        let inline appendNewLine (str: string) =
            str + Environment.NewLine

    module Stream =

        /// Safe variant of `Array.min`
        let minOrDefault defaultValue stream =
            if Stream.isEmpty stream then defaultValue
            else stream |> Stream.minBy id

        /// Safe variant of `Array.average`
        let averageOrDefault (defaultValue: float) stream =
            if Stream.isEmpty stream then defaultValue
            else stream |> Stream.toSeq |> Seq.average

        /// Safe variant of `Array.max`
        let maxOrDefault defaultValue stream =
            if Stream.isEmpty stream then defaultValue
            else stream |> Stream.maxBy id

        let inline sumBy (projection) (source: Stream<'T>) =
            source |> Stream.map(projection) |> Stream.sum

    module Array =

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

module internal Option =

    let inline (|??) (a: 'a option) (b: 'a option) =
        match a with
        | Some _ -> a
        | None   -> b

module internal Result =

    let (>=>) f1 f2 arg =
        match f1 arg with
        | Ok data -> f2 data
        | Error e -> Error e
