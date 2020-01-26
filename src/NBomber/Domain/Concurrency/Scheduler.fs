[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Concurrency.Scheduler

open System
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Errors

type SchedulerResult =
    | AddConstantCopies    of count:uint32
    | RemoveConstantCopies of count:uint32
    | AddOneTimeCopies     of count:uint32
    | DoNothing

let schedule (invokeSchedulePerSec: uint32,
              constantRunCopiesCount: uint32,
              runningStrategy: ConcurrencyStrategy) =

    match runningStrategy with
    | Constant (copiesCount, duration) ->
        if constantRunCopiesCount < copiesCount then [AddConstantCopies(copiesCount - constantRunCopiesCount)]
        elif constantRunCopiesCount > copiesCount then [RemoveConstantCopies(constantRunCopiesCount - copiesCount)]
        else [DoNothing]

    | AddPerSec (copiesCount, duration) ->
        [RemoveConstantCopies(constantRunCopiesCount)
         AddOneTimeCopies(copiesCount / invokeSchedulePerSec)]

    | RampTo (copiesCount, duration) ->
        if constantRunCopiesCount < copiesCount then
            let copiesToStart = copiesCount - constantRunCopiesCount
            let copiesPerSec = copiesToStart / uint32 duration.TotalSeconds
            [AddConstantCopies(copiesPerSec / invokeSchedulePerSec)]

        elif constantRunCopiesCount > copiesCount then
            let copiesToStop = constantRunCopiesCount - copiesCount
            let copiesPerSec = copiesToStop / uint32 duration.TotalSeconds
            [RemoveConstantCopies(copiesPerSec / invokeSchedulePerSec)]

        else [DoNothing]
