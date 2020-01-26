[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Concurrency.Scheduler

open System
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Errors

type SchedulerResult =
    | AddConstantActors    of count:uint32
    | RemoveConstantActors of count:uint32
    | AddOneTimeActors     of count:uint32
    | DoNothing

let schedule (invokeSchedulePerSec: uint32,
              constantRunCopiesCount: uint32,
              runningStrategy: ConcurrencyStrategy) =

    match runningStrategy with
    | Constant (copiesCount, duration) ->
        if constantRunCopiesCount < copiesCount then [AddConstantActors(copiesCount - constantRunCopiesCount)]
        elif constantRunCopiesCount > copiesCount then [RemoveConstantActors(constantRunCopiesCount - copiesCount)]
        else [DoNothing]

    | AddPerSec (copiesCount, duration) ->
        [RemoveConstantActors(constantRunCopiesCount)
         AddOneTimeActors(copiesCount / invokeSchedulePerSec)]

    | RampTo (copiesCount, duration) ->
        if constantRunCopiesCount < copiesCount then
            let copiesToStart = copiesCount - constantRunCopiesCount
            let copiesPerSec = copiesToStart / uint32 duration.TotalSeconds
            [AddConstantActors(copiesPerSec / invokeSchedulePerSec)]

        elif constantRunCopiesCount > copiesCount then
            let copiesToStop = constantRunCopiesCount - copiesCount
            let copiesPerSec = copiesToStop / uint32 duration.TotalSeconds
            [RemoveConstantActors(copiesPerSec / invokeSchedulePerSec)]

        else [DoNothing]
