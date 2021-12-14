namespace NBomber.Extensions

open System
open System.Collections.Concurrent
open System.IO
open System.Text
open System.Threading

open Json.Net.DataSetConverters
open FsToolkit.ErrorHandling
open Newtonsoft.Json

module internal InternalExtensions =

    let inline isNotNull (value) =
        not(isNull value)

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
            if isNotNull(boxed) then Some value
            else None

    module String =

        let replace (oldValue: string, newValue: string) (str: string) =
            str.Replace(oldValue, newValue)

        let split (separators: string[]) (str: string) =
            str.Split(separators, StringSplitOptions.None)

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

    module Actor =

        module Status =
            let [<Literal>] NotWorking = 0
            let [<Literal>] Working = 1

        type FastActor<'TState,'TMessage>(initialState: 'TState, handler: 'TState -> 'TMessage -> 'TState) as this =

            let _mailBox = ConcurrentQueue<'TMessage>()
            let mutable _isWorking = Status.NotWorking
            let mutable _currentState = initialState

            static let callback = WaitCallback(fun currentActor ->
                let actor = currentActor :?> FastActor<_,_>
                actor.Execute()
            )

            let trySchedule () =
                if Interlocked.CompareExchange(&_isWorking, Status.Working, Status.NotWorking) = Status.NotWorking then
                    ThreadPool.QueueUserWorkItem(callback, this) |> ignore

            let stop () =
                Interlocked.Exchange(&_isWorking, Status.NotWorking) |> ignore

            member _.Publish(message: 'TMessage) =
                _mailBox.Enqueue(message)
                trySchedule()

            member private _.Execute() =
                let rec loop () =
                    match _mailBox.TryDequeue() with
                    | true, msg ->
                        _currentState <- handler _currentState msg
                        loop()

                    | false, _ ->
                        stop()
                        if not _mailBox.IsEmpty then trySchedule()
                loop()

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
