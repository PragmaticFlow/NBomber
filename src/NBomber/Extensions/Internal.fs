namespace NBomber.Extensions

open System
open System.Collections.Generic
open System.Diagnostics
open System.Text
open System.Threading.Tasks

open FSharp.Json
open FsToolkit.ErrorHandling

module internal Internal =

    module Converter =

        let inline fromMicroSecToMs (microSec: float) = (microSec / 1000.0)

        let inline fromMsToMicroSec (ms: float) = (ms * 1000.0) |> int

        let inline fromBytesToKb (bytes) = Math.Round(float bytes / 1024.0, 3)

        let inline fromBytesToMb (bytes) = Math.Round(decimal bytes / 1024.0M / 1024.0M, 1)

        let inline round (digits: int) (value: float) = Math.Round(value, digits)

        let inline roundDuration (duration: TimeSpan) =
            TimeSpan(duration.Days, duration.Hours, duration.Minutes, duration.Seconds)

    type Operation =

        static member retry (retryCount: int, getResult: unit -> Task<Result<'T,'E>>) = backgroundTask {
            let mutable counter = 1
            let mutable result = Unchecked.defaultof<_>
            let! r = getResult()
            result <- r

            while Result.isError result && counter < retryCount do
                counter <- counter + 1
                let! r = getResult()
                result <- r

            return result
        }

        static member retryDuring (duration: TimeSpan, getResult: unit -> Task<Result<'T,'E>>) =            
            Operation.retryDuring(duration, None, getResult)

        static member retryDuring (duration: TimeSpan,
                                   retryDelay: TimeSpan,
                                   getResult: unit -> Task<Result<'T,'E>>,
                                   ?shouldRetry: Result<'T,'E> -> bool) =
            
            Operation.retryDuring(duration, Some retryDelay, getResult, ?shouldRetry = shouldRetry)

        static member private retryDuring (duration: TimeSpan,
                                           retryDelay: TimeSpan option,
                                           getResult: unit -> Task<Result<'T,'E>>,
                                           ?shouldRetry: Result<'T,'E> -> bool) = backgroundTask {            
            let shouldContinue =
                shouldRetry
                |> Option.defaultValue(fun _ -> true)
            
            let stopwatch = Stopwatch()
            stopwatch.Start()

            let mutable result = Unchecked.defaultof<_>
            let! r = getResult()
            result <- r

            while Result.isError result
                  && shouldContinue result
                  && stopwatch.Elapsed < duration do

                if retryDelay.IsSome then
                    do! Task.Delay retryDelay.Value

                let! r = getResult()
                result <- r

            stopwatch.Stop()
            return result
        }

    module JsonExt =

        let private parseSettings = JsonConfig.create(allowUntyped = true)

        let serialize (data: 'T) =
            Json.serializeEx parseSettings data

        let deserialize<'T> (json: string) =
            Json.deserializeEx<'T> parseSettings json

    module Result =

        let getOk (result) =
            match result with
            | Ok v    -> v
            | Error e -> failwith "result is error"

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

    module Option =

        let ofRecord (value: 'T) =
            let boxed = box(value)
            if not(isNull boxed) then Some value
            else None

        let sequence (data: 'T option list) =
            let folder (item: 'T option) (state: ('T list) option) =
                match item, state with
                | Some v, Some list -> Some(v :: list)
                | _, _              -> None

            List.foldBack folder data (Some [])

    module String =

        let replace (oldValue: string, newValue: string) (str: string) =
            str.Replace(oldValue, newValue)

        let split (separators: string[]) (str: string) =
            str.Split(separators, StringSplitOptions.None)

        let concatLines (strings: string seq) =
            String.Join(Environment.NewLine, strings)

        let concatWithComma (strings: string seq) =
            strings |> String.concat(", ")

        let filterDuplicates (data: string seq) =
            data
            |> Seq.groupBy id
            |> Seq.choose(fun (key, set) -> if Seq.length set > 1 then Some key else None)

        let toOption (str: string) =
            if String.IsNullOrWhiteSpace str then None
            else Some str

        let contains (value: string) (strings: string seq) =
            strings |> Seq.exists(fun x -> x = value)

        let inline appendNewLine (str: string) =
            str + Environment.NewLine

    module Array =

        /// shuffle an array (in-place)
        let shuffleInPlace a =
            let swap (a: _[]) x y =
                let tmp = a[x]
                a[x] <- a[y]
                a[y] <- tmp

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

    module Dict =

        let ofSeq (src: seq<'a*'b>) =
            let d = new Dictionary<'a, 'b>()
            for (k,v) in src do
                d.Add(k,v)
            d

        let inline empty<'K,'V when 'K : equality>() =
            Dictionary<'K,'V>()

namespace NBomber.Extensions.Operator

module internal Result =

    let (>=>) f1 f2 arg =
        match f1 arg with
        | Ok data -> f2 data
        | Error e -> Error e
