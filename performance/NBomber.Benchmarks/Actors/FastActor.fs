namespace NBomber.Benchmarks.Actors

open System.Collections.Concurrent
open System.Threading

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

