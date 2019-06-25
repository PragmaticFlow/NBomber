namespace NBomber.Extensions

open System
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open FSharp.Control.Tasks.V2.ContextInsensitive
open NBomber.Contracts

[<Extension>]
type TaskExtensions() =

    [<Extension>]
    static member TimeoutAfter(request: Task<'T>, 
                               duration: TimeSpan, 
                               cancellationToken: CancellationToken,
                               response: 'T -> Response) = task {
        let! completedTask = Task.WhenAny(request, Task.Delay(duration, cancellationToken))
        match completedTask.Equals(request) with
        | true  -> return response(request.Result)
        | false -> return Response.Fail()
    }

    [<Extension>]
    static member TimeoutAfter(request: Task, 
                               duration: TimeSpan, 
                               cancellationToken: CancellationToken,
                               response: unit -> Response) = task {
        let! completedTask = Task.WhenAny(request, Task.Delay(duration, cancellationToken))
        match completedTask.Equals(request) with
        | true  -> return response()
        | false -> return Response.Fail()
    }