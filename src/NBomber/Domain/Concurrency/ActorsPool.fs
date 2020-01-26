[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Concurrency.ActorsPool

open System
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Errors

type GeneralActorsPool() =

    member x.GetActors(count: uint32) =
        ()

type ConstantActorsPool(genPool: GeneralActorsPool) =

    member x.AddActors(count: uint32) =
        ()

    member x.RemoveActors(count: uint32) =
        ()

type OneTimeActorsPool(genPool: GeneralActorsPool) =

    member x.AddActors(count: uint32) =
        ()

    member x.RemoveActors(count: uint32) =
        ()
