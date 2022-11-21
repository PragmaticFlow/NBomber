namespace NBomber.Benchmarks.Actors

open System.Threading.Tasks
open System.Threading.Tasks.Dataflow
open BenchmarkDotNet.Attributes
open NBomber.CSharpImpl

type ActorMessage =
    | Inc
    | Dec
    | GetValue of awaiterTcs:TaskCompletionSource<int>

type ActorState =
    { Value: int }

    static member init () = { Value = 0 }

    static member handle (state) (msg) =
        match msg with
        | Inc -> { state with Value = state.Value + 1 }
        | Dec -> { state with Value = state.Value - 1 }
        | GetValue tcs ->
            tcs.TrySetResult(state.Value) |> ignore
            state

[<MemoryDiagnoser>]
type ActorsBenchmark() as this =

    //[<Params(100_000, 1_000_000, 10_000_000)>]
    [<Params(50_000)>]
    member val ThreadIteration = 0 with get, set

    [<Params(10)>]
    member val ThreadCount = 0 with get, set

    [<Benchmark>]
    member _.ChannelActor() =
        let actor = ChannelActor<ActorState, ActorMessage>(ActorState.init(), ActorState.handle)

        [| 1..this.ThreadCount |]
        |> Array.Parallel.iter (fun _ ->
            for i = 0 to this.ThreadIteration do
                let isAdd = i % 2 = 0
                if isAdd then actor.Publish(ActorMessage.Inc)
                else actor.Publish(ActorMessage.Dec)
        )

        let tcs = TaskCompletionSource<int>()
        actor.Publish(ActorMessage.GetValue tcs)
        tcs.Task.Wait()

    [<Benchmark>]
    member _.ChannelActorPly() =
        let actor = ChannelActorPly<ActorState, ActorMessage>(ActorState.init(), ActorState.handle)

        [| 1..this.ThreadCount |]
        |> Array.Parallel.iter (fun _ ->
            for i = 0 to this.ThreadIteration do
                let isAdd = i % 2 = 0
                if isAdd then actor.Publish(ActorMessage.Inc)
                else actor.Publish(ActorMessage.Dec)
        )

        let tcs = TaskCompletionSource<int>()
        actor.Publish(ActorMessage.GetValue tcs)
        tcs.Task.Wait()

    [<Benchmark>]
    member _.ChannelActorCSharp() =
        let actor = ChannelActorCSharp<ActorState, ActorMessage>(ActorState.init(), ActorState.handle)

        [| 1..this.ThreadCount |]
        |> Array.Parallel.iter (fun _ ->
            for i = 0 to this.ThreadIteration do
                let isAdd = i % 2 = 0
                if isAdd then actor.Publish(ActorMessage.Inc)
                else actor.Publish(ActorMessage.Dec)
        )

        let tcs = TaskCompletionSource<int>()
        actor.Publish(ActorMessage.GetValue tcs)
        tcs.Task.Wait()

    //[<Benchmark>]
    member _.FastActor() =
        let actor = Actor.FastActor<ActorState, ActorMessage>(ActorState.init(), ActorState.handle)

        [| 1..this.ThreadCount |]
        |> Array.Parallel.iter (fun _ ->
            for i = 0 to this.ThreadIteration do
                let isAdd = i % 2 = 0
                if isAdd then actor.Publish(ActorMessage.Inc)
                else actor.Publish(ActorMessage.Dec)
        )

        let tcs = TaskCompletionSource<int>()
        actor.Publish(ActorMessage.GetValue tcs)
        tcs.Task.Wait()

    //[<Benchmark>]
    member _.TPLDataFlow() =
        let mutable state = ActorState.init()

        let actor = ActionBlock(fun msg ->
            backgroundTask {
                state <- ActorState.handle state msg
            }
            :> Task
        )

        [| 1..this.ThreadCount |]
        |> Array.Parallel.iter (fun _ ->
            for i = 0 to this.ThreadIteration do
                let isAdd = i % 2 = 0
                if isAdd then actor.Post(ActorMessage.Inc) |> ignore
                else actor.Post(ActorMessage.Dec) |> ignore
        )

        let tcs = TaskCompletionSource<int>()
        actor.Post(ActorMessage.GetValue tcs) |> ignore
        tcs.Task.Wait()

    //[<Benchmark>]
    member _.MailBox() =
        let mutable state = ActorState.init()

        let actor = MailboxProcessor.Start(fun mailbox ->
            let rec loop () = async {
                let! msg = mailbox.Receive()
                state <- ActorState.handle state msg
                return! loop()
            }

            loop()
        )

        [| 1..this.ThreadCount |]
        |> Array.Parallel.iter (fun _ ->
            for i = 0 to this.ThreadIteration do
                let isAdd = i % 2 = 0
                if isAdd then actor.Post(ActorMessage.Inc) |> ignore
                else actor.Post(ActorMessage.Dec) |> ignore
        )

        let tcs = TaskCompletionSource<int>()
        actor.Post(ActorMessage.GetValue tcs) |> ignore
        tcs.Task.Wait()
