module internal NBomber.Domain.Stats.LocalScenarioStatsActor

open System.Runtime.CompilerServices
open System.Threading.Tasks.Dataflow

open NBomber
open NBomber.Contracts.Internal
open NBomber.Domain.Stats.GlobalScenarioStatsActor

type ActorMessage =
    | AddResponse of StepResponse
    | FlushBuffer

type LocalScenarioStatsActor(globalStatsActor: GlobalScenarioStatsActor) =

    let _buffer = ResizeArray<StepResponse>()

    let flushBuffer () =
        let responses = globalStatsActor.GetEmptyResponsesFromPool(_buffer.Count)
        _buffer.CopyTo(responses)
        _buffer.Clear()
        globalStatsActor.Publish(ActorMessage.AddResponses responses)

    let _actor = ActionBlock(fun msg ->
        match msg with
        | AddResponse resp ->
            _buffer.Add(resp)
            if _buffer.Count >= Constants.ResponseBufferLength then
                flushBuffer()

        | FlushBuffer ->
            if _buffer.Count > 0 then flushBuffer()
    )

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.Publish(msg) = _actor.Post(msg) |> ignore
