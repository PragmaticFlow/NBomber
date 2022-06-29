namespace NBomber.Extensions

open System
open System.Collections.Generic
open System.IO
open System.Text
open System.Threading.Tasks

open Json.Net.DataSetConverters
open FsToolkit.ErrorHandling
open Newtonsoft.Json

module internal Internal =

    module Operation =

        let waitOnComplete (retryCount: int) (getResult: unit -> Task<Result<'T,'E>>) = backgroundTask {
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

    module Json =

        let toJson (object) =
            let sb = StringBuilder()
            use sw = new StringWriter(sb)
            use writer = new JsonTextWriter(sw)

            let serializer = JsonSerializer()
            serializer.Converters.Add(DataSetConverter())
            serializer.Serialize(writer, object)

            sb.ToString()

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
            |> Seq.groupBy(id)
            |> Seq.choose(fun (key, set) -> if Seq.length(set) > 1 then Some key else None)

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
